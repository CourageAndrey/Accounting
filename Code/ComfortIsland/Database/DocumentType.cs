﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.Database
{
	public enum DocumentType
	{
		Income,
		Outcome,
		Produce,
	}

	public delegate bool ValidateDocumentDelegate(Document document, StringBuilder errors);

	public delegate IDictionary<long, double> ProcessDocumentDelegate(Document document, IList<Balance> balanceTable);

	public delegate IDictionary<long, double> GetBalanceDeltaDelegate(Document document);

	internal class DocumentTypeImplementation
	{
		#region Properties

		public DocumentType Type
		{ get; private set; }

		public string Name
		{ get; private set; }

		public GetBalanceDeltaDelegate GetBalanceDelta
		{ get; private set; }

		public ValidateDocumentDelegate Validate
		{ get; private set; }

		public ProcessDocumentDelegate Process
		{ get; private set; }

		public ProcessDocumentDelegate ProcessBack
		{ get; private set; }

		#endregion

		private DocumentTypeImplementation(
			DocumentType type,
			string name,
			GetBalanceDeltaDelegate getBalanceDelta,
			ValidateDocumentDelegate validate = null,
			ProcessDocumentDelegate process = null,
			ProcessDocumentDelegate processBack = null)
		{
			Type = type;
			Name = name;
			GetBalanceDelta = getBalanceDelta;
			Validate = validate ?? validateDefault;
			Process = process ?? processDefault;
			ProcessBack = processBack ?? processBackDefault;
		}

		#region List

		public static readonly DocumentTypeImplementation Income;

		public static readonly DocumentTypeImplementation Outcome;

		public static readonly DocumentTypeImplementation Produce;

		public static readonly IDictionary<DocumentType, DocumentTypeImplementation> AllTypes;

		#endregion

		static DocumentTypeImplementation()
		{
			Income = new DocumentTypeImplementation(DocumentType.Income, "приход", getBalanceDeltaIncome);
			Outcome = new DocumentTypeImplementation(DocumentType.Outcome, "продажа", getBalanceDeltaOutcome);
			Produce = new DocumentTypeImplementation(DocumentType.Produce, "производство", getBalanceDeltaProduce, validateProduce);
			AllTypes = new ReadOnlyDictionary<DocumentType, DocumentTypeImplementation>(new Dictionary<DocumentType, DocumentTypeImplementation>
			{
				{ DocumentType.Income, Income },
				{ DocumentType.Outcome, Outcome },
				{ DocumentType.Produce, Produce },
			});
		}

		#region Common default implementations

		private static bool validateDefault(Document document, StringBuilder errors)
		{
			var allBalance = Database.Instance.Balance;
			var products = Database.Instance.Products;
			foreach (var position in AllTypes[document.Type].GetBalanceDelta(document))
			{
				var balance = allBalance.FirstOrDefault(b => b.ProductId == position.Key);
				double count = balance != null ? balance.Count : 0;
				if ((count + position.Value) < 0)
				{
					errors.AppendLine(string.Format(
						CultureInfo.InvariantCulture,
						"Недостаточно товара \"{0}\". Имеется по факту {1}, требуется {2}.",
						products.First(p => p.ID == position.Key).DisplayMember,
						count,
						-position.Value));
				}
			}
			return errors.Length == 0;
		}

		private static bool validateProduce(Document document, StringBuilder errors)
		{
			var products = Database.Instance.Products;
			foreach (var position in document.PositionsToSerialize)
			{
				var product = products.First(p => p.ID == position.ID);
				if (product.Children.Count == 0)
				{
					errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Товар {0} не может быть произведён, так как ни из чего не состоит.", product.DisplayMember));
				}
			}
			validateDefault(document, errors);
			return errors.Length == 0;
		}

		private static IDictionary<long, double> processDefault(Document document, IList<Balance> balanceTable)
		{
			var delta = AllTypes[document.Type].GetBalanceDelta(document);
			foreach (var position in delta)
			{

				var balance = balanceTable.FirstOrDefault(b => b.ProductId == position.Key);
				if (balance != null)
				{
					balance.Count += position.Value;
				}
				else
				{
					balanceTable.Add(new Balance(position.Key, position.Value));
				}
			}
			return delta;
		}

		private static IDictionary<long, double> processBackDefault(Document document, IList<Balance> balanceTable)
		{
			var delta = AllTypes[document.Type].GetBalanceDelta(document);
			foreach (var position in delta)
			{
				var balance = balanceTable.First(b => b.ProductId == position.Key);
				balance.Count -= position.Value;
			}
			return delta;
		}

		#endregion

		#region GetDelta-methods

		private static IDictionary<long, double> getBalanceDeltaIncome(Document document)
		{
			return document.PositionsToSerialize.ToDictionary(p => p.ID, p => p.Count);
		}

		private static IDictionary<long, double> getBalanceDeltaOutcome(Document document)
		{
			return document.PositionsToSerialize.ToDictionary(p => p.ID, p => -p.Count);
		}

		private static IDictionary<long, double> getBalanceDeltaProduce(Document document)
		{
			var result = new Dictionary<long, double>();
			var products = Database.Instance.Products;
			foreach (var position in document.PositionsToSerialize)
			{
				result[position.ID] = position.Count;
				foreach (var child in products.First(p => p.ID == position.ID).Children)
				{
					double count;
					if (result.TryGetValue(child.Key.ID, out count))
					{
						count -= (position.Count * child.Value);
					}
					else
					{
						count = -(position.Count * child.Value);
					}
					result[child.Key.ID] = count;
				}
			}
			return result;
		}

		#endregion
	}
}

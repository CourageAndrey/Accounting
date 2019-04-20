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
		ToWarehouse,
	}

	public delegate IDictionary<long, double> GetBalanceDeltaDelegate(Database database, Document document);

	internal class DocumentTypeImplementation
	{
		#region Properties

		public DocumentType Type
		{ get; private set; }

		public string Name
		{ get; private set; }

		public GetBalanceDeltaDelegate GetBalanceDelta
		{ get; private set; }

		#endregion

		private DocumentTypeImplementation(DocumentType type, string name, GetBalanceDeltaDelegate getBalanceDelta)
		{
			Type = type;
			Name = name;
			GetBalanceDelta = getBalanceDelta;
		}

		#region List

		public static readonly DocumentTypeImplementation Income;

		public static readonly DocumentTypeImplementation Outcome;

		public static readonly DocumentTypeImplementation Produce;

		public static readonly DocumentTypeImplementation ToWarehouse;

		public static readonly IDictionary<DocumentType, DocumentTypeImplementation> AllTypes;

		#endregion

		static DocumentTypeImplementation()
		{
			Income = new DocumentTypeImplementation(DocumentType.Income, "приход", getBalanceDeltaIncome);
			Outcome = new DocumentTypeImplementation(DocumentType.Outcome, "продажа", getBalanceDeltaOutcome);
			Produce = new DocumentTypeImplementation(DocumentType.Produce, "производство", getBalanceDeltaProduce);
			ToWarehouse = new DocumentTypeImplementation(DocumentType.ToWarehouse, "перемещение на склад", getBalanceDeltaOutcome);
			AllTypes = new ReadOnlyDictionary<DocumentType, DocumentTypeImplementation>(new Dictionary<DocumentType, DocumentTypeImplementation>
			{
				{ DocumentType.Income, Income },
				{ DocumentType.Outcome, Outcome },
				{ DocumentType.Produce, Produce },
				{ DocumentType.ToWarehouse, ToWarehouse },
			});
		}

		public bool Validate(Database database, Document document, StringBuilder errors)
		{
			foreach (var position in GetBalanceDelta(database, document))
			{
				var balance = database.Balance.FirstOrDefault(b => b.ProductId == position.Key);
				double count = balance != null ? balance.Count : 0;
				if ((count + position.Value) < 0)
				{
					errors.AppendLine(string.Format(
						CultureInfo.InvariantCulture,
						"Недостаточно товара \"{0}\". Имеется по факту {1}, требуется {2}.",
						database.Products.First(p => p.ID == position.Key).DisplayMember,
						count,
						-position.Value));
				}
			}
			if (document.Type == DocumentType.Produce)
			{
				foreach (var position in document.PositionsToSerialize)
				{
					var product = database.Products.First(p => p.ID == position.ID);
					if (product.Children.Count == 0)
					{
						errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Товар {0} не может быть произведён, так как ни из чего не состоит.", product.DisplayMember));
					}
				}
			}
			return errors.Length == 0;
		}

		public IDictionary<long, double> Apply(Database database, Document document, IList<Balance> balanceTable)
		{
			var delta = GetBalanceDelta(database, document);
			foreach (var position in delta)
			{

				var balance = balanceTable.FirstOrDefault(b => b.ProductId == position.Key);
				if (balance != null)
				{
					balance.Count += position.Value;
				}
				else
				{
					balanceTable.Add(new Balance(database, position.Key, position.Value));
				}
			}
			return delta;
		}

		public IDictionary<long, double> Rollback(Database database, Document document, IList<Balance> balanceTable)
		{
			var delta = GetBalanceDelta(database, document);
			foreach (var position in delta)
			{
				var balance = balanceTable.First(b => b.ProductId == position.Key);
				balance.Count -= position.Value;
			}
			return delta;
		}

		#region GetDelta-methods

		private static IDictionary<long, double> getBalanceDeltaIncome(Database database, Document document)
		{
			return document.PositionsToSerialize.ToDictionary(p => p.ID, p => p.Count);
		}

		private static IDictionary<long, double> getBalanceDeltaOutcome(Database database, Document document)
		{
			return document.PositionsToSerialize.ToDictionary(p => p.ID, p => -p.Count);
		}

		private static IDictionary<long, double> getBalanceDeltaProduce(Database database, Document document)
		{
			var result = new Dictionary<long, double>();
			foreach (var position in document.PositionsToSerialize)
			{
				result[position.ID] = position.Count;
				foreach (var child in database.Products.First(p => p.ID == position.ID).Children)
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

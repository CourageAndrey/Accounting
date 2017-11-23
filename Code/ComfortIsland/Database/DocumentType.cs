using System.Collections.Generic;
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

	public delegate void ValidateDocumentDelegate(Document document, StringBuilder errors);

	public delegate void ProcessDocumentDelegate(Document document, IList<Balance> balanceTable);

	public delegate IDictionary<long, double> GetBalanceDeltaDelegate(Document document);

	internal class DocumentTypeImplementation
	{
		#region Properties

		public DocumentType Type
		{ get; private set; }

		public string Name
		{ get; private set; }

		public ValidateDocumentDelegate Validate
		{ get; private set; }

		public ProcessDocumentDelegate Process
		{ get; private set; }

		public ProcessDocumentDelegate ProcessBack
		{ get; private set; }

		#endregion

		#region Constructors

		private DocumentTypeImplementation(DocumentType type, string name, GetBalanceDeltaDelegate getBalanceDelta)
			: this(type,
				   name,
				   (document, errors) => validateDefault(getBalanceDelta(document), errors),
				   (document, balanceTable) => processDefault(getBalanceDelta(document), balanceTable),
				   (document, balanceTable) => processBackDefault(getBalanceDelta(document), balanceTable))
		{ }

		private DocumentTypeImplementation(DocumentType type, string name, ValidateDocumentDelegate validate, ProcessDocumentDelegate process, ProcessDocumentDelegate processBack)
		{
			Type = type;
			Name = name;
			Validate = validate;
			Process = process;
			ProcessBack = processBack;
		}

		#endregion

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
			Produce = new DocumentTypeImplementation(DocumentType.Produce, "производство",
				validateProduce,
				(document, balanceTable) => processDefault(getBalanceDeltaProduce(document), balanceTable),
				(document, balanceTable) => processBackDefault(getBalanceDeltaProduce(document), balanceTable));
			AllTypes = new ReadOnlyDictionary<DocumentType, DocumentTypeImplementation>(new Dictionary<DocumentType, DocumentTypeImplementation>
			{
				{ DocumentType.Income, Income },
				{ DocumentType.Outcome, Outcome },
				{ DocumentType.Produce, Produce },
			});
		}

		#region Common default implementations

		private static void validateDefault(IDictionary<long, double> balanceDelta, StringBuilder errors)
		{
			var allBalance = Database.Instance.Balance;
			var products = Database.Instance.Products;
			foreach (var position in balanceDelta)
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
		}

		private static void validateProduce(Document document, StringBuilder errors)
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
			validateDefault(getBalanceDeltaProduce(document), errors);
		}

		private static void processDefault(IDictionary<long, double> balanceDelta, IList<Balance> balanceTable)
		{
			foreach (var position in balanceDelta)
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
		}

		private static void processBackDefault(IDictionary<long, double> balanceDelta, IList<Balance> balanceTable)
		{
			foreach (var position in balanceDelta)
			{
				var balance = balanceTable.First(b => b.ProductId == position.Key);
				balance.Count -= position.Value;
			}
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

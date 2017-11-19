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

	public delegate void ProcessDocumentDelegate(Document document);

	public class DocumentTypeImplementation
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

		#endregion

		private DocumentTypeImplementation(DocumentType type, string name, ValidateDocumentDelegate validate, ProcessDocumentDelegate process)
		{
			Type = type;
			Name = name;
			Validate = validate;
			Process = process;
		}

		#region List

		public static readonly DocumentTypeImplementation Income;

		public static readonly DocumentTypeImplementation Outcome;

		public static readonly DocumentTypeImplementation Produce;

		public static readonly IDictionary<DocumentType, DocumentTypeImplementation> AllTypes;

		#endregion

		static DocumentTypeImplementation()
		{
			Income = new DocumentTypeImplementation(DocumentType.Income, "приход", validateIncome, processIncome);
			Outcome = new DocumentTypeImplementation(DocumentType.Outcome, "продажа", validateOutcome, processOutcome);
			Produce = new DocumentTypeImplementation(DocumentType.Produce, "производство", validateProduce, processProduce);
			AllTypes = new ReadOnlyDictionary<DocumentType, DocumentTypeImplementation>(new Dictionary<DocumentType, DocumentTypeImplementation>
			{
				{ DocumentType.Income, Income },
				{ DocumentType.Outcome, Outcome },
				{ DocumentType.Produce, Produce },
			});
		}

		#region Validate-methods

		private static void validateIncome(Document document, StringBuilder errors)
		{
			validate(new Position[0], Database.Instance.Products, errors);
		}

		private static void validateOutcome(Document document, StringBuilder errors)
		{
			validate(document.PositionsToSerialize, Database.Instance.Products, errors);
		}

		private static void validateProduce(Document document, StringBuilder errors)
		{
			var products = Database.Instance.Products;
			var positionsToCheck = new List<Position>();
			foreach (var position in document.PositionsToSerialize)
			{
				var product = products.First(p => p.ID == position.ID);
				if (product.Children.Count == 0)
				{
					errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Товар {0} не может быть произведён, так как ни из чего не состоит.", product.DisplayMember));
				}
				foreach (var child in product.Children)
				{
					var existing = positionsToCheck.FirstOrDefault(p => p.ID == child.Key.ID);
					if (existing != null)
					{
						existing.Count += (position.Count * child.Value);
					}
					else
					{
						positionsToCheck.Add(new Position(child.Key.ID, position.Count * child.Value));
					}
				}
			}
			validate(positionsToCheck, products, errors);
		}

		private static void validate(IEnumerable<Position> positionsToCheck, IEnumerable<Product> products, StringBuilder errors)
		{
			var allBalance = Database.Instance.Balance;
			foreach (var position in positionsToCheck)
			{
				var balance = allBalance.FirstOrDefault(b => b.ProductId == position.ID);
				long count = balance != null ? balance.Count : 0;
				if (count < position.Count)
				{
					errors.AppendLine(string.Format(
						CultureInfo.InvariantCulture,
						"Недостаточно товара \"{0}\". Имеется по факту {1}, требуется {2}.",
						products.First(p => p.ID == position.ID).DisplayMember,
						count,
						position.Count));
				}
			}
		}

		#endregion

		#region Process-methods

		private static void processIncome(Document document)
		{
			var balanceTable = Database.Instance.Balance;
			foreach (var position in document.Positions)
			{
				var balance = balanceTable.FirstOrDefault(b => b.ProductId == position.Key.ID);
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

		private static void processOutcome(Document document)
		{
			var balanceTable = Database.Instance.Balance;
			foreach (var position in document.Positions)
			{
				var balance = balanceTable.First(b => b.ProductId == position.Key.ID);
				balance.Count -= position.Value;
			}
		}

		private static void processProduce(Document document)
		{
			var balanceTable = Database.Instance.Balance;
			foreach (var position in document.Positions)
			{
				var product = position.Key;

				// increase balance
				var balance = balanceTable.FirstOrDefault(b => b.ProductId == product.ID);
				if (balance != null)
				{
					balance.Count += position.Value;
				}
				else
				{
					balanceTable.Add(new Balance(position.Key, position.Value));
				}

				// decrease balance
				foreach (var child in product.Children)
				{
					balance = balanceTable.First(b => b.ProductId == child.Key.ID);
					balance.Count -= (position.Value * child.Value);
				}
			}
		}

		#endregion
	}
}

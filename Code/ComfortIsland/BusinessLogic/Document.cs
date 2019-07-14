using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

using ComfortIsland.Helpers;

namespace ComfortIsland.BusinessLogic
{
	public class Document : IEntity
	{
		#region Properties

		public long ID
		{ get; set; }

		public long? PreviousVersionId
		{ get; }

		public string Number
		{ get; set; }

		public DateTime Date
		{ get; set; }

		public DocumentType Type
		{ get; }

		public DocumentState State
		{ get; private set; }

		public Dictionary<Product, double> Positions
		{
			get { return positions; }
			set
			{
				var errors = new StringBuilder();
				if (PositionsCountHasToBePositive(value, errors) &
					PositionCountsArePositive(value, errors) &
					PositionDoNotDuplicate(value, errors))
				{
					positions = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		private Dictionary<Product, double> positions;

		#endregion

		#region Constructors

		public Document(long? previousVersionId, DocumentType type, DocumentState state)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (state == null) throw new ArgumentNullException(nameof(state));

			PreviousVersionId = previousVersionId;
			Type = type;
			State = state;

			positions = new Dictionary<Product, double>();
		}

		public Document(DocumentType type)
			: this(null, type, DocumentState.Active)
		{ }

		public Document(Document previousVersion)
			: this(previousVersion.ID, previousVersion.Type, DocumentState.Active)
		{
			if (previousVersion.State == DocumentState.Active)
			{
				previousVersion.State = DocumentState.Edited;
			}
		}

		#endregion

		#region Validation

		public static bool PositionsCountHasToBePositive(Dictionary<Product, double> children, StringBuilder errors)
		{
			if (children.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool PositionCountsArePositive(Dictionary<Product, double> children, StringBuilder errors)
		{
			if (children.Any(c => c.Value <= 0))
			{
				errors.AppendLine("Количество товара во всех позициях должно быть строго больше ноля.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool PositionDoNotDuplicate(Dictionary<Product, double> children, StringBuilder errors)
		{
			var ids = children.Select(c => c.Key.ID).ToList();
			if (ids.Count > ids.Distinct().Count())
			{
				errors.AppendLine("Некоторые товары включены несколько раз.");
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		#region Workflow

		public static bool TryDelete(Database database, IEnumerable<Document> documentsToDelete)
		{
			// создание временной копии таблицы баланса
			var balance = database.Balance.ToPositions();

			// последовательный откат документов
			var products = new HashSet<long>();
			foreach (var document in documentsToDelete)
			{
				foreach (long productId in document.Rollback(database.Balance).Keys)
				{
					products.Add(productId);
				}
			}

			// проверка итогового баланса
			if (checkBalance(database, balance, products))
			{
				// если всё хорошо - применяем изменения в БД
				foreach (var document in documentsToDelete)
				{
					document.State = DocumentState.Deleted;
				}
				database.Balance.LoadPositions(balance);
				return true;
			}
			else
			{
				return false;
			}
		}

#warning IsNotUsed
		private bool ValidateBalance(Database database, StringBuilder errors)
		{
			foreach (var position in getBalanceDelta())
			{
				double count;
				if (!database.Balance.TryGetValue(position.Key, out count))
				{
					count = 0;
				}
				if ((count + position.Value) < 0)
				{
					errors.AppendLine(string.Format(
						CultureInfo.InvariantCulture,
						"Недостаточно товара \"{0}\". Имеется по факту {1}, требуется {2}.",
						database.Products[position.Key].DisplayMember,
						count,
						-position.Value));
				}
			}
			if (Type == DocumentType.Produce)
			{
				foreach (var position in Positions)
				{
					var product = position.Key;
					if (product.Children.Count == 0)
					{
						errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Товар {0} не может быть произведён, так как ни из чего не состоит.", product.DisplayMember));
					}
				}
			}
			return errors.Length == 0;
		}

		public IDictionary<long, double> Apply(Storage balance)
		{
			var delta = getBalanceDelta();
			foreach (var position in delta)
			{
				balance.Increase(position.Key, position.Value);
			}
			return delta;
		}

		public IDictionary<long, double> Rollback(Storage balance)
		{
			var delta = getBalanceDelta();
			foreach (var position in delta)
			{
				balance.Decrease(position.Key, position.Value);
			}
			return delta;
		}

		private IDictionary<long, double> getBalanceDelta()
		{
			return Type.GetBalanceDelta(Positions);
		}

		public bool CheckBalance(Database database, IList<Position> balanceTable, string operationNoun, string operationVerb)
		{
			return checkBalance(database, balanceTable, getBalanceDelta().Keys);
		}

		private static bool checkBalance(Database database, IList<Position> balanceTable, IEnumerable<long> products)
		{
			var wrongPositions = balanceTable.Where(p => products.Contains(p.ID) && p.Count < 0).ToList();
			if (wrongPositions.Count > 0)
			{
				var text = new StringBuilder("При выполнении данной операции остатки следующих товаров принимают отрицательные значения:");
				text.AppendLine();
				foreach (var position in wrongPositions)
				{
					var product = database.Products[position.ID];
					text.AppendLine(string.Format(" * {0} = {1}", product.DisplayMember, DigitRoundingConverter.Simplify(position.Count)));
				}
				MessageBox.Show(text.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		public StringBuilder FindUsages(Database database)
		{
			return new StringBuilder();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

using ComfortIsland.Helpers;

namespace ComfortIsland.Database
{
	[XmlType]
	public  class Document : IEntity, IEditable<Document>
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlIgnore]
		public long? PreviousVersionId
		{ get; set; }

		[XmlAttribute]
		private string PreviousVersionIdXml
		{
			get { return PreviousVersionId.ToString(); }
			set { PreviousVersionId = !string.IsNullOrEmpty(value) ? (long?) long.Parse(value) : null; }
		}

		[XmlAttribute]
		public string Number
		{ get; set; }

		[XmlAttribute]
		public DateTime Date
		{ get; set; }

		[XmlAttribute]
		public DocumentType Type
		{ get; set; }

		[XmlAttribute]
		public DocumentState State
		{ get; set; }

		[XmlIgnore]
		public string TypeName
		{ get { return DocumentTypeImplementation.AllTypes[Type].Name; } }

		[XmlIgnore]
		public string StateName
		{ get { return State.StateToString(); } }

		[XmlIgnore]
		public Dictionary<Product, double> Positions
		{ get; private set; }

		[XmlArray("Positions"), XmlArrayItem("Product")]
		public List<Position> PositionsToSerialize
		{ get; set; }

		#endregion

		public Document()
		{
			Positions = new Dictionary<Product, double>();
			PositionsToSerialize = new List<Position>();
		}

		public void Update(Document other)
		{
			this.ID = other.ID;
			this.Number = other.Number;
			this.Date = other.Date;
			this.Type = other.Type;
			this.Positions = new Dictionary<Product, double>(other.Positions);
			this.PositionsToSerialize = other.PositionsToSerialize.Select(p => new Position(p)).ToList();
		}

		public bool Validate(Database database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (PositionsToSerialize.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
			}
			bool isValid = ValidateBalance(database, errors);
			foreach (var position in PositionsToSerialize)
			{
				if (database.Products.FirstOrDefault(p => p.ID == position.ID) == null)
				{
					errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "У {0}-й позиции в списке не выбран товар.", PositionsToSerialize.IndexOf(position) + 1));
				}
				if (position.Count <= 0)
				{
					errors.AppendLine("Количество товара во всех позициях должно быть строго больше ноля.");
				}
			}
			var ids = PositionsToSerialize.Select(p => p.ID).ToList();
			if (ids.Count > ids.Distinct().Count())
			{
				errors.Append("Дублирование позиций в документе");
			}
			return isValid;
		}

		#region [De]Serialization

		public void BeforeSerialization()
		{
			BeforeEdit();
		}

		public void AfterDeserialization(Database database)
		{
			AfterEdit(database);
		}

		#endregion

		public void BeforeEdit()
		{
			PositionsToSerialize = Positions.Select(kvp => new Position(kvp.Key.ID, kvp.Value)).ToList();
		}

		public void AfterEdit(Database database)
		{
			Positions.Clear();
			foreach (var position in PositionsToSerialize)
			{
				Positions[database.Products.First(p => p.ID == position.ID)] = position.Count;
			}
		}

		#region Workflow

		public static bool TryDelete(Database database, IList<Document> documentsToDelete, IList<Balance> balance)
		{
			// приготовление к отмене
			if (documentsToDelete.Count == 0)
			{
				MessageBox.Show("Не выбрано ни одного активного документа.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			}
			if (MessageBox.Show(
				string.Format(CultureInfo.InvariantCulture, "Действительно удалить {0} выбранных документов?", documentsToDelete.Count),
				"Вопрос",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return false;
			}

			// последовательный откат документов
			var products = new HashSet<long>();
			foreach (var document in documentsToDelete)
			{
				foreach (long productId in document.Rollback(database, balance).Keys)
				{
					products.Add(productId);
				}
			}

			// проверка итогового баланса
			if (CheckBalance(database, balance, products))
			{
				// если всё хорошо - применяем изменения в БД
				foreach (var document in documentsToDelete)
				{
					document.State = DocumentState.Deleted;
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool ValidateBalance(Database database, StringBuilder errors)
		{
			foreach (var position in GetBalanceDelta(database))
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
			if (Type == DocumentType.Produce)
			{
				foreach (var position in PositionsToSerialize)
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

		public IDictionary<long, double> Apply(Database database, IList<Balance> balanceTable)
		{
			var delta = GetBalanceDelta(database);
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

		public IDictionary<long, double> Rollback(Database database, IList<Balance> balanceTable)
		{
			var delta = GetBalanceDelta(database);
			foreach (var position in delta)
			{
				var balance = balanceTable.First(b => b.ProductId == position.Key);
				balance.Count -= position.Value;
			}
			return delta;
		}

		public IDictionary<long, double> GetBalanceDelta(Database database)
		{
			return DocumentTypeImplementation.AllTypes[Type].GetBalanceDelta(database, this);
		}

		public bool CheckBalance(Database database, IList<Balance> balanceTable, string operationNoun, string operationVerb)
		{
			return CheckBalance(database, balanceTable, GetBalanceDelta(database).Keys);
		}

		public static bool CheckBalance(Database database, IList<Balance> balanceTable, IEnumerable<long> products)
		{
			var wrongPositions = balanceTable.Where(p => products.Contains(p.ProductId) && p.Count < 0).ToList();
			if (wrongPositions.Count > 0)
			{
				var text = new StringBuilder("При выполнении данной операции остатки следующих товаров принимают отрицательные значения:");
				text.AppendLine();
				foreach (var position in wrongPositions)
				{
					var product = database.Products.First(p => p.ID == position.ProductId);
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
	}
}

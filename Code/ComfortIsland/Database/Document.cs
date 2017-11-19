using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ComfortIsland.Database
{
	[XmlType]
	public  class Document : IEntity, IEditable<Document>
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlAttribute]
		public string Number
		{ get; set; }

		[XmlAttribute]
		public DateTime Date
		{ get; set; }

		[XmlAttribute]
		public DocumentType Type
		{ get; set; }

		[XmlIgnore]
		public string TypeName
		{ get { return Type.ToStringRepresentation(); } }

		[XmlIgnore]
		public Dictionary<Product, long> Positions
		{ get; private set; }

		[XmlArray("Positions"), XmlArrayItem("Product")]
		public List<Position> PositionsToSerialize
		{ get; set; }

		#endregion

		public Document()
		{
			Positions = new Dictionary<Product, long>();
			PositionsToSerialize = new List<Position>();
		}

		public void Update(Document other)
		{
			this.ID = other.ID;
			this.Number = other.Number;
			this.Date = other.Date;
			this.Type = other.Type;
			this.Positions = new Dictionary<Product, long>(other.Positions);
			this.PositionsToSerialize = other.PositionsToSerialize.Select(p => new Position(p)).ToList();
		}

		public bool Validate(out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (string.IsNullOrEmpty(Number))
			{
				errors.AppendLine("Номер не может быть пустой строкой.");
			}
			if (PositionsToSerialize.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
			}
			var products = Database.Instance.Products;
			List<Position> positionsToCheck;
			if (Type == DocumentType.Income)
			{
				positionsToCheck = new List<Position>();
			}
			else if (Type == DocumentType.Outcome)
			{
				positionsToCheck = PositionsToSerialize;
			}
			else if (Type == DocumentType.Produce)
			{
				positionsToCheck = new List<Position>();
				foreach (var position in PositionsToSerialize)
				{
					var product = products.First(p => p.ID == position.ID);
					if (product.Children.Count == 0)
					{
						errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Товар {0} не может быть произведён, так как ни из чего не состоит.", product.DisplayMember));
					}
					foreach (var child in product.Children)
					{
						var existing = positionsToCheck.FirstOrDefault(p => p.ID == position.ID);
						if (existing != null)
						{
							existing.Count += (position.Count * child.Value);
						}
						else
						{
							positionsToCheck.Add(new Position(position.ID, position.Count * child.Value));
						}
					}
				}
			}
			else
			{
				throw new NotSupportedException();
			}
			var allBalance = Database.Instance.Balance;
			foreach (var position in positionsToCheck)
			{
				var balance = allBalance.FirstOrDefault(b => b.ProductId == position.ID);
				long count = balance != null ? balance.Count : 0;
				if (count < position.Count)
				{
					errors.AppendLine(string.Format(
						CultureInfo.InvariantCulture,
						InsufficientProductsCount,
						products.First(p => p.ID == position.ID).DisplayMember,
						count,
						position.Count));
				}
			}
			foreach (var position in PositionsToSerialize)
			{
				if (position.Count <= 0)
				{
					errors.AppendLine(CountMustBePositive);
				}
			}
			var ids = PositionsToSerialize.Select(p => p.ID).ToList();
			if (ids.Count > ids.Distinct().Count())
			{
				errors.Append("Дублирование позиций в документе");
			}
			return errors.Length == 0;
		}

		private const string CountMustBePositive = "Количество товара во всех позициях должно быть строго больше ноля.";
		private const string InsufficientProductsCount = "Недостаточно товара \"{0}\". Имеется по факту: {1}, требуется {2}.";

		#region [De]Serialization

		public void BeforeSerialization()
		{
			BeforeEdit();
		}

		public void AfterDeserialization()
		{
			AfterEdit();
		}

		#endregion

		public void BeforeEdit()
		{
			PositionsToSerialize = Positions.Select(kvp => new Position(kvp.Key.ID, kvp.Value)).ToList();
		}

		public void AfterEdit()
		{
			Positions.Clear();
			foreach (var position in PositionsToSerialize)
			{
				Positions[Database.Instance.Products.First(p => p.ID == position.ID)] = position.Count;
			}
		}
	}
}

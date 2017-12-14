using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

		public bool Validate(out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (PositionsToSerialize.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
			}
			DocumentTypeImplementation.AllTypes[Type].Validate(this, errors);
			foreach (var position in PositionsToSerialize)
			{
				if (Database.Instance.Products.FirstOrDefault(p => p.ID == position.ID) == null)
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
			return errors.Length == 0;
		}

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

		#region Workflow

		public void Validate(StringBuilder errors)
		{
			DocumentTypeImplementation.AllTypes[Type].Validate(this, errors);
		}

		public void Process(IList<Balance> balanceTable)
		{
			DocumentTypeImplementation.AllTypes[Type].Process(this, balanceTable);
		}

		public void ProcessBack(IList<Balance> balanceTable)
		{
			DocumentTypeImplementation.AllTypes[Type].ProcessBack(this, balanceTable);
		}

		public IDictionary<long, double> GetBalanceDelta()
		{
			return DocumentTypeImplementation.AllTypes[Type].GetBalanceDelta(this);
		}

		public bool CheckBalance(IList<Balance> balanceTable, out IList<long> wrongPositions)
		{
			wrongPositions = new List<long>();
			foreach (long productId in GetBalanceDelta().Keys)
			{
				if (balanceTable.First(b => b.ProductId == productId).Count < 0)
				{
					wrongPositions.Add(productId);
				}
			}
			return wrongPositions.Count == 0;
		}

		#endregion
	}
}

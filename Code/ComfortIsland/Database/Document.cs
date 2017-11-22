﻿using System;
using System.Collections.Generic;
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

		[XmlIgnore]
		public string TypeName
		{ get { return DocumentTypeImplementation.AllTypes[Type].Name; } }

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
			if (PositionsToSerialize.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
			}
			DocumentTypeImplementation.AllTypes[Type].Validate(this, errors);
			foreach (var position in PositionsToSerialize)
			{
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

		#endregion
	}
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ComfortIsland.Database
{
	[XmlType]
	public class Product : IEntity, IEditable<Product>, IListBoxItem
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlAttribute]
		public string Code
		{ get; set; }

		[XmlAttribute]
		public string Name
		{ get; set; }

		[XmlIgnore]
		public Unit Unit
		{ get; set; }

		[XmlAttribute]
		public long UnitID
		{ get; set; }

		[XmlIgnore]
		public string UnitName
		{ get { return Unit.Name; } }

		[XmlIgnore]
		public string DisplayMember
		{ get { return string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", Code, Name, UnitName); } }

		[XmlIgnore]
		public Dictionary<Product, double> Children
		{ get; private set; }

		[XmlArray("Children"), XmlArrayItem("Product")]
		public List<Position> ChildrenToSerialize
		{ get; set; }

		#endregion

		public Product()
		{
			Children = new Dictionary<Product, double>();
			ChildrenToSerialize = new List<Position>();
		}

		public void Update(Product other)
		{
			this.ID = other.ID;
			this.Name = other.Name;
			this.Code = other.Code;
			this.Unit = other.Unit;
			this.Children = new Dictionary<Product, double>(other.Children);
			this.ChildrenToSerialize = other.ChildrenToSerialize.Select(c => new Position(c)).ToList();
		}

		public bool Validate(Database database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (string.IsNullOrEmpty(Name))
			{
				errors.AppendLine("Наименование не может быть пустой строкой.");
			}
			if (Unit == null)
			{
				errors.AppendLine("Не выбрана единица измерения.");
			}
			var products = database.Products;
			if (ChildrenToSerialize.Any(c => products.First(p => p.ID == c.ID).IsOrHasChild(ID)))
			{
				errors.AppendLine("Товар не может быть частью себя или содержать другие товары, частью которых является.");
			}
			if (ChildrenToSerialize.Any(c => c.Count <= 0))
			{
				errors.AppendLine("Для каждого из вложенных товаров количество должно быть строго больше ноля.");
			}
			var ids = ChildrenToSerialize.Select(c => c.ID).ToList();
			if (ids.Count > ids.Distinct().Count())
			{
				errors.AppendLine("Некоторые товары включены как части несколько раз.");
			}
			return errors.Length == 0;
		}

		public bool IsOrHasChild(long id)
		{
			return ID == id || Children.Keys.Any(c => c.IsOrHasChild(id));
		}

		#region [De]Serialization

		public void BeforeSerialization()
		{
			UnitID = Unit.ID;
			BeforeEdit();
		}

		public void AfterDeserialization(Database database)
		{
			Unit = database.Units.First(u => u.ID == UnitID);
			AfterEdit(database);
		}

		#endregion

		public void BeforeEdit()
		{
			ChildrenToSerialize = Children.Select(kvp => new Position(kvp.Key.ID, kvp.Value)).ToList();
		}

		public void AfterEdit(Database database)
		{
			Children.Clear();
			foreach (var child in ChildrenToSerialize)
			{
				Children[database.Products.First(p => p.ID == child.ID)] = child.Count;
			}
		}
	}
}

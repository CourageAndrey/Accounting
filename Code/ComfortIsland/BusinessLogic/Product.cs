using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class Product : IEntity, IEditable<Product>, IListBoxItem
	{
		#region Properties

		public long ID
		{ get; set; }

		public string Name
		{ get; set; }

		public Unit Unit
		{ get; set; }

		public long UnitID
		{ get; set; }

		public string UnitName
		{ get { return Unit.Name; } }

		public string DisplayMember
		{ get { return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, UnitName); } }

		public Dictionary<Product, double> Children
		{ get; private set; }

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

		public void BeforeSerialization(Database database)
		{
			UnitID = Unit.ID;
			BeforeEdit(database);
		}

		public void AfterDeserialization(Database database)
		{
			Unit = database.Units.First(u => u.ID == UnitID);
			AfterEdit(database);
		}

		#endregion

		public void BeforeEdit(Database database)
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

		public StringBuilder FindUsages(Database database)
		{
#warning Implement
			throw new NotImplementedException();
		}
	}
}

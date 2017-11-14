using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.Database
{
	partial class Product : IEditable<Product>
	{
		public UnitEnum UnitEnum
		{
			get { return (UnitEnum) UnitID; }
			set { UnitID = (short) value; }
		}

		public string UnitName
		{ get; private set; }

		public string DisplayMember
		{ get { return string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", Code, Name, UnitName); } }

		public List<ProductChild> BindingChildren
		{ get; private set; }

		public Product()
		{
			BindingChildren = new List<ProductChild>();
		}

		public void Update(Product other)
		{
			this.ID = other.ID;
			this.Name = other.Name;
			this.Code = other.Code;
			this.UnitID = other.UnitID;
			this.BindingChildren = new List<ProductChild>(other.BindingChildren);
		}

		public bool Validate(ComfortIslandDatabase database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (string.IsNullOrEmpty(Name))
			{
				errors.AppendLine("Наименование не может быть пустой строкой.");
			}
			if (UnitID <= 0)
			{
				errors.AppendLine("Не выбрана единица измерения.");
			}
			if (BindingChildren.Any(c => c.IsOrHasChild(ID, database)))
			{
				errors.AppendLine("Товар не может быть частью себя или содержать другие товары, частью которых является.");
			}
			if (BindingChildren.Any(c => c.Count <= 0))
			{
				errors.AppendLine("Для каждого из вложенных товаров количество должно быть строго больше ноля.");
			}
			return errors.Length == 0;
		}

		public Product PrepareToDisplay(ComfortIslandDatabase database)
		{
			UnitName = Unit.ShortName;
			if (!Children.IsLoaded)
			{
				Children.Load(MergeOption.NoTracking);
			}
			BindingChildren = Children.Select(c => new ProductChild(c)).ToList();
			return this;
		}

		public void PrepareToSave(ComfortIslandDatabase database)
		{
			var oldParts = database.IsPartOf.Where(p => p.ParentID == ID).ToList();
			foreach (var part in oldParts)
			{
				if (BindingChildren.All(c => c.Id != part.ChildID))
				{
					database.DeleteObject(part);
				}
			}
			foreach (var child in BindingChildren)
			{
				if (oldParts.All(p => p.ChildID != child.Id))
				{
					database.AddToIsPartOf(new IsPartOf
					{
						ParentID = ID,
						ChildID = child.Id,
						Count = child.Count,
					});
				}
			}
		}
	}

	public class ProductChild
	{
		public long Id
		{ get; set; }

		public long Count
		{ get; set; }

		public ProductChild()
		{ }

		public ProductChild(IsPartOf child)
		{
			Id = child.ChildID;
			Count = child.Count;
		}

		public bool IsOrHasChild(long id, ComfortIslandDatabase database)
		{
			return Id == id || database.IsPartOf.Execute(MergeOption.NoTracking).Where(p => p.ParentID == Id).Select(p => new ProductChild(p)).Any(c => c.IsOrHasChild(id, database));
		}
	}
}

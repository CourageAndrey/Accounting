using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class Product : IEntity, IListItem
	{
		#region Properties

		public long ID
		{ get; set; }

		public string Name
		{
			get { return _name; }
			set
			{
				var errors = new StringBuilder();
				if (NameIsNotNullOrEmpty(value, errors))
				{
					_name = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		public Unit Unit
		{
			get { return _unit; }
			set
			{
				var errors = new StringBuilder();
				if (UnitIsNotNull(value, errors))
				{
					_unit = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		public string DisplayMember
		{ get { return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, Unit.Name); } }

		public Dictionary<Product, decimal> Children
		{
			get { return _children; }
			set
			{
				var errors = new StringBuilder();
				var positionsToCheck = value.Select(kvp => new Position(kvp.Key.ID, kvp.Value)).ToList();
				bool isValid = Position.PositionsDoNotDuplicate(positionsToCheck, "составляющие части", errors);
				for (int line = 0; line < positionsToCheck.Count; line++)
				{
					isValid &= Position.ProductIsSet(positionsToCheck[line].ID, line + 1, errors);
					isValid &= Position.CountIsPositive(positionsToCheck[line].Count, line + 1, errors);
				}
				if (isValid & ChildrenAreNotRecursive(ID, value.Keys, errors))
				{
					_children = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		private string _name;
		private Unit _unit;
		private Dictionary<Product, decimal> _children;

		#endregion

		public Product()
		{
			Children = new Dictionary<Product, decimal>();
		}

		#region Валидация

		public static bool NameIsNotNullOrEmpty(string name, StringBuilder errors)
		{
			if (string.IsNullOrEmpty(name))
			{
				errors.AppendLine("Наименование не может быть пустой строкой.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool UnitIsNotNull(Unit unit, StringBuilder errors)
		{
			if (unit == null)
			{
				errors.AppendLine("Не выбрана единица измерения.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool ChildrenAreNotRecursive(long? id, IEnumerable<Product> children, StringBuilder errors)
		{
			if (id.HasValue && children.Any(c => c.IsOrHasChild(id.Value)))
			{
				errors.AppendLine("Товар не может быть частью себя или содержать другие товары, частью которых является.");
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		public bool IsOrHasChild(long id)
		{
			return ID == id || Children.Keys.Any(c => c.IsOrHasChild(id));
		}
	}
}

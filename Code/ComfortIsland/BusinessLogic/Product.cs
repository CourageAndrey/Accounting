using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class Product : IEntity, IListBoxItem
	{
		#region Properties

		public long ID
		{ get; set; }

		public string Name
		{
			get { return name; }
			set
			{
				var errors = new StringBuilder();
				if (NameIsNotNullOrEmpty(value, errors))
				{
					name = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		public Unit Unit
		{
			get { return unit; }
			set
			{
				var errors = new StringBuilder();
				if (UnitIsNotNull(value, errors))
				{
					unit = value;
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
			get { return children; }
			set
			{
				var errors = new StringBuilder();
				if (ChildrenAreNotRecursive(ID, value, errors) &
					ChildrenCountsArePositive(value, errors) &
					ChildrenDoNotDuplicate(value, errors))
				{
					children = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		private string name;
		private Unit unit;
		private Dictionary<Product, decimal> children;

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

		public static bool ChildrenAreNotRecursive(long? id, Dictionary<Product, decimal> children, StringBuilder errors)
		{
			if (id.HasValue && children.Keys.Any(c => c.IsOrHasChild(id.Value)))
			{
				errors.AppendLine("Товар не может быть частью себя или содержать другие товары, частью которых является.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool ChildrenCountsArePositive(Dictionary<Product, decimal> children, StringBuilder errors)
		{
			if (children.Any(c => c.Value <= 0))
			{
				errors.AppendLine("Для каждого из вложенных товаров количество должно быть строго больше ноля.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool ChildrenDoNotDuplicate(Dictionary<Product, decimal> children, StringBuilder errors)
		{
			var ids = children.Select(c => c.Key.ID).ToList();
			if (ids.Count > ids.Distinct().Count())
			{
				errors.AppendLine("Некоторые товары включены как части несколько раз.");
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

		public StringBuilder FindUsages(Database database)
		{
			var message = new StringBuilder();
			var documents = database.Documents.Where(d => d.Type.GetBalanceDelta(d.Positions).ContainsKey(ID)).ToList();
			var parentProducts = database.Products.Where(p => p.Children.ContainsKey(this)).ToList();
			if (documents.Count > 0)
			{
				message.AppendLine("Данный товар используется в следующих документах:");
				message.AppendLine();
				foreach (var document in documents)
				{
					message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0} {1} от {2}",
						document.Type.Name,
						!string.IsNullOrEmpty(document.Number) ? "\"" + document.Number + "\"" : string.Empty,
						document.Date.ToShortDateString()));
				}
				message.AppendLine();
			}
			if (parentProducts.Count > 0)
			{
				message.AppendLine("Данный товар используется как составная часть в следующих товарах:");
				message.AppendLine();
				foreach (var parent in parentProducts)
				{
					message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0}", parent.DisplayMember));
				}
				message.AppendLine();
			}
			return message;
		}
	}
}

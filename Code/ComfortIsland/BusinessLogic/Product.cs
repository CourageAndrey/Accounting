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
		{ get; set; }

		public Unit Unit
		{ get; set; }

		public string DisplayMember
		{ get { return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, Unit.Name); } }

		public Dictionary<Product, double> Children
		{ get; set; }

		#endregion

		public Product()
		{
			Children = new Dictionary<Product, double>();
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
			if (Children.Keys.Any(c => c.IsOrHasChild(ID)))
			{
				errors.AppendLine("Товар не может быть частью себя или содержать другие товары, частью которых является.");
			}
			if (Children.Any(c => c.Value <= 0))
			{
				errors.AppendLine("Для каждого из вложенных товаров количество должно быть строго больше ноля.");
			}
			var ids = Children.Select(c => c.Key.ID).ToList();
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

		public StringBuilder FindUsages(Database database)
		{
			var message = new StringBuilder();
			var documents = database.Documents.Where(d => d.Type.GetBalanceDelta(database, d).ContainsKey(ID)).ToList();
			var parentProducts = database.Products.Where(p => p.Children.ContainsKey(this)).ToList();
			if (documents.Count > 0)
			{
				message.AppendLine("Данный товар используется в следующих документах:");
				message.AppendLine();
				foreach (var document in documents)
				{
					message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0} {1} от {2}",
						document.TypeName,
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

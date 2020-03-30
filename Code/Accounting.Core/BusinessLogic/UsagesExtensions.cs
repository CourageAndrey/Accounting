using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Accounting.Core.BusinessLogic
{
	public static class UsagesExtensions
	{
		public static StringBuilder FindUsages(this IEntity entity, IDatabase database)
		{
			FindUsagesDelegate usagesFinder;
			if (possibleUsages.TryGetValue(entity.GetType(), out usagesFinder))
			{
				return usagesFinder(entity, database);
			}
			else
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.InvariantCulture,
					"Класс {0} не поддерживает поиск использования.",
					entity.GetType()));
			}
		}

		private delegate StringBuilder FindUsagesDelegate(IEntity entity, IDatabase database);

		private static IDictionary<Type, FindUsagesDelegate> possibleUsages = new Dictionary<Type, FindUsagesDelegate>
		{
			{ typeof(Unit), findUsagesOfUnit },
			{ typeof(Product), findUsagesOfProduct },
			{ typeof(Document), findUsagesOfDocument },
		};

		private static StringBuilder findUsagesOfUnit(IEntity entity, IDatabase database)
		{
			var unit = entity as Unit;
			if (unit == null) throw new ArgumentException("\"entity\" не передано или не является Единицей измерения.");

			var message = new StringBuilder();
			var products = database.Products.Where(p => p.Unit == unit).ToList();
			if (products.Count > 0)
			{
				message.AppendLine(string.Format(CultureInfo.InvariantCulture, "Следующие товары имеют единицу измерения \"{0}\":", unit.Name));
				message.AppendLine();
				foreach (var product in products)
				{
					message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0}", product.Name));
				}
			}
			return message;
		}

		private static StringBuilder findUsagesOfProduct(IEntity entity, IDatabase database)
		{
			var product = entity as Product;
			if (product == null) throw new ArgumentException("\"entity\" не передано или не является Товаром.");

			var message = new StringBuilder();
			var documents = database.Documents.Where(d => d.Type.GetBalanceDelta(d.Positions).ContainsKey(product.ID)).ToList();
			var parentProducts = database.Products.Where(p => p.Children.ContainsKey(product)).ToList();

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

		private static StringBuilder findUsagesOfDocument(IEntity entity, IDatabase database)
		{
			return new StringBuilder();
		}
	}
}

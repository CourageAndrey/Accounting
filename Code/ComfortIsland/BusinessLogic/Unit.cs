using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class Unit : IEntity
	{
		#region Properties

		public long ID
		{ get; set; }

		public string Name
		{ get; set; }

		public string ShortName
		{ get; set; }

		#endregion

		public bool Validate(Database database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (string.IsNullOrEmpty(Name))
			{
				errors.AppendLine("Наименование не может быть пустой строкой.");
			}
			if (string.IsNullOrEmpty(ShortName))
			{
				errors.AppendLine("Сокращение не может быть пустой строкой.");
			}
			return errors.Length == 0;
		}

		public StringBuilder FindUsages(Database database)
		{
			var message = new StringBuilder();
			var products = database.Products.Where(p => p.Unit == this).ToList();
			if (products.Count > 0)
			{
				message.AppendLine(string.Format(CultureInfo.InvariantCulture, "Следующие товары имеют единицу измерения \"{0}\":", Name));
				message.AppendLine();
				foreach (var product in products)
				{
					message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0}", product.Name));
				}
			}
			return message;
		}
	}
}

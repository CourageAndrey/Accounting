using System.Collections.Generic;

namespace Accounting.Core.BusinessLogic
{
	public class Database
	{
		#region Properties

		public Registry<Unit> Units
		{ get; }

		public Registry<Product> Products
		{ get; }

		public Warehouse Balance
		{ get; }

		public Registry<Document> Documents
		{ get; }

		#endregion

		public Database(
			IEnumerable<Unit> units,
			IEnumerable<Product> products,
			IDictionary<long, decimal> balance,
			IEnumerable<Document> documents)
		{
			Units = new Registry<Unit>(units);
			Products = new Registry<Product>(products);
			Balance = new Warehouse(balance);
			Documents = new Registry<Document>(documents);
		}

		public static Database CreateDefault()
		{
			return new Database(
				new[]
				{
					new Unit { ID = 1, Name = "штука", ShortName = "шт" },
					new Unit { ID = 2, Name = "метр погонный", ShortName = "м/пог" },
				},
				new Product[0],
				new Dictionary<long, decimal>(),
				new Document[0]);
		}
	}
}

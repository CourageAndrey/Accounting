using System;
using System.Collections.Generic;

namespace Accounting.Core.BusinessLogic
{
	public class Database : IDatabase
	{
		#region Properties

		public IRegistry<Unit> Units
		{ get; }

		public IRegistry<Product> Products
		{ get; }

		public IWarehouse Balance
		{ get; }

		public IRegistry<Document> Documents
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

		public static IDatabase CreateDefault()
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

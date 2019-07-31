using System.Collections.Generic;

namespace ComfortIsland.BusinessLogic
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
	}
}

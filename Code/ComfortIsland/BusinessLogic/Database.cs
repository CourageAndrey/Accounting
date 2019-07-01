using System.Collections.Generic;

namespace ComfortIsland.BusinessLogic
{
	public class Database
	{
		#region Properties

		public Warehouse<Unit> Units
		{ get; }

		public Warehouse<Product> Products
		{ get; }

		public Storage Balance
		{ get; }

		public Warehouse<Document> Documents
		{ get; }

		#endregion

		#region Constructors

		public Database()
			: this(
				new Warehouse<Unit>(),
				new Warehouse<Product>(),
				new Dictionary<long, double>(),
				new Warehouse<Document>())
		{ }

		public Database(
			IEnumerable<Unit> units,
			IEnumerable<Product> products,
			IDictionary<long, double> balance,
			IEnumerable<Document> documents)
		{
			Units = new Warehouse<Unit>(units);
			Products = new Warehouse<Product>(products);
			Balance = new Storage(balance);
			Documents = new Warehouse<Document>(documents);
		}

		#endregion
	}
}

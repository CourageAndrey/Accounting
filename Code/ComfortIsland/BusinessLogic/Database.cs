using System.Collections.Generic;
using System.Linq;

namespace ComfortIsland.BusinessLogic
{
	public class Database
	{
		#region Properties

		public Registry<Unit> Units
		{ get; }

		public Registry<Product> Products
		{ get; }

		public Storage Balance
		{ get; }

		public Registry<Document> Documents
		{ get; }

		#endregion

		#region Constructors

		public Database()
			: this(
				new Registry<Unit>(),
				new Registry<Product>(),
				new Dictionary<long, decimal>(),
				new Registry<Document>())
		{ }

		public Database(
			IEnumerable<Unit> units,
			IEnumerable<Product> products,
			IDictionary<long, decimal> balance,
			IEnumerable<Document> documents)
		{
			Units = new Registry<Unit>(units);
			Products = new Registry<Product>(products);
			Balance = new Storage(balance);
			Documents = new Registry<Document>(documents);
		}

		public Database CreateMockup()
		{
			return new Database(
				new Unit[0],
				new Product[0],
				Balance.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
				new Document[0]);
		}

		#endregion
	}
}

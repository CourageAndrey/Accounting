using System.Collections.Generic;

namespace ComfortIsland.BusinessLogic
{
	public class Database
	{
		#region Properties

		public IDictionary<long, Unit> Units
		{ get; }

		public IDictionary<long, Product> Products
		{ get; }

		public IDictionary<long, double> Balance
		{ get; }

		public IDictionary<long, Document> Documents
		{ get; }

		#endregion

		#region Constructors

		public Database()
			: this(
				new Dictionary<long, Unit>(),
				new Dictionary<long, Product>(),
				new Dictionary<long, double>(),
				new Dictionary<long, Document>())
		{ }

		public Database(
			IDictionary<long, Unit> units,
			IDictionary<long, Product> products,
			IDictionary<long, double> balance,
			IDictionary<long, Document> documents)
		{
			Units = units;
			Products = products;
			Balance = balance;
			Documents = documents;
		}

		#endregion
	}
}

using System.Collections.Generic;

namespace ComfortIsland.BusinessLogic
{
	public class Database
	{
		#region Properties

		public List<Document> Documents
		{ get; set; }

		public List<Position> Balance
		{ get; set; }

		public List<Product> Products
		{ get; set; }

		public List<Unit> Units
		{ get; set; }

		#endregion

		public Database()
		{
			Documents = new List<Document>();
			Balance = new List<Position>();
			Products = new List<Product>();
			Units = new List<Unit>();
		}
	}
}

using System.Collections.Generic;

namespace ComfortIsland.BusinessLogic
{
	public class Database
	{
		#region Properties

		public List<Document> Documents
		{ get; set; }

		public List<Balance> Balance
		{ get; set; }

		public List<Product> Products
		{ get; set; }

		public List<Unit> Units
		{ get; set; }

		#endregion

		public Database()
		{
			Documents = new List<Document>();
			Balance = new List<Balance>();
			Products = new List<Product>();
			Units = new List<Unit>();
		}
	}
}

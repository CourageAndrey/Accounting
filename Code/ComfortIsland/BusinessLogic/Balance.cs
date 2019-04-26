using System.Linq;

namespace ComfortIsland.BusinessLogic
{
	public class Balance
	{
		#region Properties

		public long ProductId
		{ get; set; }

		public double Count
		{ get; set; }

		public string ProductCode
		{ get; private set; }

		public string ProductName
		{ get; private set; }

		public string ProductUnit
		{ get; private set; }

		#endregion

		#region Constructors

		public Balance()
		{ }

		public Balance(Balance other)
		{
			ProductId = other.ProductId;
			Count = other.Count;
			ProductCode = other.ProductCode;
			ProductName = other.ProductName;
			ProductUnit = other.ProductUnit;
		}

		public Balance(Product product, double count)
		{
			ProductId = product.ID;
			Count = count;
			initializeProduct(product);
		}

		public Balance(Database database, long productId, double count)
			: this(database.Products.First(p => p.ID == productId), count)
		{ }

		#endregion

		private void initializeProduct(Product product)
		{
			ProductCode = product.Code;
			ProductName = product.Name;
			ProductUnit = product.Unit.Name;
		}

		public void AfterDeserialization(Database database)
		{
			initializeProduct(database.Products.First(p => p.ID == ProductId));
		}
	}
}

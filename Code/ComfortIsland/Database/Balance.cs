using System.Linq;
using System.Xml.Serialization;

namespace ComfortIsland.Database
{
	[XmlType]
	public class Balance
	{
		#region Properties

		[XmlAttribute("Product")]
		public long ProductId
		{ get; set; }

		[XmlAttribute]
		public double Count
		{ get; set; }

		[XmlIgnore]
		public string ProductCode
		{ get; private set; }

		[XmlIgnore]
		public string ProductName
		{ get; private set; }

		[XmlIgnore]
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

		public Balance(long productId, double count)
			: this(Database.Instance.Products.First(p => p.ID == productId), count)
		{ }

		#endregion

		private void initializeProduct(Product product)
		{
			ProductCode = product.Code;
			ProductName = product.Name;
			ProductUnit = product.Unit.Name;
		}

		public void AfterDeserialization()
		{
			initializeProduct(Database.Instance.Products.First(p => p.ID == ProductId));
		}
	}
}

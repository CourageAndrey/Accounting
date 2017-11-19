using System.Linq;
using System.Runtime.Serialization;
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
		public long Count
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

		public Balance(Product product, long count)
		{
			ProductId = product.ID;
			Count = count;
			initializeProduct(product);
		}

		#endregion

		private void initializeProduct(Product product)
		{
			ProductCode = product.Code;
			ProductName = product.Name;
			ProductUnit = product.Unit.Name;
		}

		#region [De]Serialization

		[OnSerialized]
		private void afterDeserialization(StreamingContext context)
		{
			
		}

		#endregion

		public void AfterDeserialization()
		{
			initializeProduct(Database.Instance.Products.First(p => p.ID == ProductId));
		}
	}
}

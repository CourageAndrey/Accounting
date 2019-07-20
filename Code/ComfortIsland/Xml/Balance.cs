using System.Xml.Serialization;

namespace ComfortIsland.Xml
{
	[XmlType]
	public class Balance
	{
		#region Properties

		[XmlAttribute]
		public long Product
		{ get; set; }

		[XmlAttribute]
		public decimal Count
		{ get; set; }

		#endregion

		#region Constructors

		public Balance()
		{ }

		public Balance(long product, decimal count)
		{
			Product = product;
			Count = count;
		}

		#endregion

		public BusinessLogic.Position ConvertToBusinessLogic()
		{
			return new BusinessLogic.Position
			{
				ID = Product,
				Count = Count,
			};
		}
	}
}

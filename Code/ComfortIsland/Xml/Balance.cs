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
		public double Count
		{ get; set; }

		#endregion

		#region Constructors

		public Balance()
		{ }

		public Balance(BusinessLogic.Balance balance)
		{
			Product = balance.ProductId;
			Count = balance.Count;
		}

		#endregion

		public BusinessLogic.Balance ConvertToBusinessLogic()
		{
			return new BusinessLogic.Balance
			{
				ProductId = Product,
				Count = Count,
			};
		}
	}
}

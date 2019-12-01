using System.Xml.Serialization;

namespace ComfortIsland.Configuration.Xml
{
	[XmlType]
	public class BusinessLogicSettings
	{
		[XmlElement]
		public string BalanceValidationStrategy
		{ get; set; }
	}
}

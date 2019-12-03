using System.Xml.Serialization;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public class BusinessLogicSettings
	{
		[XmlElement]
		public string BalanceValidationStrategy
		{ get; set; }
	}
}

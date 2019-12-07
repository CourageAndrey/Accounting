using System.Xml.Serialization;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public class UserInterfaceSettings
	{
		[XmlElement]
		public double FontSize
		{ get; set; }

		public UserInterfaceDriver UserInterfaceDriver
		{ get; set; }
	}
}

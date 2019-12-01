using System.Xml.Serialization;

namespace ComfortIsland.Configuration.Xml
{
	[XmlType]
	public class UserInterfaceSettings
	{
		[XmlElement]
		public double FontSize
		{ get; set; }
	}
}

using System.Xml.Serialization;

namespace ComfortIsland.Configuration.Xml
{
	[XmlType]
	public class DataAccessLayerSettings
	{
		public DatabaseDriver DatabaseDriver
		{ get; set; }
	}
}

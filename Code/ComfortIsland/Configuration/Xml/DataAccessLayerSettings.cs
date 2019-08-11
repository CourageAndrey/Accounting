using System.Xml.Serialization;

using ComfortIsland.Configuration.Xml.DatabaseDrivers;

namespace ComfortIsland.Configuration.Xml
{
	[XmlType]
	public class DataAccessLayerSettings
	{
		[XmlElement("XmlDatabaseDriver", typeof(XmlDatabaseDriver))]
		public DatabaseDriver DatabaseDriver
		{ get; set; }
	}
}

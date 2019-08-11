using System.Xml.Serialization;

using ComfortIsland.Configuration.Xml.DatabaseDrivers;

namespace ComfortIsland.Configuration.Xml
{
	[XmlType]
	[XmlInclude(typeof(XmlDatabaseDriver))]
	public abstract class DatabaseDriver
	{
		public abstract DataAccessLayer.IDatabaseDriver CreateDataAccessLayer();
	}
}

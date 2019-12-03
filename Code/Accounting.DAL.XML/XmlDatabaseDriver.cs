using System.Xml.Serialization;

using Accounting.Core.Configuration.Xml;

namespace ComfortIsland.Configuration.Xml.DatabaseDrivers
{
	[XmlType]
	public class XmlDatabaseDriver : DatabaseDriver
	{
		[XmlAttribute]
		public string FileName
		{ get; set; }

		public override DataAccessLayer.IDatabaseDriver CreateDataAccessLayer()
		{
			return new DataAccessLayer.Xml.DatabaseDriver(FileName);
		}
	}
}

using System.Xml.Serialization;

using Accounting.Core.Configuration.Xml;
using Accounting.Core.DataAccessLayer;

namespace ComfortIsland.Configuration.Xml.DatabaseDrivers
{
	[XmlType]
	public class XmlDatabaseDriver : DatabaseDriver
	{
		[XmlAttribute]
		public string FileName
		{ get; set; }

		public override IDatabaseDriver CreateDataAccessLayer()
		{
			return new DataAccessLayer.Xml.DatabaseDriver(FileName);
		}
	}
}

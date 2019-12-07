using System.Xml.Serialization;

using Accounting.Core.Application;

namespace Accounting.DAL.XML
{
	[XmlType]
	public class XmlDatabaseDriver : Accounting.Core.Configuration.Xml.DatabaseDriver
	{
		[XmlAttribute]
		public string FileName
		{ get; set; }

		public override IDatabaseDriver CreateDataAccessLayer()
		{
			return new DatabaseDriver(FileName);
		}
	}
}

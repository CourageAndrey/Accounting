using System.Xml.Serialization;

using Accounting.Core.Application;

namespace Accounting.DAL.EntityFramework
{
	[XmlType]
	public class EntityFrameworkDatabaseDriver : Accounting.Core.Configuration.Xml.DatabaseDriver
	{
		[XmlAttribute]
		public string ConnectionString
		{ get; set; }

		public override IDatabaseDriver CreateDataAccessLayer()
		{
			return new DatabaseDriver(ConnectionString);
		}
	}
}

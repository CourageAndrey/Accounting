using System.Xml.Serialization;

using Accounting.Core.Application;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public abstract class DatabaseDriver
	{
		public abstract IDatabaseDriver CreateDataAccessLayer();
	}
}

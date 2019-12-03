using System.Xml.Serialization;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public class DataAccessLayerSettings
	{
		public DatabaseDriver DatabaseDriver
		{ get; set; }
	}
}

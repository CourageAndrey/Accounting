using System.Xml.Serialization;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public class ReportingSettings
	{
		public ReportExportDriver ReportExportDriver
		{ get; set; }
	}
}

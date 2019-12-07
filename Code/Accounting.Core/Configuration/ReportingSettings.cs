using System.Xml.Serialization;

using Accounting.Core.Application;

namespace Accounting.Core.Configuration
{
	[XmlType]
	public class ReportingSettings
	{
		public IReportExporter Exporter
		{ get; }

		internal ReportingSettings(Xml.ReportingSettings xmlSettings)
		{
			Exporter = xmlSettings.ReportExportDriver.CreateReportExporter();
		}
	}
}

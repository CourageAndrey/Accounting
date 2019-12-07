using System.Xml.Serialization;

using Accounting.Core.Application;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public abstract class ReportExportDriver
	{
		public abstract IReportExporter CreateReportExporter();
	}
}

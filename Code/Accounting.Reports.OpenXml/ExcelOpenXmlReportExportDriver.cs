using Accounting.Core.Application;
using Accounting.Core.Configuration.Xml;

namespace Accounting.Reports.OpenXml
{
	public class ExcelOpenXmlReportExportDriver : ReportExportDriver
	{
		public override IReportExporter CreateReportExporter()
		{
			return new ExcelOpenXmlReportExporter();
		}
	}
}

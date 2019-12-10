using Accounting.Core.Application;
using Accounting.Core.Configuration.Xml;

namespace Accounting.Reports.OpenXml
{
	internal class ExcelOpenXmlPlugin : IAccountingPlugin
	{
		public void Setup(IAccountingApplication application)
		{
			InternalEnginesExtensions.RegisterReportExportEngine<ExcelOpenXmlReportExportDriver>("ExcelOpenXmlReportExportDriver");
		}
	}
}

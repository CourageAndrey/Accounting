using System.Collections.Generic;

using Accounting.Core.Application;
using Accounting.Core.Reports.Existing;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.Reports.Descriptors
{
	public class FinanceReportDescriptor : ReportDescriptor
	{
		public override string Title
		{ get { return "Финансовый отчёт";  } }

		private IEnumerable<ReportColumn> columns;

		public override IEnumerable<ReportColumn> GetColumns()
		{
			return columns ?? (columns = new List<ReportColumn>
			{
				new ReportColumn
				(
					"Графа",
					"Item",
					false,
					300
				),
				new ReportColumn
				(
					"Сумма",
					"Value",
					true,
					100
				),
			});
		}

		public override bool CreateReport(IAccountingApplication application, out IReport report)
		{
			var selectedPeriod = new PeriodParams();
			if (application.UserInterface.SelectPeriod(application, ref selectedPeriod))
			{
				report = new FinanceReport(application.Database, selectedPeriod);
				return true;
			}
			else
			{
				report = null;
				return false;
			}
		}
	}
}

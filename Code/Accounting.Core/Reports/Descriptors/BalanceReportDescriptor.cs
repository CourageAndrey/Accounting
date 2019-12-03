using System.Collections.Generic;

using Accounting.Core.Application;
using Accounting.Core.Reports.Existing;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.Reports.Descriptors
{
	public class BalanceReportDescriptor : ReportDescriptor
	{
		public override string Title
		{ get { return "Складские остатки"; } }

		private IEnumerable<ReportColumn> columns;

		public override IEnumerable<ReportColumn> GetColumns()
		{
			return columns ?? (columns = new List<ReportColumn>
			{
				new ReportColumn
				(
					"Товар",
					"BoundProduct.Name",
					false,
					300
				),
				new ReportColumn
				(
					"Ед/изм",
					"BoundProduct.Unit.Name",
					false,
					80
				),
				new ReportColumn
				(
					"Остатки",
					"Count",
					true,
					100
				),
			});
		}

		public override bool CreateReport(IAccountingApplication application, out IReport report)
		{
			var selectedDate = new BalanceReportParams();
			if (application.UserInterface.SelectDate(application, ref selectedDate))
			{
				report = new BalanceReport(application.Database, selectedDate);
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

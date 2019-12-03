using System.Collections.Generic;

using Accounting.Core.Application;
using Accounting.Core.Reports.Existing;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.Reports.Descriptors
{
	public class TradeReportDescriptor : ReportDescriptor
	{
		public override string Title
		{ get { return "Товарный отчёт"; } }

		private IEnumerable<ReportColumn> columns;

		public override IEnumerable<ReportColumn> GetColumns()
		{
			return columns ?? (columns = new List<ReportColumn>
			{
				new ReportColumn
				(
					"Товар",
					"ProductName",
					false,
					300
				),
				new ReportColumn
				(
					"Ед/изм",
					"ProductUnit",
					false,
					80
				),
				new ReportColumn
				(
					"На начало периода",
					"InitialBalance",
					true,
					100
				),
				new ReportColumn
				(
					"Приобретено",
					"Income",
					true,
					100
				),
				new ReportColumn
				(
					"Произведено",
					"Produced",
					true,
					100
				),
				new ReportColumn
				(
					"Продано",
					"Selled",
					true,
					100
				),
				new ReportColumn
				(
					"Израсходовано",
					"UsedToProduce",
					true,
					100
				),
				new ReportColumn
				(
					"Отправлено на склад",
					"SentToWarehouse",
					true,
					100
				),
				new ReportColumn
				(
					"На конец периода",
					"FinalBalance",
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
				report = new TradeReport(application.Database, selectedPeriod);
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

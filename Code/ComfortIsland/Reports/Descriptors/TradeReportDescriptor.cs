using System;
using System.Collections.Generic;

using ComfortIsland.Dialogs;
using ComfortIsland.Helpers;
using ComfortIsland.Reports.Params;

namespace ComfortIsland.Reports
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
					50
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
			var dialog = new SelectPeriodDialog { EditValue = new PeriodParams() };
			dialog.ConnectTo(application);
			if (dialog.ShowDialog() == true)
			{
				report = new TradeReport(application.Database, dialog.EditValue);
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

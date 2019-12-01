using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Dialogs;
using ComfortIsland.Helpers;

namespace ComfortIsland.Reports
{
	public class ReportDescriptor
	{
		#region Properties

		public string Title
		{ get; private set; }

		private readonly Func<IEnumerable<ReportColumn>> _columnsGetter;
		private delegate bool ReportCreator(IAccountingApplication application, out IReport report);
		private readonly ReportCreator _reportCreator;

		#endregion

		private ReportDescriptor(string title, Func<IEnumerable<ReportColumn>> columnsGetter, ReportCreator reportCreator)
		{
			Title = title;
			_columnsGetter = columnsGetter;
			_reportCreator = reportCreator;
		}

		public IEnumerable<ReportColumn> GetColumns()
		{
			return _columnsGetter();
		}

		public bool CreateReport(IAccountingApplication application, Database database, out IReport report)
		{
			return _reportCreator(application, out report);
		}

		#region Список

		public static readonly ReportDescriptor Balance = new ReportDescriptor("Складские остатки", () => new List<ReportColumn>
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
				50
			),
			new ReportColumn
			(
				"Остатки",
				"Count",
				true,
				100
			),
		}, createBalanceReport);

		public static readonly ReportDescriptor Trade = new ReportDescriptor("Товарный отчёт", () => new List<ReportColumn>
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
		}, createTradeReport);


		public static readonly IEnumerable<ReportDescriptor> All = new ReadOnlyCollection<ReportDescriptor>(new List<ReportDescriptor>
		{
			Balance,
			Trade,
		});

		private static bool createBalanceReport(IAccountingApplication application, out IReport report)
		{
			var dialog = new SelectDateDialog { EditValue = DateTime.Now };
			dialog.ConnectTo(application);
			if (dialog.ShowDialog() == true)
			{
				report = new BalanceReport(application.Database, dialog.EditValue, dialog.IncludeAllProducts);
				return true;
			}
			else
			{
				report = null;
				return false;
			}
		}

		private static bool createTradeReport(IAccountingApplication application, out IReport report)
		{
			var dialog = new SelectPeriodDialog { EditValue = new Period(DateTime.Now.AddDays(-7), DateTime.Now) };
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

		#endregion
	}
}

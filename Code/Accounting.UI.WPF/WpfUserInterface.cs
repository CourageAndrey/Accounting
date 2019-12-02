using ComfortIsland.Dialogs;
using ComfortIsland.Reports.Params;

using Accounting.Core.Application;

namespace ComfortIsland
{
	public class WpfUserInterface : IUserInterface
	{
		public bool SelectDate(IAccountingApplication application, ref BalanceReportParams selectedValue)
		{
			var dialog = new SelectDateDialog { EditValue = selectedValue };
			dialog.ConnectTo(application);
			if (dialog.ShowDialog() == true)
			{
				selectedValue = dialog.EditValue;
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool SelectPeriod(IAccountingApplication application, ref PeriodParams selectedValue)
		{
			var dialog = new SelectPeriodDialog { EditValue = selectedValue };
			dialog.ConnectTo(application);
			if (dialog.ShowDialog() == true)
			{
				selectedValue = dialog.EditValue;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

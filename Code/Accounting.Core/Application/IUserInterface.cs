using ComfortIsland.Reports.Params;

namespace Accounting.Core.Application
{
	public interface IUserInterface
	{
		bool SelectDate(IAccountingApplication application, ref BalanceReportParams selectedValue);

		bool SelectPeriod(IAccountingApplication application, ref PeriodParams selectedValue);
	}
}

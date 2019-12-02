using ComfortIsland.Reports.Params;

namespace ComfortIsland
{
	public interface IUserInterface
	{
		bool SelectDate(IAccountingApplication application, ref BalanceReportParams selectedValue);

		bool SelectPeriod(IAccountingApplication application, ref PeriodParams selectedValue);
	}
}

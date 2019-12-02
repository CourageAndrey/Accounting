namespace Accounting.Core.Application
{
	public interface IAccountingApplicationClient
	{
		void ConnectTo(IAccountingApplication application);
	}
}
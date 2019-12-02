using ComfortIsland.BusinessLogic;

namespace Accounting.Core.Application
{
	public interface IAccountingApplication
	{
		string StartupPath { get; }

		ComfortIsland.Configuration.Settings Settings { get; }

		Database Database { get; }

		IUserInterface UserInterface { get; }

		IReportExporter ReportExporter { get; }
	}
}
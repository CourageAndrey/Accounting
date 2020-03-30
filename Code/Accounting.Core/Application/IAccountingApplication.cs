using Accounting.Core.BusinessLogic;
using Accounting.Core.Configuration;

namespace Accounting.Core.Application
{
	public interface IAccountingApplication
	{
		string StartupPath { get; }

		Settings Settings { get; }

		IDatabaseDriver DatabaseDriver { get; }

		IUserInterface UserInterface { get; }

		IReportExporter ReportExporter { get; }

		IDatabase Database { get; }
	}
}
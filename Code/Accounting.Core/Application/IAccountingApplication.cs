using Accounting.Core.BusinessLogic;
using Accounting.Core.Configuration;

namespace Accounting.Core.Application
{
	public interface IAccountingApplication
	{
		string StartupPath { get; }

		Settings Settings { get; }

		Database Database { get; }

		IUserInterface UserInterface { get; }

		IReportExporter ReportExporter { get; }
	}
}
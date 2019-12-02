using ComfortIsland.BusinessLogic;

namespace ComfortIsland
{
	public interface IAccountingApplication
	{
		string StartupPath { get; }

		Configuration.Settings Settings { get; }

		Database Database { get; }

		IUserInterface UserInterface { get; }
	}
}
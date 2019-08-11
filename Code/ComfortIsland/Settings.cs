using ComfortIsland.BusinessLogic;

namespace ComfortIsland
{
	public class Settings
	{
		#region Свойства

		public double FontSize
		{ get; }

		public BalanceValidationStrategy BalanceValidationStrategy
		{ get; }

		public IDatabaseDriver DatabaseDriver
		{ get; }

		#endregion

		public Settings(string databaseFileName)
		{
			FontSize = 16;
			BalanceValidationStrategy = BalanceValidationStrategy.FinalOnly;
			DatabaseDriver = new Xml.DatabaseDriver(databaseFileName);
		}
	}
}

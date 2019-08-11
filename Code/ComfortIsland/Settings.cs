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

		public DataAccessLayer.IDatabaseDriver DatabaseDriver
		{ get; }

		#endregion

		public Settings(string databaseFileName)
		{
			FontSize = 16;
			BalanceValidationStrategy = BalanceValidationStrategy.FinalOnly;
			DatabaseDriver = new DataAccessLayer.Xml.DatabaseDriver(databaseFileName);
		}
	}
}

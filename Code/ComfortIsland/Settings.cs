using ComfortIsland.BusinessLogic;

namespace ComfortIsland
{
	public static class Settings
	{
		public static double FontSize
		{ get { return 16; } }

		public static BalanceValidationStrategy BalanceValidationStrategy
		{ get { return BalanceValidationStrategy.FinalOnly; } }

		public static IDatabaseDriver DatabaseDriver
		{ get { return _databaseDriver; } }
		private static readonly IDatabaseDriver _databaseDriver = new Xml.DatabaseDriver();
	}
}

using ComfortIsland.BusinessLogic;

namespace ComfortIsland
{
	public static class Settings
	{
		public static double FontSize
		{ get { return 16; } }

		public static BalanceValidationStrategy BalanceValidationStrategy
		{ get { return BalanceValidationStrategy.FinalOnly; } }
	}
}

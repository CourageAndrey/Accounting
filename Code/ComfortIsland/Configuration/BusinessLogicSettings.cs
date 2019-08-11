using ComfortIsland.BusinessLogic;

namespace ComfortIsland.Configuration
{
	public class BusinessLogicSettings
	{
		public BalanceValidationStrategy BalanceValidationStrategy
		{ get; }

		internal BusinessLogicSettings(Xml.BusinessLogicSettings xmlSettings)
		{
			BalanceValidationStrategy = BalanceValidationStrategy.All[xmlSettings.BalanceValidationStrategy];
		}
	}
}

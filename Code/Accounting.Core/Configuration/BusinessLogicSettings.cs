using ComfortIsland.BusinessLogic;

namespace Accounting.Core.Configuration
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

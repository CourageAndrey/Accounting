using ComfortIsland.BusinessLogic;

namespace ComfortIsland.Reports
{
	public class TradeItem
	{
		#region Properties

		public long ProductId
		{ get; private set; }

		public string ProductName
		{ get; private set; }

		public string ProductUnit
		{ get; private set; }

		public decimal InitialBalance
		{ get; internal set; }

		public decimal Income
		{ get; internal set; }

		public decimal Produced
		{ get; internal set; }

		public decimal Selled
		{ get; internal set; }

		public decimal UsedToProduce
		{ get; internal set; }

		public decimal SentToWarehouse
		{ get; internal set; }

		public decimal FinalBalance
		{ get; internal set; }

		#endregion

		public TradeItem(Product product)
		{
			ProductId = product.ID;
			ProductName = product.Name;
			ProductUnit = product.Unit.Name;
			InitialBalance = Income = Produced = Selled = UsedToProduce = SentToWarehouse = FinalBalance = 0;
		}
	}
}
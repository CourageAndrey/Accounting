using Accounting.Core.BusinessLogic;
using Accounting.Core.Helpers;

namespace Accounting.Core.Reports.Existing
{
	public class TradeItem : IReportItem
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

		public string GetValue(string columnBinding)
		{
			switch (columnBinding)
			{
				case "ProductId":
					return ProductId.ToString();
				case "ProductName":
					return ProductName;
				case "ProductUnit":
					return ProductUnit;
				case "InitialBalance":
					return InitialBalance.Simplify();
				case "Income":
					return Income.Simplify();
				case "Produced":
					return Produced.Simplify();
				case "Selled":
					return Selled.Simplify();
				case "UsedToProduce":
					return UsedToProduce.Simplify();
				case "SentToWarehouse":
					return SentToWarehouse.Simplify();
				case "FinalBalance":
					return FinalBalance.Simplify();
				default:
					return null;
			}
		}
	}
}
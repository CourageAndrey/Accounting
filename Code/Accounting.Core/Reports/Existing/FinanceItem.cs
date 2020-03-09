using Accounting.Core.Helpers;

namespace Accounting.Core.Reports.Existing
{
	public class FinanceItem : IReportItem
	{
		#region Properties

		public string Item
		{ get; }

		public decimal Value
		{ get; }

		#endregion

		public FinanceItem(string item, decimal value)
		{
			Item = item;
			Value = value;
		}

		public string GetValue(string columnBinding)
		{
			switch (columnBinding)
			{
				case "Item":
					return Item;
				case "Value":
					return Value.Simplify();
				default:
					return null;
			}
		}
	}
}
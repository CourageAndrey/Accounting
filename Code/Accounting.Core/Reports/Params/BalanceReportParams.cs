using System;

namespace ComfortIsland.Reports.Params
{
	public class BalanceReportParams
	{
		public DateTime Date
		{ get; set; }

		public bool IncludeAllProducts
		{ get; set; }

		#region Constructors

		public BalanceReportParams()
			: this(DateTime.Now, true)
		{ }

		public BalanceReportParams(DateTime date, bool includeAllProducts)
		{
			Date = date;
			IncludeAllProducts = includeAllProducts;
		}

		#endregion
	}
}

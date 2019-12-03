using System;

using Accounting.Core.Helpers;

namespace ComfortIsland.Reports.Params
{
	public class PeriodParams
	{
		public Period Period
		{ get; set; }

		#region Constructors

		public PeriodParams()
			: this(DateTime.Now.AddDays(-7), DateTime.Now)
		{ }

		public PeriodParams(Period period)
		{
			Period = period;
		}

		public PeriodParams(DateTime fromDate, DateTime toDate)
			: this(new Period(fromDate, toDate))
		{ }

		#endregion
	}
}

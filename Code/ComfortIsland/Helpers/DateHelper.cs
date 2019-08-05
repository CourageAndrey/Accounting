using System;
using System.Collections.Generic;

namespace ComfortIsland.Helpers
{
	public static class DateHelper
	{
		private static readonly IDictionary<DayOfWeek, int> _mondayOffset = new Dictionary<DayOfWeek, int>
		{
			{ DayOfWeek.Monday, 0 },
			{ DayOfWeek.Tuesday, -1 },
			{ DayOfWeek.Wednesday, -2 },
			{ DayOfWeek.Thursday, -3 },
			{ DayOfWeek.Friday, -4 },
			{ DayOfWeek.Saturday, -5 },
			{ DayOfWeek.Sunday, -6 },
		};

		public static DateTime GetBeginOfWeek(this DateTime dateTime)
		{
			return dateTime.AddDays(_mondayOffset[dateTime.DayOfWeek]);
		}

		public static DateTime EndOfDay(this DateTime dateTime)
		{
			return dateTime.Date.AddDays(1).AddMilliseconds(-1);
		}
	}
}

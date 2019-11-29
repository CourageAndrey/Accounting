using System;

using NUnit.Framework;

using ComfortIsland.Helpers;

namespace Accounting.Core.UnitTests.Helpers
{
	public class DateHelperTest
	{
		[Test]
		public void GetBeginOfWeekForAllDays()
		{
			var now = DateTime.Now;
			for (int i = 0; i < 8; i++)
			{
				Assert.AreEqual(DayOfWeek.Monday, now.AddDays(i).GetBeginOfWeek().DayOfWeek);
			}
		}

		[Test]
		public void GetEndOfWholeDay()
		{
			var now = new DateTime(2000, 1, 1);
			while (now.Day < 2)
			{
				var endOfDay = now.EndOfDay();

				Assert.AreEqual(now.Date, endOfDay.Date);
				Assert.AreEqual(23, endOfDay.Hour);
				Assert.AreEqual(59, endOfDay.Minute);
				Assert.AreEqual(59, endOfDay.Second);

				now = now.AddMinutes(45);
			}
		}
	}
}

using System;

using NUnit.Framework;

using ComfortIsland.Helpers;

namespace ComfortIsland.UnitTests.Helpers
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
	}
}

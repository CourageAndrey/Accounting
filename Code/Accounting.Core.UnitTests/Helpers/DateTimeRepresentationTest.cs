using System;

using NUnit.Framework;

using Accounting.Core.Helpers;

namespace Accounting.Core.UnitTests.Helpers
{
	public class DateTimeRepresentationTest
	{
		[Test]
		public void HappyPath()
		{
			// act
			var date = DateTime.Now;
			string dateString = date.ToConvinientStringRepresentation();

			// assert
			Assert.IsTrue(dateString.Contains(date.ToLongDateString()));
			Assert.IsTrue(dateString.Contains(date.ToLongTimeString()));
		}
	}
}

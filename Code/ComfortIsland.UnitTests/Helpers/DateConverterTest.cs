using System;
using System.Globalization;

using NUnit.Framework;

using ComfortIsland.Helpers;

namespace ComfortIsland.UnitTests.Helpers
{
	public class DateConverterTest
	{
		[Test]
		public void ConvertAcceptDateTimeOnly()
		{
			// arrange
			var converter = new DateConverter();

			// act and assert
			Assert.Throws<InvalidCastException>(() => converter.Convert(
				string.Empty,
				typeof(string),
				null,
				CultureInfo.InvariantCulture));
		}

		[Test]
		public void HappyPath()
		{
			// arrange
			var converter = new DateConverter();

			// act
			var date = DateTime.Now;
			string dateString = (string) converter.Convert(
				date,
				typeof(DateTime),
				null,
				CultureInfo.InvariantCulture);

			// assert
			Assert.IsTrue(dateString.Contains(date.ToLongDateString()));
			Assert.IsTrue(dateString.Contains(date.ToLongDateString()));
		}

		[Test]
		public void ConvertBackIsNotSupported()
		{
			// arrange
			var converter = new DateConverter();

			// act and assert
			Assert.Throws<NotSupportedException>(() => converter.ConvertBack(
				string.Empty,
				typeof(DateTime),
				null,
				CultureInfo.InvariantCulture));
		}
	}
}

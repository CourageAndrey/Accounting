using System;
using System.Globalization;

namespace ComfortIsland.Helpers
{
	public static class DateTimeRepresentation
	{
		public static string ToConvinientStringRepresentation(this DateTime value)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", value.ToLongDateString(), value.ToLongTimeString());
		}
	}
}

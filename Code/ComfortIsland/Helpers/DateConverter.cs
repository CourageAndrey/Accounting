using System;
using System.Globalization;

using System.Windows.Data;

namespace ComfortIsland.Helpers
{
	public class DateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTime) value).ToConvinientStringRepresentation();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	public static class DateTimeRepresentation
	{
		public static string ToConvinientStringRepresentation(this DateTime value)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", value.ToLongDateString(), value.ToLongTimeString());
		}
	}
}

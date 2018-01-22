using System;
using System.Globalization;

using System.Windows.Data;

namespace ComfortIsland.Helpers
{
	public class DateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var timestamp = (DateTime) value;
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", timestamp.ToLongDateString(), timestamp.ToShortTimeString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

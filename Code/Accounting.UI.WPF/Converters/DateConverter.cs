using System;
using System.Globalization;
using System.Windows.Data;

using Accounting.Core.Helpers;

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
}

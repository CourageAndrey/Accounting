using System;
using System.Globalization;
using System.Windows.Data;

using Accounting.Core.Helpers;

namespace Accounting.UI.WPF.Converters
{
	[ValueConversion(typeof(decimal), typeof(string))]
	public class DigitRoundingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((decimal) value).Simplify();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return decimal.Parse(value.ToString());
		}

		public static readonly DigitRoundingConverter Instance = new DigitRoundingConverter();
	}
}

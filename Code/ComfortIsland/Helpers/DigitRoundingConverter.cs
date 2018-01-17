using System;
using System.Globalization;
using System.Windows.Data;

namespace ComfortIsland.Helpers
{
	[ValueConversion(typeof(double), typeof(string))]
	public class DigitRoundingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double v = (double) value;
			string result = v.ToString(CultureInfo.CurrentCulture);
			var numberFormat = CultureInfo.CurrentCulture.NumberFormat;
			return result.Contains(numberFormat.CurrencyDecimalSeparator) || result.Contains(numberFormat.NumberDecimalSeparator)
				? v.ToString("N1")
				: v.ToString("N0");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return double.Parse(value.ToString());
		}

		public static readonly DigitRoundingConverter Instance = new DigitRoundingConverter();
	}
}

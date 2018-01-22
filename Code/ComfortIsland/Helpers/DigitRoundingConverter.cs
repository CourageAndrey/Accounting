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
			return Simplify((double) value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return double.Parse(value.ToString());
		}

		public static string Simplify(double value)
		{
			string result = value.ToString(CultureInfo.CurrentCulture);
			var numberFormat = CultureInfo.CurrentCulture.NumberFormat;
			return result.Contains(numberFormat.CurrencyDecimalSeparator) || result.Contains(numberFormat.NumberDecimalSeparator)
				? value.ToString("N1")
				: value.ToString("N0");
		}

		public static readonly DigitRoundingConverter Instance = new DigitRoundingConverter();
	}
}

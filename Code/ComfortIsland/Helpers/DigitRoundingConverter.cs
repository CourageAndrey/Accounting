using System;
using System.Globalization;
using System.Windows.Data;

namespace ComfortIsland.Helpers
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

	public static class DigitRounder
	{
		public static string Simplify(this decimal value)
		{
			string result = value.ToString(CultureInfo.CurrentCulture);
			var numberFormat = CultureInfo.CurrentCulture.NumberFormat;
			return result.Contains(numberFormat.CurrencyDecimalSeparator) || result.Contains(numberFormat.NumberDecimalSeparator)
				? value.ToString("N1")
				: value.ToString("N0");
		}
	}
}

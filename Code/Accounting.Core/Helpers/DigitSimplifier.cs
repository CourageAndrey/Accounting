using System.Globalization;

namespace Accounting.Core.Helpers
{
	public static class DigitSimplifier
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;

namespace Accounting.Reports.OpenXml.Helpers
{
	[SuppressMessage("ReSharper", "PossiblyMistakenUseOfParamsMethod")]
	public static class ThemePartHelper
	{
		public static TColor DefineColorAsSystem<TColor>(SystemColorValues systemColorValues, string lastColor)
			where TColor: Color2Type, new()
		{
			var color = new TColor();

			var systemColor = new SystemColor
			{
				Val = systemColorValues,
				LastColor = lastColor,
			};
			color.Append(systemColor);

			return color;
		}

		public static TColor DefineColorAsRgb<TColor>(string rgbColorModelHex)
			where TColor : Color2Type, new()
		{
			var color = new TColor();

			var hex = new RgbColorModelHex { Val = rgbColorModelHex };
			color.Append(hex);

			return color;
		}

		public static GradientStop DefineGradientStop(
			int position,
			IEnumerable<PercentageValue> percentageValues,
			SchemeColorValues schemeColorValues = SchemeColorValues.PhColor)
		{
			var gradientStop = new GradientStop { Position = position };

			var schemeColor = new SchemeColor { Val = schemeColorValues };
			foreach (var value in percentageValues)
			{
				schemeColor.Append(value.Define());
			}

			gradientStop.Append(schemeColor);

			return gradientStop;
		}
	}

	#region Helper classes

	public abstract class PercentageValue
	{
		public abstract OpenXmlLeafElement Define();
	}

	public class PercentageValue<T> : PercentageValue
		where T : PercentageType, new()
	{
		private readonly int _value;

		public PercentageValue(int value)
		{
			_value = value;
		}

		public override OpenXmlLeafElement Define()
		{
			return new T { Val = _value };
		}
	}

	public class PositiveFixedPercentageValue<T> : PercentageValue
		where T : PositiveFixedPercentageType, new()
	{
		private readonly int _value;

		public PositiveFixedPercentageValue(int value)
		{
			_value = value;
		}

		public override OpenXmlLeafElement Define()
		{
			return new T { Val = _value };
		}
	}

	#endregion
}

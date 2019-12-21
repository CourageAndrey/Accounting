using System.Diagnostics.CodeAnalysis;

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
	}
}

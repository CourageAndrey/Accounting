using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DocumentFormat.OpenXml.Drawing;

namespace Accounting.Reports.OpenXml.Helpers
{
	[SuppressMessage("ReSharper", "PossiblyMistakenUseOfParamsMethod")]
	public static class FontCollectionTypeHelper
	{
		public static void DefineFontScripts(
			this FontCollectionType fontCollectionType,
			string latinFontTypeface,
			string latinFontPanose,
			IDictionary<string, string> additionalSuplementalFonts)
		{
			var latinFont = new LatinFont
			{
				Typeface = latinFontTypeface,
				Panose = latinFontPanose,
			};
			fontCollectionType.Append(latinFont);

			var asianFont = new EastAsianFont { Typeface = "" };
			fontCollectionType.Append(asianFont);

			var complexScriptFont = new ComplexScriptFont { Typeface = "" };
			fontCollectionType.Append(complexScriptFont);

			foreach (string fontScript in AllFontScripts)
			{
				string fontTypeface;
				if (!CommonSupplementalFonts.TryGetValue(fontScript, out fontTypeface))
				{
					fontTypeface = additionalSuplementalFonts[fontScript];
				}

				var supplementalFont = new SupplementalFont
				{
					Script = fontScript,
					Typeface = fontTypeface,
				};

				fontCollectionType.Append(supplementalFont);
			}
		}

		private static readonly IReadOnlyCollection<string> AllFontScripts = new[]
		{
			"Jpan",
			"Hang",
			"Hans",
			"Hant",
			"Arab",
			"Hebr",
			"Thai",
			"Ethi",
			"Beng",
			"Gujr",
			"Khmr",
			"Knda",
			"Guru",
			"Cans",
			"Cher",
			"Yiii",
			"Tibt",
			"Thaa",
			"Deva",
			"Telu",
			"Taml",
			"Syrc",
			"Orya",
			"Mlym",
			"Laoo",
			"Sinh",
			"Mong",
			"Viet",
			"Uigh",
			"Geor",
		};

		public static readonly IDictionary<string, string> CommonSupplementalFonts = new Dictionary<string, string>
		{
			{ "Jpan", "ＭＳ Ｐゴシック" },
			{ "Hang", "맑은 고딕" },
			{ "Hans", "宋体" },
			{ "Hant", "新細明體" },
			{ "Thai", "Tahoma" },
			{ "Ethi", "Nyala" },
			{ "Beng", "Vrinda" },
			{ "Gujr", "Shruti" },
			{ "Knda", "Tunga" },
			{ "Guru", "Raavi" },
			{ "Cans", "Euphemia" },
			{ "Cher", "Plantagenet Cherokee" },
			{ "Yiii", "Microsoft Yi Baiti" },
			{ "Tibt", "Microsoft Himalaya" },
			{ "Thaa", "MV Boli" },
			{ "Deva", "Mangal" },
			{ "Telu", "Gautami" },
			{ "Taml", "Latha" },
			{ "Syrc", "Estrangelo Edessa" },
			{ "Orya", "Kalinga" },
			{ "Mlym", "Kartika" },
			{ "Laoo", "DokChampa" },
			{ "Sinh", "Iskoola Pota" },
			{ "Mong", "Mongolian Baiti" },
			{ "Uigh", "Microsoft Uighur" },
			{ "Geor", "Sylfaen" },
		};

		public static readonly IDictionary<string, string> MajorSupplementalFonts = new Dictionary<string, string>
		{
			{ "Arab", "Times New Roman" },
			{ "Hebr", "Times New Roman" },
			{ "Khmr", "MoolBoran" },
			{ "Viet", "Times New Roman" },
		};

		public static readonly IDictionary<string, string> MinorSupplementalFonts = new Dictionary<string, string>
		{
			{ "Arab", "Arial" },
			{ "Hebr", "Arial" },
			{ "Khmr", "DaunPenh" },
			{ "Viet", "Arial" },
		};
	}
}

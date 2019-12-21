using System.Diagnostics.CodeAnalysis;

using DocumentFormat.OpenXml.Spreadsheet;

namespace Accounting.Reports.OpenXml.Helpers
{
	[SuppressMessage("ReSharper", "PossiblyMistakenUseOfParamsMethod")]
	public static class StylesPartHelper
	{
		public static Border DefineBorder(bool isThin)
		{
			var border = new Border();

			foreach (var sideBorder in new BorderPropertiesType[]
			{
				new LeftBorder(),
				new RightBorder(),
				new TopBorder(),
				new BottomBorder(),
				new DiagonalBorder(),
			})
			{
				var color = new Color();
				if (isThin)
				{
					sideBorder.Style = BorderStyleValues.Thin;
					color.Indexed = 64U;
				}
				sideBorder.Append(color);

				border.Append(sideBorder);
			}

			return border;
		}

		public static Font DefineFont(
			bool isBold = false,
			double fontSize = 11,
			uint color = 1,
			string fontName = "Calibri",
			int fontFamilyNumbering = 2,
			int fontCharSet = 204,
			FontSchemeValues fontSchemeValues = FontSchemeValues.Minor)
		{
			var font = new Font();

			if (isBold)
			{
				font.Append(new Bold());
			}
			font.Append(new FontSize { Val = fontSize });
			font.Append(new Color { Theme = color });
			font.Append(new FontName { Val = fontName });
			font.Append(new FontFamilyNumbering { Val = fontFamilyNumbering });
			font.Append(new FontCharSet { Val = fontCharSet });
			font.Append(new FontScheme { Val = fontSchemeValues });

			return font;
		}

		public static StylesheetExtension DefineStylesheetExtension(
			string guidUri,
			string namespacePrefix,
			string namespaceUri,
			string defaultSlicerStyle)
		{
			var stylesheetExtension = new StylesheetExtension { Uri = guidUri };
			stylesheetExtension.AddNamespaceDeclaration(namespacePrefix, namespaceUri);
			var styles = new DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles { DefaultSlicerStyle = defaultSlicerStyle };

			stylesheetExtension.Append(styles);

			return stylesheetExtension;
		}
	}
}

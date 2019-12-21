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

		public static Font DefineFont(bool isBold)
		{
			var font = new Font();

			if (isBold)
			{
				font.Append(new Bold());
			}
			font.Append(new FontSize { Val = 11D });
			font.Append(new Color { Theme = 1U });
			font.Append(new FontName { Val = "Calibri" });
			font.Append(new FontFamilyNumbering { Val = 2 });
			font.Append(new FontCharSet { Val = 204 });
			font.Append(new FontScheme { Val = FontSchemeValues.Minor });

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

using System;
using System.IO;
using System.Linq;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Accounting.Core.Application;
using Accounting.Core.Reports;

namespace Accounting.Reports.OpenXml
{
	public class ExcelOpenXmlReportExporter : IReportExporter
	{
		public string SaveFileDialogFilter
		{ get { return "Файлы Excel OpenXML (XLSX)|*.xlsx"; } }

		public void ExportReport(IReport report, string fileName)
		{
			using (var spreadsheet = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
			{
				exportReport(spreadsheet, report);
			}
		}

		private static void exportReport(SpreadsheetDocument document, IReport report)
		{
			var extendedFilePropertiesPart = document.AddNewPart<ExtendedFilePropertiesPart>("rId3");
			generateExtendedFilePropertiesPartContent(extendedFilePropertiesPart);

			var workbookPart = document.AddWorkbookPart();
			generateWorkbookPartContent(workbookPart);

			var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rId3");
			generateWorkbookStylesPartContent(workbookStylesPart);

			var themePart = workbookPart.AddNewPart<ThemePart>("rId2");
			generateThemePartContent(themePart);

			var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("rId1");
			generateWorksheetPartContent(worksheetPart, report);

			var spreadsheetPrinterSettingsPart = worksheetPart.AddNewPart<SpreadsheetPrinterSettingsPart>("rId1");
			generateSpreadsheetPrinterSettingsPartContent(spreadsheetPrinterSettingsPart);

			var sharedStringTablePart = workbookPart.AddNewPart<SharedStringTablePart>("rId4");
			generateSharedStringTablePartContent(sharedStringTablePart);

			setPackageProperties(document);
		}

		private static void generateExtendedFilePropertiesPartContent(ExtendedFilePropertiesPart extendedFilePropertiesPart)
		{
			var properties = new DocumentFormat.OpenXml.ExtendedProperties.Properties();
			properties.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
			var application = new DocumentFormat.OpenXml.ExtendedProperties.Application { Text = "Microsoft Excel" };
			var documentSecurity = new DocumentFormat.OpenXml.ExtendedProperties.DocumentSecurity { Text = "0" };
			var scaleCrop = new DocumentFormat.OpenXml.ExtendedProperties.ScaleCrop { Text = "false" };

			var headingPairs = new DocumentFormat.OpenXml.ExtendedProperties.HeadingPairs();

			var vTVector = new DocumentFormat.OpenXml.VariantTypes.VTVector
			{
				BaseType = DocumentFormat.OpenXml.VariantTypes.VectorBaseValues.Variant,
				Size = (UInt32Value)2U
			};

			var variantPages = new DocumentFormat.OpenXml.VariantTypes.Variant();
			var vTLPSTRPages = new DocumentFormat.OpenXml.VariantTypes.VTLPSTR { Text = "Листы" };

			variantPages.Append(vTLPSTRPages);

			var variant1 = new DocumentFormat.OpenXml.VariantTypes.Variant();
			var vTInt321 = new DocumentFormat.OpenXml.VariantTypes.VTInt32 { Text = "1" };

			variant1.Append(vTInt321);

			vTVector.Append(variantPages);
			vTVector.Append(variant1);

			headingPairs.Append(vTVector);

			var titlesOfParts = new DocumentFormat.OpenXml.ExtendedProperties.TitlesOfParts();

			var vTVectorTitlesOfParts = new DocumentFormat.OpenXml.VariantTypes.VTVector
			{
				BaseType = DocumentFormat.OpenXml.VariantTypes.VectorBaseValues.Lpstr,
				Size = (UInt32Value)1U
			};
			var vTLPSTRTitlesOfParts = new DocumentFormat.OpenXml.VariantTypes.VTLPSTR { Text = "Лист1" };

			vTVectorTitlesOfParts.Append(vTLPSTRTitlesOfParts);

			titlesOfParts.Append(vTVectorTitlesOfParts);
			var company = new DocumentFormat.OpenXml.ExtendedProperties.Company { Text = "" };
			var linksUpToDate = new DocumentFormat.OpenXml.ExtendedProperties.LinksUpToDate { Text = "false" };
			var sharedDocument = new DocumentFormat.OpenXml.ExtendedProperties.SharedDocument { Text = "false" };
			var hyperlinksChanged = new DocumentFormat.OpenXml.ExtendedProperties.HyperlinksChanged { Text = "false" };
			var applicationVersion = new DocumentFormat.OpenXml.ExtendedProperties.ApplicationVersion { Text = "15.0300" };

			properties.Append(application);
			properties.Append(documentSecurity);
			properties.Append(scaleCrop);
			properties.Append(headingPairs);
			properties.Append(titlesOfParts);
			properties.Append(company);
			properties.Append(linksUpToDate);
			properties.Append(sharedDocument);
			properties.Append(hyperlinksChanged);
			properties.Append(applicationVersion);

			extendedFilePropertiesPart.Properties = properties;
		}

		private static void generateWorkbookPartContent(WorkbookPart workbookPart)
		{
			var workbook = new Workbook { MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "x15" } };
			workbook.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			workbook.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			workbook.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
			var fileVersion = new FileVersion { ApplicationName = "xl", LastEdited = "6", LowestEdited = "6", BuildVersion = "14420" };
			var workbookProperties1 = new WorkbookProperties { DefaultThemeVersion = (UInt32Value)153222U };

			var alternateContent = new AlternateContent();
			alternateContent.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");

			var alternateContentChoice = new AlternateContentChoice { Requires = "x15" };

			var absolutePath = new DocumentFormat.OpenXml.Office2013.ExcelAc.AbsolutePath { Url = "D:\\Current\\" };
			absolutePath.AddNamespaceDeclaration("x15ac", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/ac");

			alternateContentChoice.Append(absolutePath);

			alternateContent.Append(alternateContentChoice);

			var bookViews = new BookViews();
			var workbookView = new WorkbookView { XWindow = 0, YWindow = 0, WindowWidth = (UInt32Value)28800U, WindowHeight = (UInt32Value)12435U };

			bookViews.Append(workbookView);

			var sheets = new Sheets();
			var sheet = new Sheet { Name = "Лист1", SheetId = (UInt32Value)1U, Id = "rId1" };

			sheets.Append(sheet);
			var calculationProperties = new CalculationProperties { CalculationId = (UInt32Value)152511U, ReferenceMode = ReferenceModeValues.R1C1 };

			var workbookExtensionList = new WorkbookExtensionList();

			var workbookExtension = new WorkbookExtension { Uri = "{140A7094-0E35-4892-8432-C4D2E57EDEB5}" };
			workbookExtension.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
			var workbookProperties2 = new DocumentFormat.OpenXml.Office2013.Excel.WorkbookProperties { ChartTrackingReferenceBase = true };

			workbookExtension.Append(workbookProperties2);

			workbookExtensionList.Append(workbookExtension);

			workbook.Append(fileVersion);
			workbook.Append(workbookProperties1);
			workbook.Append(alternateContent);
			workbook.Append(bookViews);
			workbook.Append(sheets);
			workbook.Append(calculationProperties);
			workbook.Append(workbookExtensionList);

			workbookPart.Workbook = workbook;
		}

		private static void generateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart)
		{
			var stylesheet = new Stylesheet { MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "x14ac" } };
			stylesheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			stylesheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

			var fonts = new Fonts { Count = (UInt32Value)2U, KnownFonts = true };

			var font1 = new Font();
			var fontSize1 = new FontSize { Val = 11D };
			var color1 = new Color { Theme = (UInt32Value)1U };
			var fontName1 = new FontName { Val = "Calibri" };
			var fontFamilyNumbering1 = new FontFamilyNumbering { Val = 2 };
			var fontCharSet1 = new FontCharSet { Val = 204 };
			var fontScheme1 = new FontScheme { Val = FontSchemeValues.Minor };

			font1.Append(fontSize1);
			font1.Append(color1);
			font1.Append(fontName1);
			font1.Append(fontFamilyNumbering1);
			font1.Append(fontCharSet1);
			font1.Append(fontScheme1);

			var font2 = new Font();
			var bold1 = new Bold();
			var fontSize2 = new FontSize { Val = 11D };
			var color2 = new Color { Theme = (UInt32Value)1U };
			var fontName2 = new FontName { Val = "Calibri" };
			var fontFamilyNumbering2 = new FontFamilyNumbering { Val = 2 };
			var fontCharSet2 = new FontCharSet { Val = 204 };
			var fontScheme2 = new FontScheme { Val = FontSchemeValues.Minor };

			font2.Append(bold1);
			font2.Append(fontSize2);
			font2.Append(color2);
			font2.Append(fontName2);
			font2.Append(fontFamilyNumbering2);
			font2.Append(fontCharSet2);
			font2.Append(fontScheme2);

			fonts.Append(font1);
			fonts.Append(font2);

			var fills = new Fills { Count = (UInt32Value)2U };

			var fill1 = new Fill();
			var patternFill1 = new PatternFill { PatternType = PatternValues.None };

			fill1.Append(patternFill1);

			var fill2 = new Fill();
			var patternFill2 = new PatternFill { PatternType = PatternValues.Gray125 };

			fill2.Append(patternFill2);

			fills.Append(fill1);
			fills.Append(fill2);

			var borders = new Borders { Count = (UInt32Value)2U };

			var border1 = new Border();
			var leftBorder1 = new LeftBorder();
			var rightBorder1 = new RightBorder();
			var topBorder1 = new TopBorder();
			var bottomBorder1 = new BottomBorder();
			var diagonalBorder1 = new DiagonalBorder();

			border1.Append(leftBorder1);
			border1.Append(rightBorder1);
			border1.Append(topBorder1);
			border1.Append(bottomBorder1);
			border1.Append(diagonalBorder1);

			var border2 = new Border();

			var leftBorder2 = new LeftBorder { Style = BorderStyleValues.Thin };
			var color3 = new Color { Indexed = (UInt32Value)64U };

			leftBorder2.Append(color3);

			var rightBorder2 = new RightBorder { Style = BorderStyleValues.Thin };
			var color4 = new Color { Indexed = (UInt32Value)64U };

			rightBorder2.Append(color4);

			var topBorder2 = new TopBorder { Style = BorderStyleValues.Thin };
			var color5 = new Color { Indexed = (UInt32Value)64U };

			topBorder2.Append(color5);

			var bottomBorder2 = new BottomBorder { Style = BorderStyleValues.Thin };
			var color6 = new Color { Indexed = (UInt32Value)64U };

			bottomBorder2.Append(color6);
			var diagonalBorder2 = new DiagonalBorder();

			border2.Append(leftBorder2);
			border2.Append(rightBorder2);
			border2.Append(topBorder2);
			border2.Append(bottomBorder2);
			border2.Append(diagonalBorder2);

			borders.Append(border1);
			borders.Append(border2);

			var cellStyleFormats = new CellStyleFormats { Count = (UInt32Value)1U };
			var cellFormat1 = new CellFormat { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

			cellStyleFormats.Append(cellFormat1);

			var cellFormats1 = new CellFormats { Count = (UInt32Value)4U };
			var cellFormat2 = new CellFormat { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

			var cellFormat3 = new CellFormat { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true, ApplyAlignment = true };
			var alignment1 = new Alignment { Vertical = VerticalAlignmentValues.Center, WrapText = true };

			cellFormat3.Append(alignment1);
			var cellFormat4 = new CellFormat { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };
			var cellFormat5 = new CellFormat { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true };

			cellFormats1.Append(cellFormat2);
			cellFormats1.Append(cellFormat3);
			cellFormats1.Append(cellFormat4);
			cellFormats1.Append(cellFormat5);

			var cellStyles = new CellStyles { Count = (UInt32Value)1U };
			var cellStyle1 = new CellStyle { Name = "Обычный", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

			cellStyles.Append(cellStyle1);
			var differentialFormats = new DifferentialFormats { Count = (UInt32Value)0U };
			var tableStyles = new TableStyles { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

			var stylesheetExtensionList = new StylesheetExtensionList();

			var stylesheetExtension1 = new StylesheetExtension { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
			stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
			var slicerStyles1 = new DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles { DefaultSlicerStyle = "SlicerStyleLight1" };

			stylesheetExtension1.Append(slicerStyles1);

			var stylesheetExtension2 = new StylesheetExtension { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
			stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
			var timelineStyles1 = new DocumentFormat.OpenXml.Office2013.Excel.TimelineStyles { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

			stylesheetExtension2.Append(timelineStyles1);

			stylesheetExtensionList.Append(stylesheetExtension1);
			stylesheetExtensionList.Append(stylesheetExtension2);

			stylesheet.Append(fonts);
			stylesheet.Append(fills);
			stylesheet.Append(borders);
			stylesheet.Append(cellStyleFormats);
			stylesheet.Append(cellFormats1);
			stylesheet.Append(cellStyles);
			stylesheet.Append(differentialFormats);
			stylesheet.Append(tableStyles);
			stylesheet.Append(stylesheetExtensionList);

			workbookStylesPart.Stylesheet = stylesheet;
		}

		private static void generateThemePartContent(ThemePart themePart)
		{
			var theme = new DocumentFormat.OpenXml.Drawing.Theme { Name = "Тема Office" };
			theme.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

			var themeElements = new DocumentFormat.OpenXml.Drawing.ThemeElements();

			var colorScheme = new DocumentFormat.OpenXml.Drawing.ColorScheme { Name = "Стандартная" };

			var dark1Color1 = new DocumentFormat.OpenXml.Drawing.Dark1Color();
			var systemColor1 = new DocumentFormat.OpenXml.Drawing.SystemColor { Val = DocumentFormat.OpenXml.Drawing.SystemColorValues.WindowText, LastColor = "000000" };

			dark1Color1.Append(systemColor1);

			var light1Color1 = new DocumentFormat.OpenXml.Drawing.Light1Color();
			var systemColor2 = new DocumentFormat.OpenXml.Drawing.SystemColor { Val = DocumentFormat.OpenXml.Drawing.SystemColorValues.Window, LastColor = "FFFFFF" };

			light1Color1.Append(systemColor2);

			var dark2Color1 = new DocumentFormat.OpenXml.Drawing.Dark2Color();
			var rgbColorModelHex1 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "44546A" };

			dark2Color1.Append(rgbColorModelHex1);

			var light2Color1 = new DocumentFormat.OpenXml.Drawing.Light2Color();
			var rgbColorModelHex2 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "E7E6E6" };

			light2Color1.Append(rgbColorModelHex2);

			var accent1Color1 = new DocumentFormat.OpenXml.Drawing.Accent1Color();
			var rgbColorModelHex3 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "5B9BD5" };

			accent1Color1.Append(rgbColorModelHex3);

			var accent2Color1 = new DocumentFormat.OpenXml.Drawing.Accent2Color();
			var rgbColorModelHex4 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "ED7D31" };

			accent2Color1.Append(rgbColorModelHex4);

			var accent3Color1 = new DocumentFormat.OpenXml.Drawing.Accent3Color();
			var rgbColorModelHex5 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "A5A5A5" };

			accent3Color1.Append(rgbColorModelHex5);

			var accent4Color1 = new DocumentFormat.OpenXml.Drawing.Accent4Color();
			var rgbColorModelHex6 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "FFC000" };

			accent4Color1.Append(rgbColorModelHex6);

			var accent5Color1 = new DocumentFormat.OpenXml.Drawing.Accent5Color();
			var rgbColorModelHex7 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "4472C4" };

			accent5Color1.Append(rgbColorModelHex7);

			var accent6Color1 = new DocumentFormat.OpenXml.Drawing.Accent6Color();
			var rgbColorModelHex8 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "70AD47" };

			accent6Color1.Append(rgbColorModelHex8);

			var hyperlink = new DocumentFormat.OpenXml.Drawing.Hyperlink();
			var rgbColorModelHex9 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "0563C1" };

			hyperlink.Append(rgbColorModelHex9);

			var followedHyperlinkColor = new DocumentFormat.OpenXml.Drawing.FollowedHyperlinkColor();
			var rgbColorModelHex10 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "954F72" };

			followedHyperlinkColor.Append(rgbColorModelHex10);

			colorScheme.Append(dark1Color1);
			colorScheme.Append(light1Color1);
			colorScheme.Append(dark2Color1);
			colorScheme.Append(light2Color1);
			colorScheme.Append(accent1Color1);
			colorScheme.Append(accent2Color1);
			colorScheme.Append(accent3Color1);
			colorScheme.Append(accent4Color1);
			colorScheme.Append(accent5Color1);
			colorScheme.Append(accent6Color1);
			colorScheme.Append(hyperlink);
			colorScheme.Append(followedHyperlinkColor);

			var fontScheme3 = new DocumentFormat.OpenXml.Drawing.FontScheme { Name = "Стандартная" };

			var majorFont1 = new DocumentFormat.OpenXml.Drawing.MajorFont();
			var latinFont1 = new DocumentFormat.OpenXml.Drawing.LatinFont { Typeface = "Calibri Light", Panose = "020F0302020204030204" };
			var eastAsianFont1 = new DocumentFormat.OpenXml.Drawing.EastAsianFont { Typeface = "" };
			var complexScriptFont1 = new DocumentFormat.OpenXml.Drawing.ComplexScriptFont { Typeface = "" };
			var supplementalFont1 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Jpan", Typeface = "ＭＳ Ｐゴシック" };
			var supplementalFont2 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hang", Typeface = "맑은 고딕" };
			var supplementalFont3 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hans", Typeface = "宋体" };
			var supplementalFont4 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hant", Typeface = "新細明體" };
			var supplementalFont5 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Arab", Typeface = "Times New Roman" };
			var supplementalFont6 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hebr", Typeface = "Times New Roman" };
			var supplementalFont7 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Thai", Typeface = "Tahoma" };
			var supplementalFont8 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Ethi", Typeface = "Nyala" };
			var supplementalFont9 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Beng", Typeface = "Vrinda" };
			var supplementalFont10 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Gujr", Typeface = "Shruti" };
			var supplementalFont11 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Khmr", Typeface = "MoolBoran" };
			var supplementalFont12 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Knda", Typeface = "Tunga" };
			var supplementalFont13 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Guru", Typeface = "Raavi" };
			var supplementalFont14 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Cans", Typeface = "Euphemia" };
			var supplementalFont15 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Cher", Typeface = "Plantagenet Cherokee" };
			var supplementalFont16 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
			var supplementalFont17 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Tibt", Typeface = "Microsoft Himalaya" };
			var supplementalFont18 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Thaa", Typeface = "MV Boli" };
			var supplementalFont19 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Deva", Typeface = "Mangal" };
			var supplementalFont20 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Telu", Typeface = "Gautami" };
			var supplementalFont21 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Taml", Typeface = "Latha" };
			var supplementalFont22 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Syrc", Typeface = "Estrangelo Edessa" };
			var supplementalFont23 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Orya", Typeface = "Kalinga" };
			var supplementalFont24 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Mlym", Typeface = "Kartika" };
			var supplementalFont25 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Laoo", Typeface = "DokChampa" };
			var supplementalFont26 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Sinh", Typeface = "Iskoola Pota" };
			var supplementalFont27 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Mong", Typeface = "Mongolian Baiti" };
			var supplementalFont28 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Viet", Typeface = "Times New Roman" };
			var supplementalFont29 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Uigh", Typeface = "Microsoft Uighur" };
			var supplementalFont30 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Geor", Typeface = "Sylfaen" };

			majorFont1.Append(latinFont1);
			majorFont1.Append(eastAsianFont1);
			majorFont1.Append(complexScriptFont1);
			majorFont1.Append(supplementalFont1);
			majorFont1.Append(supplementalFont2);
			majorFont1.Append(supplementalFont3);
			majorFont1.Append(supplementalFont4);
			majorFont1.Append(supplementalFont5);
			majorFont1.Append(supplementalFont6);
			majorFont1.Append(supplementalFont7);
			majorFont1.Append(supplementalFont8);
			majorFont1.Append(supplementalFont9);
			majorFont1.Append(supplementalFont10);
			majorFont1.Append(supplementalFont11);
			majorFont1.Append(supplementalFont12);
			majorFont1.Append(supplementalFont13);
			majorFont1.Append(supplementalFont14);
			majorFont1.Append(supplementalFont15);
			majorFont1.Append(supplementalFont16);
			majorFont1.Append(supplementalFont17);
			majorFont1.Append(supplementalFont18);
			majorFont1.Append(supplementalFont19);
			majorFont1.Append(supplementalFont20);
			majorFont1.Append(supplementalFont21);
			majorFont1.Append(supplementalFont22);
			majorFont1.Append(supplementalFont23);
			majorFont1.Append(supplementalFont24);
			majorFont1.Append(supplementalFont25);
			majorFont1.Append(supplementalFont26);
			majorFont1.Append(supplementalFont27);
			majorFont1.Append(supplementalFont28);
			majorFont1.Append(supplementalFont29);
			majorFont1.Append(supplementalFont30);

			var minorFont1 = new DocumentFormat.OpenXml.Drawing.MinorFont();
			var latinFont2 = new DocumentFormat.OpenXml.Drawing.LatinFont { Typeface = "Calibri", Panose = "020F0502020204030204" };
			var eastAsianFont2 = new DocumentFormat.OpenXml.Drawing.EastAsianFont { Typeface = "" };
			var complexScriptFont2 = new DocumentFormat.OpenXml.Drawing.ComplexScriptFont { Typeface = "" };
			var supplementalFont31 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Jpan", Typeface = "ＭＳ Ｐゴシック" };
			var supplementalFont32 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hang", Typeface = "맑은 고딕" };
			var supplementalFont33 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hans", Typeface = "宋体" };
			var supplementalFont34 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hant", Typeface = "新細明體" };
			var supplementalFont35 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Arab", Typeface = "Arial" };
			var supplementalFont36 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Hebr", Typeface = "Arial" };
			var supplementalFont37 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Thai", Typeface = "Tahoma" };
			var supplementalFont38 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Ethi", Typeface = "Nyala" };
			var supplementalFont39 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Beng", Typeface = "Vrinda" };
			var supplementalFont40 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Gujr", Typeface = "Shruti" };
			var supplementalFont41 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Khmr", Typeface = "DaunPenh" };
			var supplementalFont42 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Knda", Typeface = "Tunga" };
			var supplementalFont43 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Guru", Typeface = "Raavi" };
			var supplementalFont44 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Cans", Typeface = "Euphemia" };
			var supplementalFont45 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Cher", Typeface = "Plantagenet Cherokee" };
			var supplementalFont46 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
			var supplementalFont47 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Tibt", Typeface = "Microsoft Himalaya" };
			var supplementalFont48 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Thaa", Typeface = "MV Boli" };
			var supplementalFont49 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Deva", Typeface = "Mangal" };
			var supplementalFont50 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Telu", Typeface = "Gautami" };
			var supplementalFont51 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Taml", Typeface = "Latha" };
			var supplementalFont52 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Syrc", Typeface = "Estrangelo Edessa" };
			var supplementalFont53 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Orya", Typeface = "Kalinga" };
			var supplementalFont54 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Mlym", Typeface = "Kartika" };
			var supplementalFont55 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Laoo", Typeface = "DokChampa" };
			var supplementalFont56 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Sinh", Typeface = "Iskoola Pota" };
			var supplementalFont57 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Mong", Typeface = "Mongolian Baiti" };
			var supplementalFont58 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Viet", Typeface = "Arial" };
			var supplementalFont59 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Uigh", Typeface = "Microsoft Uighur" };
			var supplementalFont60 = new DocumentFormat.OpenXml.Drawing.SupplementalFont { Script = "Geor", Typeface = "Sylfaen" };

			minorFont1.Append(latinFont2);
			minorFont1.Append(eastAsianFont2);
			minorFont1.Append(complexScriptFont2);
			minorFont1.Append(supplementalFont31);
			minorFont1.Append(supplementalFont32);
			minorFont1.Append(supplementalFont33);
			minorFont1.Append(supplementalFont34);
			minorFont1.Append(supplementalFont35);
			minorFont1.Append(supplementalFont36);
			minorFont1.Append(supplementalFont37);
			minorFont1.Append(supplementalFont38);
			minorFont1.Append(supplementalFont39);
			minorFont1.Append(supplementalFont40);
			minorFont1.Append(supplementalFont41);
			minorFont1.Append(supplementalFont42);
			minorFont1.Append(supplementalFont43);
			minorFont1.Append(supplementalFont44);
			minorFont1.Append(supplementalFont45);
			minorFont1.Append(supplementalFont46);
			minorFont1.Append(supplementalFont47);
			minorFont1.Append(supplementalFont48);
			minorFont1.Append(supplementalFont49);
			minorFont1.Append(supplementalFont50);
			minorFont1.Append(supplementalFont51);
			minorFont1.Append(supplementalFont52);
			minorFont1.Append(supplementalFont53);
			minorFont1.Append(supplementalFont54);
			minorFont1.Append(supplementalFont55);
			minorFont1.Append(supplementalFont56);
			minorFont1.Append(supplementalFont57);
			minorFont1.Append(supplementalFont58);
			minorFont1.Append(supplementalFont59);
			minorFont1.Append(supplementalFont60);

			fontScheme3.Append(majorFont1);
			fontScheme3.Append(minorFont1);

			var formatScheme = new DocumentFormat.OpenXml.Drawing.FormatScheme { Name = "Стандартная" };

			var fillStyleList = new DocumentFormat.OpenXml.Drawing.FillStyleList();

			var solidFill1 = new DocumentFormat.OpenXml.Drawing.SolidFill();
			var schemeColor1 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };

			solidFill1.Append(schemeColor1);

			var gradientFill1 = new DocumentFormat.OpenXml.Drawing.GradientFill { RotateWithShape = true };

			var gradientStopList1 = new DocumentFormat.OpenXml.Drawing.GradientStopList();

			var gradientStop1 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 0 };

			var schemeColor2 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var luminanceModulation1 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 110000 };
			var saturationModulation1 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 105000 };
			var tint1 = new DocumentFormat.OpenXml.Drawing.Tint { Val = 67000 };

			schemeColor2.Append(luminanceModulation1);
			schemeColor2.Append(saturationModulation1);
			schemeColor2.Append(tint1);

			gradientStop1.Append(schemeColor2);

			var gradientStop2 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 50000 };

			var schemeColor3 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var luminanceModulation2 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 105000 };
			var saturationModulation2 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 103000 };
			var tint2 = new DocumentFormat.OpenXml.Drawing.Tint { Val = 73000 };

			schemeColor3.Append(luminanceModulation2);
			schemeColor3.Append(saturationModulation2);
			schemeColor3.Append(tint2);

			gradientStop2.Append(schemeColor3);

			var gradientStop3 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 100000 };

			var schemeColor4 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var luminanceModulation3 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 105000 };
			var saturationModulation3 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 109000 };
			var tint3 = new DocumentFormat.OpenXml.Drawing.Tint { Val = 81000 };

			schemeColor4.Append(luminanceModulation3);
			schemeColor4.Append(saturationModulation3);
			schemeColor4.Append(tint3);

			gradientStop3.Append(schemeColor4);

			gradientStopList1.Append(gradientStop1);
			gradientStopList1.Append(gradientStop2);
			gradientStopList1.Append(gradientStop3);
			var linearGradientFill1 = new DocumentFormat.OpenXml.Drawing.LinearGradientFill { Angle = 5400000, Scaled = false };

			gradientFill1.Append(gradientStopList1);
			gradientFill1.Append(linearGradientFill1);

			var gradientFill2 = new DocumentFormat.OpenXml.Drawing.GradientFill { RotateWithShape = true };

			var gradientStopList2 = new DocumentFormat.OpenXml.Drawing.GradientStopList();

			var gradientStop4 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 0 };

			var schemeColor5 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var saturationModulation4 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 103000 };
			var luminanceModulation4 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 102000 };
			var tint4 = new DocumentFormat.OpenXml.Drawing.Tint { Val = 94000 };

			schemeColor5.Append(saturationModulation4);
			schemeColor5.Append(luminanceModulation4);
			schemeColor5.Append(tint4);

			gradientStop4.Append(schemeColor5);

			var gradientStop5 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 50000 };

			var schemeColor6 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var saturationModulation5 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 110000 };
			var luminanceModulation5 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 100000 };
			var shade1 = new DocumentFormat.OpenXml.Drawing.Shade { Val = 100000 };

			schemeColor6.Append(saturationModulation5);
			schemeColor6.Append(luminanceModulation5);
			schemeColor6.Append(shade1);

			gradientStop5.Append(schemeColor6);

			var gradientStop6 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 100000 };

			var schemeColor7 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var luminanceModulation6 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 99000 };
			var saturationModulation6 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 120000 };
			var shade2 = new DocumentFormat.OpenXml.Drawing.Shade { Val = 78000 };

			schemeColor7.Append(luminanceModulation6);
			schemeColor7.Append(saturationModulation6);
			schemeColor7.Append(shade2);

			gradientStop6.Append(schemeColor7);

			gradientStopList2.Append(gradientStop4);
			gradientStopList2.Append(gradientStop5);
			gradientStopList2.Append(gradientStop6);
			var linearGradientFill2 = new DocumentFormat.OpenXml.Drawing.LinearGradientFill { Angle = 5400000, Scaled = false };

			gradientFill2.Append(gradientStopList2);
			gradientFill2.Append(linearGradientFill2);

			fillStyleList.Append(solidFill1);
			fillStyleList.Append(gradientFill1);
			fillStyleList.Append(gradientFill2);

			var lineStyleList1 = new DocumentFormat.OpenXml.Drawing.LineStyleList();

			var outline1 = new DocumentFormat.OpenXml.Drawing.Outline { Width = 6350, CapType = DocumentFormat.OpenXml.Drawing.LineCapValues.Flat, CompoundLineType = DocumentFormat.OpenXml.Drawing.CompoundLineValues.Single, Alignment = DocumentFormat.OpenXml.Drawing.PenAlignmentValues.Center };

			var solidFill2 = new DocumentFormat.OpenXml.Drawing.SolidFill();
			var schemeColor8 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };

			solidFill2.Append(schemeColor8);
			var presetDash1 = new DocumentFormat.OpenXml.Drawing.PresetDash { Val = DocumentFormat.OpenXml.Drawing.PresetLineDashValues.Solid };
			var miter1 = new DocumentFormat.OpenXml.Drawing.Miter { Limit = 800000 };

			outline1.Append(solidFill2);
			outline1.Append(presetDash1);
			outline1.Append(miter1);

			var outline2 = new DocumentFormat.OpenXml.Drawing.Outline { Width = 12700, CapType = DocumentFormat.OpenXml.Drawing.LineCapValues.Flat, CompoundLineType = DocumentFormat.OpenXml.Drawing.CompoundLineValues.Single, Alignment = DocumentFormat.OpenXml.Drawing.PenAlignmentValues.Center };

			var solidFill3 = new DocumentFormat.OpenXml.Drawing.SolidFill();
			var schemeColor9 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };

			solidFill3.Append(schemeColor9);
			var presetDash2 = new DocumentFormat.OpenXml.Drawing.PresetDash { Val = DocumentFormat.OpenXml.Drawing.PresetLineDashValues.Solid };
			var miter2 = new DocumentFormat.OpenXml.Drawing.Miter { Limit = 800000 };

			outline2.Append(solidFill3);
			outline2.Append(presetDash2);
			outline2.Append(miter2);

			var outline3 = new DocumentFormat.OpenXml.Drawing.Outline { Width = 19050, CapType = DocumentFormat.OpenXml.Drawing.LineCapValues.Flat, CompoundLineType = DocumentFormat.OpenXml.Drawing.CompoundLineValues.Single, Alignment = DocumentFormat.OpenXml.Drawing.PenAlignmentValues.Center };

			var solidFill4 = new DocumentFormat.OpenXml.Drawing.SolidFill();
			var schemeColor10 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };

			solidFill4.Append(schemeColor10);
			var presetDash3 = new DocumentFormat.OpenXml.Drawing.PresetDash { Val = DocumentFormat.OpenXml.Drawing.PresetLineDashValues.Solid };
			var miter3 = new DocumentFormat.OpenXml.Drawing.Miter { Limit = 800000 };

			outline3.Append(solidFill4);
			outline3.Append(presetDash3);
			outline3.Append(miter3);

			lineStyleList1.Append(outline1);
			lineStyleList1.Append(outline2);
			lineStyleList1.Append(outline3);

			var effectStyleList1 = new DocumentFormat.OpenXml.Drawing.EffectStyleList();

			var effectStyle1 = new DocumentFormat.OpenXml.Drawing.EffectStyle();
			var effectList1 = new DocumentFormat.OpenXml.Drawing.EffectList();

			effectStyle1.Append(effectList1);

			var effectStyle2 = new DocumentFormat.OpenXml.Drawing.EffectStyle();
			var effectList2 = new DocumentFormat.OpenXml.Drawing.EffectList();

			effectStyle2.Append(effectList2);

			var effectStyle3 = new DocumentFormat.OpenXml.Drawing.EffectStyle();

			var effectList3 = new DocumentFormat.OpenXml.Drawing.EffectList();

			var outerShadow1 = new DocumentFormat.OpenXml.Drawing.OuterShadow { BlurRadius = 57150L, Distance = 19050L, Direction = 5400000, Alignment = DocumentFormat.OpenXml.Drawing.RectangleAlignmentValues.Center, RotateWithShape = false };

			var rgbColorModelHex11 = new DocumentFormat.OpenXml.Drawing.RgbColorModelHex { Val = "000000" };
			var alpha1 = new DocumentFormat.OpenXml.Drawing.Alpha { Val = 63000 };

			rgbColorModelHex11.Append(alpha1);

			outerShadow1.Append(rgbColorModelHex11);

			effectList3.Append(outerShadow1);

			effectStyle3.Append(effectList3);

			effectStyleList1.Append(effectStyle1);
			effectStyleList1.Append(effectStyle2);
			effectStyleList1.Append(effectStyle3);

			var backgroundFillStyleList1 = new DocumentFormat.OpenXml.Drawing.BackgroundFillStyleList();

			var solidFill5 = new DocumentFormat.OpenXml.Drawing.SolidFill();
			var schemeColor11 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };

			solidFill5.Append(schemeColor11);

			var solidFill6 = new DocumentFormat.OpenXml.Drawing.SolidFill();

			var schemeColor12 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var tint5 = new DocumentFormat.OpenXml.Drawing.Tint { Val = 95000 };
			var saturationModulation7 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 170000 };

			schemeColor12.Append(tint5);
			schemeColor12.Append(saturationModulation7);

			solidFill6.Append(schemeColor12);

			var gradientFill3 = new DocumentFormat.OpenXml.Drawing.GradientFill { RotateWithShape = true };

			var gradientStopList3 = new DocumentFormat.OpenXml.Drawing.GradientStopList();

			var gradientStop7 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 0 };

			var schemeColor13 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var tint6 = new DocumentFormat.OpenXml.Drawing.Tint { Val = 93000 };
			var saturationModulation8 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 150000 };
			var shade3 = new DocumentFormat.OpenXml.Drawing.Shade { Val = 98000 };
			var luminanceModulation7 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 102000 };

			schemeColor13.Append(tint6);
			schemeColor13.Append(saturationModulation8);
			schemeColor13.Append(shade3);
			schemeColor13.Append(luminanceModulation7);

			gradientStop7.Append(schemeColor13);

			var gradientStop8 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 50000 };

			var schemeColor14 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var tint7 = new DocumentFormat.OpenXml.Drawing.Tint { Val = 98000 };
			var saturationModulation9 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 130000 };
			var shade4 = new DocumentFormat.OpenXml.Drawing.Shade { Val = 90000 };
			var luminanceModulation8 = new DocumentFormat.OpenXml.Drawing.LuminanceModulation { Val = 103000 };

			schemeColor14.Append(tint7);
			schemeColor14.Append(saturationModulation9);
			schemeColor14.Append(shade4);
			schemeColor14.Append(luminanceModulation8);

			gradientStop8.Append(schemeColor14);

			var gradientStop9 = new DocumentFormat.OpenXml.Drawing.GradientStop { Position = 100000 };

			var schemeColor15 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };
			var shade5 = new DocumentFormat.OpenXml.Drawing.Shade { Val = 63000 };
			var saturationModulation10 = new DocumentFormat.OpenXml.Drawing.SaturationModulation { Val = 120000 };

			schemeColor15.Append(shade5);
			schemeColor15.Append(saturationModulation10);

			gradientStop9.Append(schemeColor15);

			gradientStopList3.Append(gradientStop7);
			gradientStopList3.Append(gradientStop8);
			gradientStopList3.Append(gradientStop9);
			var linearGradientFill3 = new DocumentFormat.OpenXml.Drawing.LinearGradientFill { Angle = 5400000, Scaled = false };

			gradientFill3.Append(gradientStopList3);
			gradientFill3.Append(linearGradientFill3);

			backgroundFillStyleList1.Append(solidFill5);
			backgroundFillStyleList1.Append(solidFill6);
			backgroundFillStyleList1.Append(gradientFill3);

			formatScheme.Append(fillStyleList);
			formatScheme.Append(lineStyleList1);
			formatScheme.Append(effectStyleList1);
			formatScheme.Append(backgroundFillStyleList1);

			themeElements.Append(colorScheme);
			themeElements.Append(fontScheme3);
			themeElements.Append(formatScheme);
			var objectDefaults = new DocumentFormat.OpenXml.Drawing.ObjectDefaults();
			var extraColorSchemeList = new DocumentFormat.OpenXml.Drawing.ExtraColorSchemeList();

			var officeStyleSheetExtensionList = new DocumentFormat.OpenXml.Drawing.OfficeStyleSheetExtensionList();

			var officeStyleSheetExtension = new DocumentFormat.OpenXml.Drawing.OfficeStyleSheetExtension { Uri = "{05A4C25C-085E-4340-85A3-A5531E510DB2}" };

			var themeFamily = new DocumentFormat.OpenXml.Office2013.Theme.ThemeFamily { Name = "Office Theme", Id = "{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}", Vid = "{4A3C46E8-61CC-4603-A589-7422A47A8E4A}" };
			themeFamily.AddNamespaceDeclaration("thm15", "http://schemas.microsoft.com/office/thememl/2012/main");

			officeStyleSheetExtension.Append(themeFamily);

			officeStyleSheetExtensionList.Append(officeStyleSheetExtension);

			theme.Append(themeElements);
			theme.Append(objectDefaults);
			theme.Append(extraColorSchemeList);
			theme.Append(officeStyleSheetExtensionList);

			themePart.Theme = theme;
		}

		private static void generateWorksheetPartContent(WorksheetPart worksheetPart, IReport report)
		{
			var worksheet = new Worksheet { MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "x14ac" } };
			worksheet.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			worksheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			worksheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
			var sheetDimension = new SheetDimension { Reference = "B2:E8" };

			var sheetViews = new SheetViews();

			var sheetView = new SheetView { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
			var selection = new Selection { ActiveCell = "E9", SequenceOfReferences = new ListValue<StringValue> { InnerText = "E9" } };

			sheetView.Append(selection);

			sheetViews.Append(sheetView);
			var sheetFormatProperties = new SheetFormatProperties { DefaultRowHeight = 15D, DyDescent = 0.25D };

			var reportColumns = report.Descriptor.GetColumns().ToList();

			var columns = new Columns();
			for (uint c = 0; c < reportColumns.Count; c++)
			{
				columns.Append(new Column { Min = c + 2U, Max = c + 2U, Width = (uint) reportColumns[(int) c].MinWidth / 5, CustomWidth = true });
			}

			var sheetData = new SheetData();

			var rowReportHeader = new Row { RowIndex = (UInt32Value)2U, Spans = new ListValue<StringValue> { InnerText = "2:5" }, DyDescent = 0.25D };

			var cellReportHeader = new Cell { CellReference = "B2", StyleIndex = 3U, DataType = CellValues.String };
			cellReportHeader.Append(new CellValue { Text = report.Title });
			rowReportHeader.Append(cellReportHeader);
			sheetData.Append(rowReportHeader);

			var rowTableHeader = new Row { RowIndex = 4U, Spans = new ListValue<StringValue> { InnerText = "2:5" }, DyDescent = 0.25D };
			for (int c = 0; c < reportColumns.Count; c++)
			{
				var headerCell = new Cell { CellReference = (char)('B' + c) + "4", StyleIndex = 2U, DataType = CellValues.String };
				headerCell.Append(new CellValue { Text = reportColumns[c].Header.ToString() });
				rowTableHeader.Append(headerCell);
			}
			sheetData.Append(rowTableHeader);

			for (uint r = 0; r < report.Items.Count; r++)
			{
				var row = new Row { RowIndex = 5 + r, Spans = new ListValue<StringValue> { InnerText = "2:5" }, DyDescent = 0.25D };
				for (int c = 0; c < reportColumns.Count; c++)
				{
					var cell = new Cell { CellReference = (char)('B' + c) + (5 + r).ToString(), StyleIndex = 1U, DataType = CellValues.String };
					string cellText = report.Items[(int) r].GetValue(reportColumns[c].Binding);
					cell.Append(new CellValue { Text = cellText });
					row.Append(cell);
				}
				sheetData.Append(row);
			}

			var pageMargins = new PageMargins { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };
			var pageSetup = new PageSetup { PaperSize = (UInt32Value)9U, Orientation = OrientationValues.Portrait, Id = "rId1" };

			worksheet.Append(sheetDimension);
			worksheet.Append(sheetViews);
			worksheet.Append(sheetFormatProperties);
			worksheet.Append(columns);
			worksheet.Append(sheetData);
			worksheet.Append(pageMargins);
			worksheet.Append(pageSetup);

			worksheetPart.Worksheet = worksheet;
		}

		private static void generateSpreadsheetPrinterSettingsPartContent(SpreadsheetPrinterSettingsPart spreadsheetPrinterSettingsPart)
		{
			using (var data = getBinaryDataStream(spreadsheetPrinterSettingsPartData))
			{
				spreadsheetPrinterSettingsPart.FeedData(data);
			}
		}

		private static void generateSharedStringTablePartContent(SharedStringTablePart sharedStringTablePart)
		{
			sharedStringTablePart.SharedStringTable = new SharedStringTable();
		}

		private static void setPackageProperties(OpenXmlPackage document)
		{
			document.PackageProperties.Creator = document.PackageProperties.LastModifiedBy = "Учётная система";
			document.PackageProperties.Created = document.PackageProperties.Modified = DateTime.Now;
		}

		#region Binary Data

		private const string spreadsheetPrinterSettingsPartData = "RgBvAHgAaQB0ACAAUgBlAGEAZABlAHIAIABQAEQARgAgAFAAcgBpAG4AdABlAHIAAAAAAAAAAAAAAAAAAAAAAAEEAQTcAAAAX/+BBwEACQCaCzQIZAABAAcAWAICAAEAWAICAAAAQQA0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAAAAAACAAAAAQAAAAEAAAABAAAAAAAAAAAAAAAAAAAAAAAAAA==";

		private static Stream getBinaryDataStream(string base64String)
		{
			return new MemoryStream(Convert.FromBase64String(base64String));
		}

		#endregion
	}
}

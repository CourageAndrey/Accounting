using System;
using System.IO;
using System.Linq;

using DocumentFormat.OpenXml.Packaging;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using Vt = DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using X15ac = DocumentFormat.OpenXml.Office2013.ExcelAc;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using A = DocumentFormat.OpenXml.Drawing;
using Border = DocumentFormat.OpenXml.Spreadsheet.Border;
using Thm15 = DocumentFormat.OpenXml.Office2013.Theme;

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
			var properties1 = new Ap.Properties();
			properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
			var application1 = new Ap.Application();
			application1.Text = "Microsoft Excel";
			var documentSecurity1 = new Ap.DocumentSecurity();
			documentSecurity1.Text = "0";
			var scaleCrop1 = new Ap.ScaleCrop();
			scaleCrop1.Text = "false";

			var headingPairs1 = new Ap.HeadingPairs();

			var vTVector1 = new Vt.VTVector() { BaseType = Vt.VectorBaseValues.Variant, Size = (UInt32Value)2U };

			var variant1 = new Vt.Variant();
			var vTLPSTR1 = new Vt.VTLPSTR();
			vTLPSTR1.Text = "Листы";

			variant1.Append(vTLPSTR1);

			var variant2 = new Vt.Variant();
			var vTInt321 = new Vt.VTInt32();
			vTInt321.Text = "1";

			variant2.Append(vTInt321);

			vTVector1.Append(variant1);
			vTVector1.Append(variant2);

			headingPairs1.Append(vTVector1);

			var titlesOfParts1 = new Ap.TitlesOfParts();

			var vTVector2 = new Vt.VTVector() { BaseType = Vt.VectorBaseValues.Lpstr, Size = (UInt32Value)1U };
			var vTLPSTR2 = new Vt.VTLPSTR();
			vTLPSTR2.Text = "Лист1";

			vTVector2.Append(vTLPSTR2);

			titlesOfParts1.Append(vTVector2);
			var company1 = new Ap.Company();
			company1.Text = "";
			var linksUpToDate1 = new Ap.LinksUpToDate();
			linksUpToDate1.Text = "false";
			var sharedDocument1 = new Ap.SharedDocument();
			sharedDocument1.Text = "false";
			var hyperlinksChanged1 = new Ap.HyperlinksChanged();
			hyperlinksChanged1.Text = "false";
			var applicationVersion1 = new Ap.ApplicationVersion();
			applicationVersion1.Text = "15.0300";

			properties1.Append(application1);
			properties1.Append(documentSecurity1);
			properties1.Append(scaleCrop1);
			properties1.Append(headingPairs1);
			properties1.Append(titlesOfParts1);
			properties1.Append(company1);
			properties1.Append(linksUpToDate1);
			properties1.Append(sharedDocument1);
			properties1.Append(hyperlinksChanged1);
			properties1.Append(applicationVersion1);

			extendedFilePropertiesPart.Properties = properties1;
		}

		private static void generateWorkbookPartContent(WorkbookPart workbookPart)
		{
			var workbook1 = new Workbook() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x15" } };
			workbook1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			workbook1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			workbook1.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
			var fileVersion1 = new FileVersion() { ApplicationName = "xl", LastEdited = "6", LowestEdited = "6", BuildVersion = "14420" };
			var workbookProperties1 = new WorkbookProperties() { DefaultThemeVersion = (UInt32Value)153222U };

			var alternateContent1 = new AlternateContent();
			alternateContent1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");

			var alternateContentChoice1 = new AlternateContentChoice() { Requires = "x15" };

			var absolutePath1 = new X15ac.AbsolutePath() { Url = "D:\\Current\\" };
			absolutePath1.AddNamespaceDeclaration("x15ac", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/ac");

			alternateContentChoice1.Append(absolutePath1);

			alternateContent1.Append(alternateContentChoice1);

			var bookViews1 = new BookViews();
			var workbookView1 = new WorkbookView() { XWindow = 0, YWindow = 0, WindowWidth = (UInt32Value)28800U, WindowHeight = (UInt32Value)12435U };

			bookViews1.Append(workbookView1);

			var sheets1 = new Sheets();
			var sheet1 = new Sheet() { Name = "Лист1", SheetId = (UInt32Value)1U, Id = "rId1" };

			sheets1.Append(sheet1);
			var calculationProperties1 = new CalculationProperties() { CalculationId = (UInt32Value)152511U, ReferenceMode = ReferenceModeValues.R1C1 };

			var workbookExtensionList1 = new WorkbookExtensionList();

			var workbookExtension1 = new WorkbookExtension() { Uri = "{140A7094-0E35-4892-8432-C4D2E57EDEB5}" };
			workbookExtension1.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
			var workbookProperties2 = new X15.WorkbookProperties() { ChartTrackingReferenceBase = true };

			workbookExtension1.Append(workbookProperties2);

			workbookExtensionList1.Append(workbookExtension1);

			workbook1.Append(fileVersion1);
			workbook1.Append(workbookProperties1);
			workbook1.Append(alternateContent1);
			workbook1.Append(bookViews1);
			workbook1.Append(sheets1);
			workbook1.Append(calculationProperties1);
			workbook1.Append(workbookExtensionList1);

			workbookPart.Workbook = workbook1;
		}

		private static void generateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart)
		{
			var stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
			stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

			var fonts1 = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

			var font1 = new Font();
			var fontSize1 = new FontSize() { Val = 11D };
			var color1 = new Color() { Theme = (UInt32Value)1U };
			var fontName1 = new FontName() { Val = "Calibri" };
			var fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
			var fontCharSet1 = new FontCharSet() { Val = 204 };
			var fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

			font1.Append(fontSize1);
			font1.Append(color1);
			font1.Append(fontName1);
			font1.Append(fontFamilyNumbering1);
			font1.Append(fontCharSet1);
			font1.Append(fontScheme1);

			var font2 = new Font();
			var bold1 = new Bold();
			var fontSize2 = new FontSize() { Val = 11D };
			var color2 = new Color() { Theme = (UInt32Value)1U };
			var fontName2 = new FontName() { Val = "Calibri" };
			var fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
			var fontCharSet2 = new FontCharSet() { Val = 204 };
			var fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

			font2.Append(bold1);
			font2.Append(fontSize2);
			font2.Append(color2);
			font2.Append(fontName2);
			font2.Append(fontFamilyNumbering2);
			font2.Append(fontCharSet2);
			font2.Append(fontScheme2);

			fonts1.Append(font1);
			fonts1.Append(font2);

			var fills1 = new Fills() { Count = (UInt32Value)2U };

			var fill1 = new Fill();
			var patternFill1 = new PatternFill() { PatternType = PatternValues.None };

			fill1.Append(patternFill1);

			var fill2 = new Fill();
			var patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

			fill2.Append(patternFill2);

			fills1.Append(fill1);
			fills1.Append(fill2);

			var borders1 = new Borders() { Count = (UInt32Value)2U };

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

			var leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
			var color3 = new Color() { Indexed = (UInt32Value)64U };

			leftBorder2.Append(color3);

			var rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
			var color4 = new Color() { Indexed = (UInt32Value)64U };

			rightBorder2.Append(color4);

			var topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
			var color5 = new Color() { Indexed = (UInt32Value)64U };

			topBorder2.Append(color5);

			var bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
			var color6 = new Color() { Indexed = (UInt32Value)64U };

			bottomBorder2.Append(color6);
			var diagonalBorder2 = new DiagonalBorder();

			border2.Append(leftBorder2);
			border2.Append(rightBorder2);
			border2.Append(topBorder2);
			border2.Append(bottomBorder2);
			border2.Append(diagonalBorder2);

			borders1.Append(border1);
			borders1.Append(border2);

			var cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
			var cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

			cellStyleFormats1.Append(cellFormat1);

			var cellFormats1 = new CellFormats() { Count = (UInt32Value)4U };
			var cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

			var cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true, ApplyAlignment = true };
			var alignment1 = new Alignment() { Vertical = VerticalAlignmentValues.Center, WrapText = true };

			cellFormat3.Append(alignment1);
			var cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };
			var cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true };

			cellFormats1.Append(cellFormat2);
			cellFormats1.Append(cellFormat3);
			cellFormats1.Append(cellFormat4);
			cellFormats1.Append(cellFormat5);

			var cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
			var cellStyle1 = new CellStyle() { Name = "Обычный", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

			cellStyles1.Append(cellStyle1);
			var differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
			var tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

			var stylesheetExtensionList1 = new StylesheetExtensionList();

			var stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
			stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
			var slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

			stylesheetExtension1.Append(slicerStyles1);

			var stylesheetExtension2 = new StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
			stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
			var timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

			stylesheetExtension2.Append(timelineStyles1);

			stylesheetExtensionList1.Append(stylesheetExtension1);
			stylesheetExtensionList1.Append(stylesheetExtension2);

			stylesheet1.Append(fonts1);
			stylesheet1.Append(fills1);
			stylesheet1.Append(borders1);
			stylesheet1.Append(cellStyleFormats1);
			stylesheet1.Append(cellFormats1);
			stylesheet1.Append(cellStyles1);
			stylesheet1.Append(differentialFormats1);
			stylesheet1.Append(tableStyles1);
			stylesheet1.Append(stylesheetExtensionList1);

			workbookStylesPart.Stylesheet = stylesheet1;
		}

		private static void generateThemePartContent(ThemePart themePart)
		{
			var theme1 = new A.Theme() { Name = "Тема Office" };
			theme1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

			var themeElements1 = new A.ThemeElements();

			var colorScheme1 = new A.ColorScheme() { Name = "Стандартная" };

			var dark1Color1 = new A.Dark1Color();
			var systemColor1 = new A.SystemColor() { Val = A.SystemColorValues.WindowText, LastColor = "000000" };

			dark1Color1.Append(systemColor1);

			var light1Color1 = new A.Light1Color();
			var systemColor2 = new A.SystemColor() { Val = A.SystemColorValues.Window, LastColor = "FFFFFF" };

			light1Color1.Append(systemColor2);

			var dark2Color1 = new A.Dark2Color();
			var rgbColorModelHex1 = new A.RgbColorModelHex() { Val = "44546A" };

			dark2Color1.Append(rgbColorModelHex1);

			var light2Color1 = new A.Light2Color();
			var rgbColorModelHex2 = new A.RgbColorModelHex() { Val = "E7E6E6" };

			light2Color1.Append(rgbColorModelHex2);

			var accent1Color1 = new A.Accent1Color();
			var rgbColorModelHex3 = new A.RgbColorModelHex() { Val = "5B9BD5" };

			accent1Color1.Append(rgbColorModelHex3);

			var accent2Color1 = new A.Accent2Color();
			var rgbColorModelHex4 = new A.RgbColorModelHex() { Val = "ED7D31" };

			accent2Color1.Append(rgbColorModelHex4);

			var accent3Color1 = new A.Accent3Color();
			var rgbColorModelHex5 = new A.RgbColorModelHex() { Val = "A5A5A5" };

			accent3Color1.Append(rgbColorModelHex5);

			var accent4Color1 = new A.Accent4Color();
			var rgbColorModelHex6 = new A.RgbColorModelHex() { Val = "FFC000" };

			accent4Color1.Append(rgbColorModelHex6);

			var accent5Color1 = new A.Accent5Color();
			var rgbColorModelHex7 = new A.RgbColorModelHex() { Val = "4472C4" };

			accent5Color1.Append(rgbColorModelHex7);

			var accent6Color1 = new A.Accent6Color();
			var rgbColorModelHex8 = new A.RgbColorModelHex() { Val = "70AD47" };

			accent6Color1.Append(rgbColorModelHex8);

			var hyperlink1 = new A.Hyperlink();
			var rgbColorModelHex9 = new A.RgbColorModelHex() { Val = "0563C1" };

			hyperlink1.Append(rgbColorModelHex9);

			var followedHyperlinkColor1 = new A.FollowedHyperlinkColor();
			var rgbColorModelHex10 = new A.RgbColorModelHex() { Val = "954F72" };

			followedHyperlinkColor1.Append(rgbColorModelHex10);

			colorScheme1.Append(dark1Color1);
			colorScheme1.Append(light1Color1);
			colorScheme1.Append(dark2Color1);
			colorScheme1.Append(light2Color1);
			colorScheme1.Append(accent1Color1);
			colorScheme1.Append(accent2Color1);
			colorScheme1.Append(accent3Color1);
			colorScheme1.Append(accent4Color1);
			colorScheme1.Append(accent5Color1);
			colorScheme1.Append(accent6Color1);
			colorScheme1.Append(hyperlink1);
			colorScheme1.Append(followedHyperlinkColor1);

			var fontScheme3 = new A.FontScheme() { Name = "Стандартная" };

			var majorFont1 = new A.MajorFont();
			var latinFont1 = new A.LatinFont() { Typeface = "Calibri Light", Panose = "020F0302020204030204" };
			var eastAsianFont1 = new A.EastAsianFont() { Typeface = "" };
			var complexScriptFont1 = new A.ComplexScriptFont() { Typeface = "" };
			var supplementalFont1 = new A.SupplementalFont() { Script = "Jpan", Typeface = "ＭＳ Ｐゴシック" };
			var supplementalFont2 = new A.SupplementalFont() { Script = "Hang", Typeface = "맑은 고딕" };
			var supplementalFont3 = new A.SupplementalFont() { Script = "Hans", Typeface = "宋体" };
			var supplementalFont4 = new A.SupplementalFont() { Script = "Hant", Typeface = "新細明體" };
			var supplementalFont5 = new A.SupplementalFont() { Script = "Arab", Typeface = "Times New Roman" };
			var supplementalFont6 = new A.SupplementalFont() { Script = "Hebr", Typeface = "Times New Roman" };
			var supplementalFont7 = new A.SupplementalFont() { Script = "Thai", Typeface = "Tahoma" };
			var supplementalFont8 = new A.SupplementalFont() { Script = "Ethi", Typeface = "Nyala" };
			var supplementalFont9 = new A.SupplementalFont() { Script = "Beng", Typeface = "Vrinda" };
			var supplementalFont10 = new A.SupplementalFont() { Script = "Gujr", Typeface = "Shruti" };
			var supplementalFont11 = new A.SupplementalFont() { Script = "Khmr", Typeface = "MoolBoran" };
			var supplementalFont12 = new A.SupplementalFont() { Script = "Knda", Typeface = "Tunga" };
			var supplementalFont13 = new A.SupplementalFont() { Script = "Guru", Typeface = "Raavi" };
			var supplementalFont14 = new A.SupplementalFont() { Script = "Cans", Typeface = "Euphemia" };
			var supplementalFont15 = new A.SupplementalFont() { Script = "Cher", Typeface = "Plantagenet Cherokee" };
			var supplementalFont16 = new A.SupplementalFont() { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
			var supplementalFont17 = new A.SupplementalFont() { Script = "Tibt", Typeface = "Microsoft Himalaya" };
			var supplementalFont18 = new A.SupplementalFont() { Script = "Thaa", Typeface = "MV Boli" };
			var supplementalFont19 = new A.SupplementalFont() { Script = "Deva", Typeface = "Mangal" };
			var supplementalFont20 = new A.SupplementalFont() { Script = "Telu", Typeface = "Gautami" };
			var supplementalFont21 = new A.SupplementalFont() { Script = "Taml", Typeface = "Latha" };
			var supplementalFont22 = new A.SupplementalFont() { Script = "Syrc", Typeface = "Estrangelo Edessa" };
			var supplementalFont23 = new A.SupplementalFont() { Script = "Orya", Typeface = "Kalinga" };
			var supplementalFont24 = new A.SupplementalFont() { Script = "Mlym", Typeface = "Kartika" };
			var supplementalFont25 = new A.SupplementalFont() { Script = "Laoo", Typeface = "DokChampa" };
			var supplementalFont26 = new A.SupplementalFont() { Script = "Sinh", Typeface = "Iskoola Pota" };
			var supplementalFont27 = new A.SupplementalFont() { Script = "Mong", Typeface = "Mongolian Baiti" };
			var supplementalFont28 = new A.SupplementalFont() { Script = "Viet", Typeface = "Times New Roman" };
			var supplementalFont29 = new A.SupplementalFont() { Script = "Uigh", Typeface = "Microsoft Uighur" };
			var supplementalFont30 = new A.SupplementalFont() { Script = "Geor", Typeface = "Sylfaen" };

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

			var minorFont1 = new A.MinorFont();
			var latinFont2 = new A.LatinFont() { Typeface = "Calibri", Panose = "020F0502020204030204" };
			var eastAsianFont2 = new A.EastAsianFont() { Typeface = "" };
			var complexScriptFont2 = new A.ComplexScriptFont() { Typeface = "" };
			var supplementalFont31 = new A.SupplementalFont() { Script = "Jpan", Typeface = "ＭＳ Ｐゴシック" };
			var supplementalFont32 = new A.SupplementalFont() { Script = "Hang", Typeface = "맑은 고딕" };
			var supplementalFont33 = new A.SupplementalFont() { Script = "Hans", Typeface = "宋体" };
			var supplementalFont34 = new A.SupplementalFont() { Script = "Hant", Typeface = "新細明體" };
			var supplementalFont35 = new A.SupplementalFont() { Script = "Arab", Typeface = "Arial" };
			var supplementalFont36 = new A.SupplementalFont() { Script = "Hebr", Typeface = "Arial" };
			var supplementalFont37 = new A.SupplementalFont() { Script = "Thai", Typeface = "Tahoma" };
			var supplementalFont38 = new A.SupplementalFont() { Script = "Ethi", Typeface = "Nyala" };
			var supplementalFont39 = new A.SupplementalFont() { Script = "Beng", Typeface = "Vrinda" };
			var supplementalFont40 = new A.SupplementalFont() { Script = "Gujr", Typeface = "Shruti" };
			var supplementalFont41 = new A.SupplementalFont() { Script = "Khmr", Typeface = "DaunPenh" };
			var supplementalFont42 = new A.SupplementalFont() { Script = "Knda", Typeface = "Tunga" };
			var supplementalFont43 = new A.SupplementalFont() { Script = "Guru", Typeface = "Raavi" };
			var supplementalFont44 = new A.SupplementalFont() { Script = "Cans", Typeface = "Euphemia" };
			var supplementalFont45 = new A.SupplementalFont() { Script = "Cher", Typeface = "Plantagenet Cherokee" };
			var supplementalFont46 = new A.SupplementalFont() { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
			var supplementalFont47 = new A.SupplementalFont() { Script = "Tibt", Typeface = "Microsoft Himalaya" };
			var supplementalFont48 = new A.SupplementalFont() { Script = "Thaa", Typeface = "MV Boli" };
			var supplementalFont49 = new A.SupplementalFont() { Script = "Deva", Typeface = "Mangal" };
			var supplementalFont50 = new A.SupplementalFont() { Script = "Telu", Typeface = "Gautami" };
			var supplementalFont51 = new A.SupplementalFont() { Script = "Taml", Typeface = "Latha" };
			var supplementalFont52 = new A.SupplementalFont() { Script = "Syrc", Typeface = "Estrangelo Edessa" };
			var supplementalFont53 = new A.SupplementalFont() { Script = "Orya", Typeface = "Kalinga" };
			var supplementalFont54 = new A.SupplementalFont() { Script = "Mlym", Typeface = "Kartika" };
			var supplementalFont55 = new A.SupplementalFont() { Script = "Laoo", Typeface = "DokChampa" };
			var supplementalFont56 = new A.SupplementalFont() { Script = "Sinh", Typeface = "Iskoola Pota" };
			var supplementalFont57 = new A.SupplementalFont() { Script = "Mong", Typeface = "Mongolian Baiti" };
			var supplementalFont58 = new A.SupplementalFont() { Script = "Viet", Typeface = "Arial" };
			var supplementalFont59 = new A.SupplementalFont() { Script = "Uigh", Typeface = "Microsoft Uighur" };
			var supplementalFont60 = new A.SupplementalFont() { Script = "Geor", Typeface = "Sylfaen" };

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

			var formatScheme1 = new A.FormatScheme() { Name = "Стандартная" };

			var fillStyleList1 = new A.FillStyleList();

			var solidFill1 = new A.SolidFill();
			var schemeColor1 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

			solidFill1.Append(schemeColor1);

			var gradientFill1 = new A.GradientFill() { RotateWithShape = true };

			var gradientStopList1 = new A.GradientStopList();

			var gradientStop1 = new A.GradientStop() { Position = 0 };

			var schemeColor2 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var luminanceModulation1 = new A.LuminanceModulation() { Val = 110000 };
			var saturationModulation1 = new A.SaturationModulation() { Val = 105000 };
			var tint1 = new A.Tint() { Val = 67000 };

			schemeColor2.Append(luminanceModulation1);
			schemeColor2.Append(saturationModulation1);
			schemeColor2.Append(tint1);

			gradientStop1.Append(schemeColor2);

			var gradientStop2 = new A.GradientStop() { Position = 50000 };

			var schemeColor3 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var luminanceModulation2 = new A.LuminanceModulation() { Val = 105000 };
			var saturationModulation2 = new A.SaturationModulation() { Val = 103000 };
			var tint2 = new A.Tint() { Val = 73000 };

			schemeColor3.Append(luminanceModulation2);
			schemeColor3.Append(saturationModulation2);
			schemeColor3.Append(tint2);

			gradientStop2.Append(schemeColor3);

			var gradientStop3 = new A.GradientStop() { Position = 100000 };

			var schemeColor4 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var luminanceModulation3 = new A.LuminanceModulation() { Val = 105000 };
			var saturationModulation3 = new A.SaturationModulation() { Val = 109000 };
			var tint3 = new A.Tint() { Val = 81000 };

			schemeColor4.Append(luminanceModulation3);
			schemeColor4.Append(saturationModulation3);
			schemeColor4.Append(tint3);

			gradientStop3.Append(schemeColor4);

			gradientStopList1.Append(gradientStop1);
			gradientStopList1.Append(gradientStop2);
			gradientStopList1.Append(gradientStop3);
			var linearGradientFill1 = new A.LinearGradientFill() { Angle = 5400000, Scaled = false };

			gradientFill1.Append(gradientStopList1);
			gradientFill1.Append(linearGradientFill1);

			var gradientFill2 = new A.GradientFill() { RotateWithShape = true };

			var gradientStopList2 = new A.GradientStopList();

			var gradientStop4 = new A.GradientStop() { Position = 0 };

			var schemeColor5 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var saturationModulation4 = new A.SaturationModulation() { Val = 103000 };
			var luminanceModulation4 = new A.LuminanceModulation() { Val = 102000 };
			var tint4 = new A.Tint() { Val = 94000 };

			schemeColor5.Append(saturationModulation4);
			schemeColor5.Append(luminanceModulation4);
			schemeColor5.Append(tint4);

			gradientStop4.Append(schemeColor5);

			var gradientStop5 = new A.GradientStop() { Position = 50000 };

			var schemeColor6 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var saturationModulation5 = new A.SaturationModulation() { Val = 110000 };
			var luminanceModulation5 = new A.LuminanceModulation() { Val = 100000 };
			var shade1 = new A.Shade() { Val = 100000 };

			schemeColor6.Append(saturationModulation5);
			schemeColor6.Append(luminanceModulation5);
			schemeColor6.Append(shade1);

			gradientStop5.Append(schemeColor6);

			var gradientStop6 = new A.GradientStop() { Position = 100000 };

			var schemeColor7 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var luminanceModulation6 = new A.LuminanceModulation() { Val = 99000 };
			var saturationModulation6 = new A.SaturationModulation() { Val = 120000 };
			var shade2 = new A.Shade() { Val = 78000 };

			schemeColor7.Append(luminanceModulation6);
			schemeColor7.Append(saturationModulation6);
			schemeColor7.Append(shade2);

			gradientStop6.Append(schemeColor7);

			gradientStopList2.Append(gradientStop4);
			gradientStopList2.Append(gradientStop5);
			gradientStopList2.Append(gradientStop6);
			var linearGradientFill2 = new A.LinearGradientFill() { Angle = 5400000, Scaled = false };

			gradientFill2.Append(gradientStopList2);
			gradientFill2.Append(linearGradientFill2);

			fillStyleList1.Append(solidFill1);
			fillStyleList1.Append(gradientFill1);
			fillStyleList1.Append(gradientFill2);

			var lineStyleList1 = new A.LineStyleList();

			var outline1 = new A.Outline() { Width = 6350, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

			var solidFill2 = new A.SolidFill();
			var schemeColor8 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

			solidFill2.Append(schemeColor8);
			var presetDash1 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };
			var miter1 = new A.Miter() { Limit = 800000 };

			outline1.Append(solidFill2);
			outline1.Append(presetDash1);
			outline1.Append(miter1);

			var outline2 = new A.Outline() { Width = 12700, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

			var solidFill3 = new A.SolidFill();
			var schemeColor9 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

			solidFill3.Append(schemeColor9);
			var presetDash2 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };
			var miter2 = new A.Miter() { Limit = 800000 };

			outline2.Append(solidFill3);
			outline2.Append(presetDash2);
			outline2.Append(miter2);

			var outline3 = new A.Outline() { Width = 19050, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

			var solidFill4 = new A.SolidFill();
			var schemeColor10 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

			solidFill4.Append(schemeColor10);
			var presetDash3 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };
			var miter3 = new A.Miter() { Limit = 800000 };

			outline3.Append(solidFill4);
			outline3.Append(presetDash3);
			outline3.Append(miter3);

			lineStyleList1.Append(outline1);
			lineStyleList1.Append(outline2);
			lineStyleList1.Append(outline3);

			var effectStyleList1 = new A.EffectStyleList();

			var effectStyle1 = new A.EffectStyle();
			var effectList1 = new A.EffectList();

			effectStyle1.Append(effectList1);

			var effectStyle2 = new A.EffectStyle();
			var effectList2 = new A.EffectList();

			effectStyle2.Append(effectList2);

			var effectStyle3 = new A.EffectStyle();

			var effectList3 = new A.EffectList();

			var outerShadow1 = new A.OuterShadow() { BlurRadius = 57150L, Distance = 19050L, Direction = 5400000, Alignment = A.RectangleAlignmentValues.Center, RotateWithShape = false };

			var rgbColorModelHex11 = new A.RgbColorModelHex() { Val = "000000" };
			var alpha1 = new A.Alpha() { Val = 63000 };

			rgbColorModelHex11.Append(alpha1);

			outerShadow1.Append(rgbColorModelHex11);

			effectList3.Append(outerShadow1);

			effectStyle3.Append(effectList3);

			effectStyleList1.Append(effectStyle1);
			effectStyleList1.Append(effectStyle2);
			effectStyleList1.Append(effectStyle3);

			var backgroundFillStyleList1 = new A.BackgroundFillStyleList();

			var solidFill5 = new A.SolidFill();
			var schemeColor11 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

			solidFill5.Append(schemeColor11);

			var solidFill6 = new A.SolidFill();

			var schemeColor12 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var tint5 = new A.Tint() { Val = 95000 };
			var saturationModulation7 = new A.SaturationModulation() { Val = 170000 };

			schemeColor12.Append(tint5);
			schemeColor12.Append(saturationModulation7);

			solidFill6.Append(schemeColor12);

			var gradientFill3 = new A.GradientFill() { RotateWithShape = true };

			var gradientStopList3 = new A.GradientStopList();

			var gradientStop7 = new A.GradientStop() { Position = 0 };

			var schemeColor13 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var tint6 = new A.Tint() { Val = 93000 };
			var saturationModulation8 = new A.SaturationModulation() { Val = 150000 };
			var shade3 = new A.Shade() { Val = 98000 };
			var luminanceModulation7 = new A.LuminanceModulation() { Val = 102000 };

			schemeColor13.Append(tint6);
			schemeColor13.Append(saturationModulation8);
			schemeColor13.Append(shade3);
			schemeColor13.Append(luminanceModulation7);

			gradientStop7.Append(schemeColor13);

			var gradientStop8 = new A.GradientStop() { Position = 50000 };

			var schemeColor14 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var tint7 = new A.Tint() { Val = 98000 };
			var saturationModulation9 = new A.SaturationModulation() { Val = 130000 };
			var shade4 = new A.Shade() { Val = 90000 };
			var luminanceModulation8 = new A.LuminanceModulation() { Val = 103000 };

			schemeColor14.Append(tint7);
			schemeColor14.Append(saturationModulation9);
			schemeColor14.Append(shade4);
			schemeColor14.Append(luminanceModulation8);

			gradientStop8.Append(schemeColor14);

			var gradientStop9 = new A.GradientStop() { Position = 100000 };

			var schemeColor15 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
			var shade5 = new A.Shade() { Val = 63000 };
			var saturationModulation10 = new A.SaturationModulation() { Val = 120000 };

			schemeColor15.Append(shade5);
			schemeColor15.Append(saturationModulation10);

			gradientStop9.Append(schemeColor15);

			gradientStopList3.Append(gradientStop7);
			gradientStopList3.Append(gradientStop8);
			gradientStopList3.Append(gradientStop9);
			var linearGradientFill3 = new A.LinearGradientFill() { Angle = 5400000, Scaled = false };

			gradientFill3.Append(gradientStopList3);
			gradientFill3.Append(linearGradientFill3);

			backgroundFillStyleList1.Append(solidFill5);
			backgroundFillStyleList1.Append(solidFill6);
			backgroundFillStyleList1.Append(gradientFill3);

			formatScheme1.Append(fillStyleList1);
			formatScheme1.Append(lineStyleList1);
			formatScheme1.Append(effectStyleList1);
			formatScheme1.Append(backgroundFillStyleList1);

			themeElements1.Append(colorScheme1);
			themeElements1.Append(fontScheme3);
			themeElements1.Append(formatScheme1);
			var objectDefaults1 = new A.ObjectDefaults();
			var extraColorSchemeList1 = new A.ExtraColorSchemeList();

			var officeStyleSheetExtensionList1 = new A.OfficeStyleSheetExtensionList();

			var officeStyleSheetExtension1 = new A.OfficeStyleSheetExtension() { Uri = "{05A4C25C-085E-4340-85A3-A5531E510DB2}" };

			var themeFamily1 = new Thm15.ThemeFamily() { Name = "Office Theme", Id = "{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}", Vid = "{4A3C46E8-61CC-4603-A589-7422A47A8E4A}" };
			themeFamily1.AddNamespaceDeclaration("thm15", "http://schemas.microsoft.com/office/thememl/2012/main");

			officeStyleSheetExtension1.Append(themeFamily1);

			officeStyleSheetExtensionList1.Append(officeStyleSheetExtension1);

			theme1.Append(themeElements1);
			theme1.Append(objectDefaults1);
			theme1.Append(extraColorSchemeList1);
			theme1.Append(officeStyleSheetExtensionList1);

			themePart.Theme = theme1;
		}

		private static void generateWorksheetPartContent(WorksheetPart worksheetPart, IReport report)
		{
			var worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
			worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
			var sheetDimension1 = new SheetDimension() { Reference = "B2:E8" };

			var sheetViews1 = new SheetViews();

			var sheetView1 = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
			var selection1 = new Selection() { ActiveCell = "E9", SequenceOfReferences = new ListValue<StringValue>() { InnerText = "E9" } };

			sheetView1.Append(selection1);

			sheetViews1.Append(sheetView1);
			var sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

			var reportColumns = report.Descriptor.GetColumns().ToList();

			var columns1 = new Columns();
			for (uint c = 0; c < reportColumns.Count; c++)
			{
				columns1.Append(new Column { Min = c + 2U, Max = c + 2U, Width = (uint) reportColumns[(int) c].MinWidth / 5, CustomWidth = true });
			}

			var sheetData1 = new SheetData();

			var rowReportHeader = new Row { RowIndex = (UInt32Value)2U, Spans = new ListValue<StringValue>() { InnerText = "2:5" }, DyDescent = 0.25D };

			var cellReportHeader = new Cell { CellReference = "B2", StyleIndex = 3U, DataType = CellValues.String };
			cellReportHeader.Append(new CellValue { Text = report.Title });
			rowReportHeader.Append(cellReportHeader);
			sheetData1.Append(rowReportHeader);

			var rowTableHeader = new Row { RowIndex = 4U, Spans = new ListValue<StringValue> { InnerText = "2:5" }, DyDescent = 0.25D };
			for (int c = 0; c < reportColumns.Count; c++)
			{
				var headerCell = new Cell { CellReference = (char)('B' + c) + "4", StyleIndex = 2U, DataType = CellValues.String };
				headerCell.Append(new CellValue { Text = reportColumns[c].Header.ToString() });
				rowTableHeader.Append(headerCell);
			}
			sheetData1.Append(rowTableHeader);

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
				sheetData1.Append(row);
			}

			var pageMargins1 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };
			var pageSetup1 = new PageSetup() { PaperSize = (UInt32Value)9U, Orientation = OrientationValues.Portrait, Id = "rId1" };

			worksheet1.Append(sheetDimension1);
			worksheet1.Append(sheetViews1);
			worksheet1.Append(sheetFormatProperties1);
			worksheet1.Append(columns1);
			worksheet1.Append(sheetData1);
			worksheet1.Append(pageMargins1);
			worksheet1.Append(pageSetup1);

			worksheetPart.Worksheet = worksheet1;
		}

		private static void generateSpreadsheetPrinterSettingsPartContent(SpreadsheetPrinterSettingsPart spreadsheetPrinterSettingsPart)
		{
			var data = GetBinaryDataStream(spreadsheetPrinterSettingsPartData);
			spreadsheetPrinterSettingsPart.FeedData(data);
			data.Close();
		}

		private static void generateSharedStringTablePartContent(SharedStringTablePart sharedStringTablePart)
		{
			sharedStringTablePart.SharedStringTable = new SharedStringTable();
		}

		private static void setPackageProperties(OpenXmlPackage document)
		{
			document.PackageProperties.Creator = "Андрей Бычко";
			document.PackageProperties.Created = System.Xml.XmlConvert.ToDateTime("2017-12-17T04:45:44Z", System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
			document.PackageProperties.Modified = System.Xml.XmlConvert.ToDateTime("2017-12-17T04:48:26Z", System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
			document.PackageProperties.LastModifiedBy = "Андрей Бычко";
		}

		#region Binary Data
		private const string spreadsheetPrinterSettingsPartData = "RgBvAHgAaQB0ACAAUgBlAGEAZABlAHIAIABQAEQARgAgAFAAcgBpAG4AdABlAHIAAAAAAAAAAAAAAAAAAAAAAAEEAQTcAAAAX/+BBwEACQCaCzQIZAABAAcAWAICAAEAWAICAAAAQQA0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAAAAAACAAAAAQAAAAEAAAABAAAAAAAAAAAAAAAAAAAAAAAAAA==";

		private static Stream GetBinaryDataStream(string base64String)
		{
			return new MemoryStream(Convert.FromBase64String(base64String));
		}
		#endregion
	}
}

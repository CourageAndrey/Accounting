using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Accounting.Core.Application;
using Accounting.Core.Reports;
using Accounting.Reports.OpenXml.Helpers;

namespace Accounting.Reports.OpenXml
{
	[SuppressMessage("ReSharper", "PossiblyMistakenUseOfParamsMethod")]
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
				Size = 2U
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
				Size = 1U
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
			var workbookProperties1 = new WorkbookProperties { DefaultThemeVersion = 153222U };

			var alternateContent = new AlternateContent();
			alternateContent.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");

			var alternateContentChoice = new AlternateContentChoice { Requires = "x15" };

			var absolutePath = new DocumentFormat.OpenXml.Office2013.ExcelAc.AbsolutePath { Url = "D:\\Current\\" };
			absolutePath.AddNamespaceDeclaration("x15ac", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/ac");

			alternateContentChoice.Append(absolutePath);

			alternateContent.Append(alternateContentChoice);

			var bookViews = new BookViews();
			var workbookView = new WorkbookView { XWindow = 0, YWindow = 0, WindowWidth = 28800U, WindowHeight = 12435U };

			bookViews.Append(workbookView);

			var sheets = new Sheets();
			var sheet = new Sheet { Name = "Лист1", SheetId = 1U, Id = "rId1" };

			sheets.Append(sheet);
			var calculationProperties = new CalculationProperties { CalculationId = 152511U, ReferenceMode = ReferenceModeValues.R1C1 };

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

			var fonts = new Fonts
			{
				Count = 2U,
				KnownFonts = true
			};
			fonts.Append(StylesPartHelper.DefineFont());
			fonts.Append(StylesPartHelper.DefineFont(true));

			var fills = new Fills { Count = 2U };

			var fill1 = new Fill();
			var patternFill1 = new PatternFill { PatternType = PatternValues.None };

			fill1.Append(patternFill1);

			var fill2 = new Fill();
			var patternFill2 = new PatternFill { PatternType = PatternValues.Gray125 };

			fill2.Append(patternFill2);

			fills.Append(fill1);
			fills.Append(fill2);

			var borders = new Borders { Count = 2U };
			borders.Append(StylesPartHelper.DefineBorder(false));
			borders.Append(StylesPartHelper.DefineBorder(true));

			var cellStyleFormats = new CellStyleFormats { Count = 1U };
			var cellFormat1 = new CellFormat { NumberFormatId = 0U, FontId = 0U, FillId = 0U, BorderId = 0U };

			cellStyleFormats.Append(cellFormat1);

			var cellFormats = new CellFormats { Count = 4U };
			var cellFormat2 = new CellFormat { NumberFormatId = 0U, FontId = 0U, FillId = 0U, BorderId = 0U, FormatId = 0U };

			var cellFormat3 = new CellFormat { NumberFormatId = 0U, FontId = 0U, FillId = 0U, BorderId = 1U, FormatId = 0U, ApplyBorder = true, ApplyAlignment = true };
			var alignment1 = new Alignment { Vertical = VerticalAlignmentValues.Center, WrapText = true };

			cellFormat3.Append(alignment1);
			var cellFormat4 = new CellFormat { NumberFormatId = 0U, FontId = 1U, FillId = 0U, BorderId = 1U, FormatId = 0U, ApplyFont = true, ApplyBorder = true };
			var cellFormat5 = new CellFormat { NumberFormatId = 0U, FontId = 1U, FillId = 0U, BorderId = 0U, FormatId = 0U, ApplyFont = true };

			cellFormats.Append(cellFormat2);
			cellFormats.Append(cellFormat3);
			cellFormats.Append(cellFormat4);
			cellFormats.Append(cellFormat5);

			var cellStyles = new CellStyles { Count = 1U };
			var cellStyle = new CellStyle { Name = "Обычный", FormatId = 0U, BuiltinId = 0U };

			cellStyles.Append(cellStyle);
			var differentialFormats = new DifferentialFormats { Count = 0U };
			var tableStyles = new TableStyles { Count = 0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

			var stylesheetExtensionList = new StylesheetExtensionList();
			stylesheetExtensionList.Append(StylesPartHelper.DefineStylesheetExtension(
				"{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}",
				"x14",
				"http://schemas.microsoft.com/office/spreadsheetml/2009/9/main",
				"SlicerStyleLight1"));
			stylesheetExtensionList.Append(StylesPartHelper.DefineStylesheetExtension(
				"{9260A510-F301-46a8-8635-F512D64BE5F5}",
				"x15",
				"http://schemas.microsoft.com/office/spreadsheetml/2010/11/main",
				"TimeSlicerStyleLight1"));

			stylesheet.Append(fonts);
			stylesheet.Append(fills);
			stylesheet.Append(borders);
			stylesheet.Append(cellStyleFormats);
			stylesheet.Append(cellFormats);
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
			colorScheme.Append(ThemePartHelper.DefineColorAsSystem<DocumentFormat.OpenXml.Drawing.Dark1Color>(DocumentFormat.OpenXml.Drawing.SystemColorValues.WindowText, "000000"));
			colorScheme.Append(ThemePartHelper.DefineColorAsSystem<DocumentFormat.OpenXml.Drawing.Light1Color>(DocumentFormat.OpenXml.Drawing.SystemColorValues.Window, "FFFFFF"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Dark2Color>("44546A"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Light2Color>("E7E6E6"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Accent1Color>("5B9BD5"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Accent2Color>("ED7D31"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Accent3Color>("A5A5A5"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Accent4Color>("FFC000"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Accent5Color>("4472C4"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Accent6Color>("70AD47"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.Hyperlink>("0563C1"));
			colorScheme.Append(ThemePartHelper.DefineColorAsRgb<DocumentFormat.OpenXml.Drawing.FollowedHyperlinkColor>("954F72"));

			var fontScheme = new DocumentFormat.OpenXml.Drawing.FontScheme { Name = "Стандартная" };

			var majorFont = new DocumentFormat.OpenXml.Drawing.MajorFont();
			majorFont.DefineFontScripts(
				"Calibri Light",
				"020F0302020204030204",
				FontCollectionTypeHelper.MajorSupplementalFonts);

			var minorFont = new DocumentFormat.OpenXml.Drawing.MinorFont();
			minorFont.DefineFontScripts(
				"Calibri",
				"020F0502020204030204",
				FontCollectionTypeHelper.MinorSupplementalFonts);

			fontScheme.Append(majorFont);
			fontScheme.Append(minorFont);

			var formatScheme = new DocumentFormat.OpenXml.Drawing.FormatScheme { Name = "Стандартная" };

			var fillStyleList = new DocumentFormat.OpenXml.Drawing.FillStyleList();

			var solidFill = new DocumentFormat.OpenXml.Drawing.SolidFill();
			var schemeColor1 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };

			solidFill.Append(schemeColor1);

			var gradientFill = new DocumentFormat.OpenXml.Drawing.GradientFill { RotateWithShape = true };

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

			gradientFill.Append(gradientStopList1);
			gradientFill.Append(linearGradientFill1);

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

			fillStyleList.Append(solidFill);
			fillStyleList.Append(gradientFill);
			fillStyleList.Append(gradientFill2);

			var lineStyleList = new DocumentFormat.OpenXml.Drawing.LineStyleList();

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

			lineStyleList.Append(outline1);
			lineStyleList.Append(outline2);
			lineStyleList.Append(outline3);

			var effectStyleList = new DocumentFormat.OpenXml.Drawing.EffectStyleList();

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

			effectStyleList.Append(effectStyle1);
			effectStyleList.Append(effectStyle2);
			effectStyleList.Append(effectStyle3);

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
			formatScheme.Append(lineStyleList);
			formatScheme.Append(effectStyleList);
			formatScheme.Append(backgroundFillStyleList1);

			themeElements.Append(colorScheme);
			themeElements.Append(fontScheme);
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

			var sheetView = new SheetView { TabSelected = true, WorkbookViewId = 0U };
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

			var rowReportHeader = new Row { RowIndex = 2U, Spans = new ListValue<StringValue> { InnerText = "2:5" }, DyDescent = 0.25D };

			var cellReportHeader = new Cell { CellReference = "B2", StyleIndex = 3U, DataType = CellValues.String };
			cellReportHeader.Append(new CellValue { Text = report.Title });
			rowReportHeader.Append(cellReportHeader);
			sheetData.Append(rowReportHeader);

			var rowTableHeader = new Row { RowIndex = 4U, Spans = new ListValue<StringValue> { InnerText = "2:5" }, DyDescent = 0.25D };
			for (int c = 0; c < reportColumns.Count; c++)
			{
				var headerCell = new Cell { CellReference = (char)('B' + c) + "4", StyleIndex = 2U, DataType = CellValues.String };
				headerCell.Append(new CellValue { Text = reportColumns[c].Header });
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
			var pageSetup = new PageSetup { PaperSize = 9U, Orientation = OrientationValues.Portrait, Id = "rId1" };

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

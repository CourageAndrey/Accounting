﻿using System;
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
			fontScheme.Append(FontCollectionTypeHelper.DefineFontCollectionType<DocumentFormat.OpenXml.Drawing.MajorFont>(
				"Calibri Light",
				"020F0302020204030204",
				FontCollectionTypeHelper.MajorSupplementalFonts));
			fontScheme.Append(FontCollectionTypeHelper.DefineFontCollectionType<DocumentFormat.OpenXml.Drawing.MinorFont>(
				"Calibri",
				"020F0502020204030204",
				FontCollectionTypeHelper.MinorSupplementalFonts));

			var formatScheme = new DocumentFormat.OpenXml.Drawing.FormatScheme { Name = "Стандартная" };

			var fillStyleList = new DocumentFormat.OpenXml.Drawing.FillStyleList();

			var solidFill = new DocumentFormat.OpenXml.Drawing.SolidFill();
			var schemeColor1 = new DocumentFormat.OpenXml.Drawing.SchemeColor { Val = DocumentFormat.OpenXml.Drawing.SchemeColorValues.PhColor };

			solidFill.Append(schemeColor1);

			var gradientFill = new DocumentFormat.OpenXml.Drawing.GradientFill { RotateWithShape = true };

			var gradientStopList1 = new DocumentFormat.OpenXml.Drawing.GradientStopList();
			gradientStopList1.Append(ThemePartHelper.DefineGradientStop(0, new PercentageValue[] {
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(110000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(105000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Tint>(67000) }));
			gradientStopList1.Append(ThemePartHelper.DefineGradientStop(50000, new PercentageValue[] {
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(105000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(103000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Tint>(73000) }));
			gradientStopList1.Append(ThemePartHelper.DefineGradientStop(100000, new PercentageValue[] {
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(105000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(109000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Tint>(81000) }));

			var linearGradientFill1 = new DocumentFormat.OpenXml.Drawing.LinearGradientFill { Angle = 5400000, Scaled = false };

			gradientFill.Append(gradientStopList1);
			gradientFill.Append(linearGradientFill1);

			var gradientFill2 = new DocumentFormat.OpenXml.Drawing.GradientFill { RotateWithShape = true };

			var gradientStopList2 = new DocumentFormat.OpenXml.Drawing.GradientStopList();
			gradientStopList2.Append(ThemePartHelper.DefineGradientStop(0, new PercentageValue[] {
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(103000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(102000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Tint>(94000) }));
			gradientStopList2.Append(ThemePartHelper.DefineGradientStop(50000, new PercentageValue[] {
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(110000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(100000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Shade>(100000) }));
			gradientStopList2.Append(ThemePartHelper.DefineGradientStop(100000, new PercentageValue[] {
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(99000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(120000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Shade>(78000) }));

			var linearGradientFill2 = new DocumentFormat.OpenXml.Drawing.LinearGradientFill { Angle = 5400000, Scaled = false };

			gradientFill2.Append(gradientStopList2);
			gradientFill2.Append(linearGradientFill2);

			fillStyleList.Append(solidFill);
			fillStyleList.Append(gradientFill);
			fillStyleList.Append(gradientFill2);

			var lineStyleList = new DocumentFormat.OpenXml.Drawing.LineStyleList();
			lineStyleList.Append(ThemePartHelper.DefineOutline(6350));
			lineStyleList.Append(ThemePartHelper.DefineOutline(12700));
			lineStyleList.Append(ThemePartHelper.DefineOutline(19050));

			var effectStyleList = new DocumentFormat.OpenXml.Drawing.EffectStyleList();
			effectStyleList.Append(ThemePartHelper.DefineEffectStyle());
			effectStyleList.Append(ThemePartHelper.DefineEffectStyle());
			effectStyleList.Append(ThemePartHelper.DefineEffectStyle(ThemePartHelper.DefineOuterShadow(
				57150L,
				19050L,
				5400000,
				DocumentFormat.OpenXml.Drawing.RectangleAlignmentValues.Center,
				false,
				"000000",
				63000)));

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
			gradientStopList3.Append(ThemePartHelper.DefineGradientStop(0, new PercentageValue[] {
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Tint>(93000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(150000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Shade>(98000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(102000) }));
			gradientStopList3.Append(ThemePartHelper.DefineGradientStop(50000, new PercentageValue[] {
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Tint>(98000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(130000),
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Shade>(90000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.LuminanceModulation>(103000) }));
			gradientStopList3.Append(ThemePartHelper.DefineGradientStop(100000, new PercentageValue[] {
				new PositiveFixedPercentageValue<DocumentFormat.OpenXml.Drawing.Shade>(63000),
				new PercentageValue<DocumentFormat.OpenXml.Drawing.SaturationModulation>(120000) }));

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

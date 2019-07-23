using System.Windows;

using ComfortIsland.Helpers;
using ComfortIsland.Reports;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace ComfortIsland.Controls
{
	public partial class ReportControl
	{
		public ReportControl()
		{
			InitializeComponent();
		}

		private IReport report;

		public IReport Report
		{
			get { return report; }
			set
			{
				report = value;

				itemsGrid.ItemsSource = null;
				itemsGrid.Columns.Clear();

				if (report != null)
				{
					header.Text = report.Title;

					foreach (var column in report.Descriptor.GetColumns())
					{
						itemsGrid.Columns.Add(column);
					}
					itemsGrid.ItemsSource = report.Items;

					buttonPrint.IsEnabled = true;
				}
				else
				{
					header.Text = string.Empty;

					buttonPrint.IsEnabled = false;
				}
			}
		}

		private static readonly Microsoft.Win32.SaveFileDialog _saveDialog = ExcelHelper.CreateSaveDialog();

		private void printClick(object sender, RoutedEventArgs e)
		{
			if (_saveDialog.ShowDialog() == true)
			{
				using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(_saveDialog.FileName, SpreadsheetDocumentType.Workbook))
				{
					ExcelHelper.ExportReport(spreadsheet, header.Text, itemsGrid);
				}
				_saveDialog.FileName.ShellOpen();
			}
		}
	}
}

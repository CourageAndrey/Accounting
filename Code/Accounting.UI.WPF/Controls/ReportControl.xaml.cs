using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ComfortIsland.Helpers;
using ComfortIsland.Reports;

using Microsoft.Win32;

namespace ComfortIsland.Controls
{
	public partial class ReportControl : IAccountingApplicationClient
	{
		public void ConnectTo(IAccountingApplication application)
		{
			_application = application;
		}

		private IAccountingApplication _application;

		public ReportControl()
		{
			InitializeComponent();
		}

		private IReport _report;

		public IReport Report
		{
			get { return _report; }
			set
			{
				_report = value;

				itemsGrid.ItemsSource = null;
				itemsGrid.Columns.Clear();

				if (_report != null)
				{
					header.Text = _report.Title;

					foreach (var column in _report.Descriptor.GetColumns())
					{
						itemsGrid.Columns.Add(convertColumn(column));
					}
					itemsGrid.ItemsSource = _report.Items;

					buttonPrint.IsEnabled = true;
				}
				else
				{
					header.Text = string.Empty;

					buttonPrint.IsEnabled = false;
				}
			}
		}

		private static DataGridTextColumn convertColumn(ReportColumn reportColumn)
		{
			var column = new DataGridTextColumn
			{
				Header = reportColumn.Header,
				Binding = bindAndSetStyle(reportColumn.Binding, reportColumn.NeedsDigitRounding),
				MinWidth = reportColumn.MinWidth,
			};
			if (reportColumn.NeedsDigitRounding)
			{
				setNumberStyle(column);
			}
			return column;
		}

		private static Binding bindAndSetStyle(string propertyPath, bool needsDigitRounding)
		{
			var binding = new Binding
			{
				Path = new PropertyPath(propertyPath),
				Mode = BindingMode.OneTime,
			};
			if (needsDigitRounding)
			{
				binding.Converter = DigitRoundingConverter.Instance;
			}
			return binding;
		}

		private static void setNumberStyle(DataGridColumn column)
		{
			column.CellStyle = column.CellStyle ?? new Style();
			column.CellStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right));
		}

		private void printClick(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog
			{
				Title = "Выберите имя файла для сохранения...",
				Filter = _application.ReportExporter.SaveFileDialogFilter,
			};
			if (saveDialog.ShowDialog() == true)
			{
				_application.ReportExporter.ExportReport(_report, saveDialog.FileName);
				saveDialog.FileName.ShellOpen();
			}
		}
	}
}

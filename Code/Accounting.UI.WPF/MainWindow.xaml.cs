using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Accounting.Core.Application;
using Accounting.Core.BusinessLogic;
using Accounting.Core.Helpers;
using Accounting.Core.Reports;
using Accounting.UI.WPF.Dialogs;

namespace Accounting.UI.WPF
{
	public partial class MainWindow : IAccountingApplicationClient
	{
		#region Инициализация

		public MainWindow()
		{
			InitializeComponent();

			_documentsViewSource = (CollectionViewSource) Resources["documentsCollectionViewSource"];
		}

		public void ConnectTo(IAccountingApplication application)
		{
			_application = (WpfAccountingApplication) application;

			reportControl.ConnectTo(application);

			refBookControlUnits.ConnectTo(application);
			refBookControlUnits.Initialize(typeof(Unit), new DataGridColumn[]
			{
				new DataGridTextColumn
				{
					Header = "Название",
					Binding = new Binding("Name") { Mode = BindingMode.OneTime },
					MinWidth = 200,
				},
				new DataGridTextColumn
				{
					Header = "Сокращение",
					Binding = new Binding("ShortName") { Mode = BindingMode.OneTime },
					MinWidth = 100,
				},
			});

			refBookControlProducts.ConnectTo(application);
			refBookControlProducts.Initialize(typeof(Product), new DataGridColumn[]
			{
				new DataGridTextColumn
				{
					Header = "Наименование",
					Binding = new Binding("Name") { Mode = BindingMode.OneTime },
					MinWidth = 300,
				},
				new DataGridTextColumn
				{
					Header = "Ед/изм",
					Binding = new Binding("Unit.Name") { Mode = BindingMode.OneTime },
					MinWidth = 50,
				},
			});

			FontSize = application.Settings.UserInterface.FontSize;
		}

		private WpfAccountingApplication _application;

		private void formLoaded(object sender, RoutedEventArgs e)
		{
			// документы
			stateColumn.Visibility = checkBoxShowObsoleteDocuments.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
			_documentsViewSource.Source = _application.Database.Documents;
			_documentsCollectionView = CollectionViewSource.GetDefaultView(documentsGrid.ItemsSource);
			documentsWeekClick(null, null);

			// отчёты
			listReports.ItemsSource = ReportDescriptor.All;
			reportControl.Report = null;

			// справочники
			reloadComplexProducts();
			documentTypesGrid.ItemsSource = DocumentType.All;
		}

		#endregion

		#region Работа с документами

		private readonly CollectionViewSource _documentsViewSource;
		private ICollectionView _documentsCollectionView;

		private void documentsFilter(object sender, FilterEventArgs e)
		{
			var document = (Document) e.Item;
			e.Accepted =	(documentsFromDatePicker.SelectedDate == null || document.Date.Date >= documentsFromDatePicker.SelectedDate.Value.Date) &&
							(documentsToDatePicker.SelectedDate == null || document.Date.Date <= documentsToDatePicker.SelectedDate.Value.Date) &&
							((checkBoxShowObsoleteDocuments.IsChecked == true) || document.State == DocumentState.Active);
		}

		private void incomeClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.Income);
		}

		private void outcomeClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.Outcome);
		}

		private void produceClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.Produce);
		}

		private void toWarehouseClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.ToWarehouse);
		}

		private void checkBalanceClick(object sender, RoutedEventArgs e)
		{
			var dialog = new SelectProductDialog { Owner = this };
			dialog.ConnectTo(_application);
			if (dialog.ShowDialog() == true)
			{
				var report = new ProductBalance(dialog.EditValue, _application.Database.Balance);
				LongTextDialog.Info(
					report.ToString(),
					"Товар - " + report.Product.DisplayMember);
			}
		}

		private void deleteDocumentsClick(object sender, RoutedEventArgs e)
		{
			var documentsToDelete = documentsGrid.SelectedItems.OfType<Document>().Where(doc => doc.State == DocumentState.Active).ToList();
			if (documentsToDelete.Count == 0)
			{
				MessageBox.Show("Не выбрано ни одного активного документа.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if (MessageBox.Show(
				string.Format(CultureInfo.InvariantCulture, "Действительно удалить {0} выбранных документов?", documentsToDelete.Count),
				"Вопрос",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return;
			}

			var errors = new StringBuilder();
			if (_application.Settings.BusinessLogic.BalanceValidationStrategy.VerifyDelete(_application.Database, documentsToDelete, errors))
			{
				foreach (var document in documentsToDelete)
				{
					document.MakeObsolete(_application.Database.Balance, DocumentState.Deleted);
				}
				_application.DatabaseDriver.Save(_application.Database);
				refreshDocuments();
				reportControl.Report = null;
			}
			else
			{
				MessageBox.Show(errors.ToString(), "Не удалось выполнить удаление документов", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void editDocumentClick(object sender, RoutedEventArgs e)
		{
			var instance = documentsGrid.SelectedItems.OfType<Document>().Single();
			var viewModel = new Accounting.UI.WPF.ViewModels.Document(instance);
			var dialog = new DocumentDialog { Owner = this };
			if (instance.Type == DocumentType.Produce)
			{
				dialog.ProductsGetter = db => db.Products.Where(p => p.Children.Count > 0);
			}
			dialog.ConnectTo(_application);
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					instance = viewModel.ConvertToBusinessLogic(_application.Database);
					_application.DatabaseDriver.Save(_application.Database);

					refreshDocuments();
					documentsGrid.SelectedItem = instance;
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				reportControl.Report = null;
			}
		}

		private void selectedDocumentsChanged(object sender, SelectionChangedEventArgs e)
		{
			var documents = documentsGrid.SelectedItems.OfType<Document>().ToList();
			deleteDocumentsButton.IsEnabled = documents.Count > 0 && documents.All(d => d.State == DocumentState.Active);
			editDocumentButton.IsEnabled = documents.Count == 1;
		}

		private void createDocument(DocumentType type)
		{
			var viewModel = new Accounting.UI.WPF.ViewModels.Document(type);
			var dialog = new DocumentDialog { Owner = this };
			if (type == DocumentType.Produce)
			{
				dialog.ProductsGetter = db => db.Products.Where(p => p.Children.Count > 0);
			}
			dialog.ConnectTo(_application);
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var instance = viewModel.ConvertToBusinessLogic(_application.Database);
					_application.DatabaseDriver.Save(_application.Database);
					refreshDocuments();
					documentsGrid.SelectedItem = instance;
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				reportControl.Report = null;
			}
		}

		private void documentsGridDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var selectedItem = documentsGrid.SelectedItems.OfType<Document>().FirstOrDefault();
			if (selectedItem != null)
			{
				var dialog = new DocumentDialog { Owner = this };
				dialog.SetReadOnly();
				dialog.ConnectTo(_application);
				dialog.EditValue = new Accounting.UI.WPF.ViewModels.Document(selectedItem);
				dialog.ShowDialog();
			}
		}

		private void documentStateFilterChecked(object sender, RoutedEventArgs e)
		{
			stateColumn.Visibility = checkBoxShowObsoleteDocuments.IsChecked == true
				? Visibility.Visible
				: Visibility.Collapsed;
			refreshDocuments();
		}

		private void documentsDateFilterChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_suppressDocumentRefresh)
			{
				refreshDocuments();
			}
		}

		private bool _suppressDocumentRefresh;

		private void refreshDocuments()
		{
			if (!_suppressDocumentRefresh)
			{
				_documentsCollectionView.Refresh();
			}
		}

		private void documentsTodayClick(object sender, RoutedEventArgs e)
		{
			var now = DateTime.Now;

			filterDocuments(
				now,
				now);
		}

		private void documentsWeekClick(object sender, RoutedEventArgs e)
		{
			DateTime beginDate = DateTime.Now.GetBeginOfWeek();
			filterDocuments(
				beginDate,
				beginDate.AddDays(6));
		}

		private void documentsMonthClick(object sender, RoutedEventArgs e)
		{
			var now = DateTime.Now;
			var beginDate = new DateTime(now.Year, now.Month, 1);

			filterDocuments(
				beginDate,
				beginDate.AddMonths(1).AddMilliseconds(-1));
		}

		private void documentsYearClick(object sender, RoutedEventArgs e)
		{
			var now = DateTime.Now;
			var beginDate = new DateTime(now.Year, 1, 1);

			filterDocuments(
				beginDate,
				beginDate.AddYears(1).AddMilliseconds(-1));
		}

		private void filterDocuments(DateTime fromDate, DateTime toDate)
		{
			_suppressDocumentRefresh = true;
			documentsFromDatePicker.SelectedDate = fromDate;
			documentsToDatePicker.SelectedDate = toDate;
			_suppressDocumentRefresh = false;
			refreshDocuments();
		}

		#endregion

		#region Работа со справочниками

		private void reloadComplexProducts()
		{
			treeViewComplexProducts.ItemsSource = _application.Database.Products.Where(p => p.Children.Count > 0).ToList();
		}

		private void refBookChanged(object sender, EventArgs e)
		{
			reloadComplexProducts();
		}

		#endregion

		private void newReportClick(object sender, MouseButtonEventArgs e)
		{
			var reportDescriptor = listReports.SelectedItem as ReportDescriptor;
			if (reportDescriptor != null)
			{
				IReport report;
				if (reportDescriptor.CreateReport(_application, out report))
				{
					reportControl.Report = report;
				}
			}
		}
	}
}

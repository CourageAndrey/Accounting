using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Dialogs;
using ComfortIsland.Helpers;
using ComfortIsland.Reports;

namespace ComfortIsland
{
	public partial class MainWindow : IApplicationClient
	{
		#region Инициализация

		public MainWindow()
		{
			InitializeComponent();

			_documentsViewSource = (CollectionViewSource) Resources["documentsCollectionViewSource"];
		}

		public void ConnectTo(IApplication application)
		{
			_application = application;
			FontSize = application.Settings.FontSize;
		}

		private IApplication _application;

		private void formLoaded(object sender, RoutedEventArgs e)
		{
			// подготовка таблиц к заполнению
			unitsGrid.Tag = _application.Database.Units;
			productsGrid.Tag = _application.Database.Products;

			// документы
			stateColumn.Visibility = checkBoxShowObsoleteDocuments.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
			_documentsViewSource.Source = _application.Database.Documents;
			_documentsCollectionView = CollectionViewSource.GetDefaultView(documentsGrid.ItemsSource);
			documentsWeekClick(null, null);

			// отчёты
			listReports.ItemsSource = ReportDescriptor.All;
			reportControl.Report = null;

			// справочники
			refreshGrid(productsGrid);
			reloadComplexProducts();
			refreshGrid(unitsGrid);
			documentTypesGrid.ItemsSource = DocumentType.AllTypes.Values;

			updateButtonsAvailability(productsGrid, buttonEditProduct, buttonDeleteProduct);
			updateButtonsAvailability(unitsGrid, buttonEditUnit, buttonDeleteUnit);
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
			var dialog = new SelectProductDialog();
			dialog.ConnectTo(_application);
			if (dialog.ShowDialog() == true)
			{
				var report = new ProductBalance(dialog.EditValue, _application.Database.Balance);
				MessageBox.Show(
					report.ToString(),
					"Товар - " + report.Product.DisplayMember,
					MessageBoxButton.OK,
					MessageBoxImage.Information);
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
			if (_application.Settings.BalanceValidationStrategy.VerifyDelete(_application.Database, documentsToDelete, errors))
			{
				foreach (var document in documentsToDelete)
				{
					document.MakeObsolete(_application.Database.Balance, DocumentState.Deleted);
				}
				_application.Settings.DatabaseDriver.Save(_application.Database);
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
			var viewModel = new ViewModels.Document(instance);
			var dialog = new DocumentDialog();
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
					_application.Settings.DatabaseDriver.Save(_application.Database);

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
			var viewModel = new ViewModels.Document(type);
			var dialog = new DocumentDialog();
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
					_application.Settings.DatabaseDriver.Save(_application.Database);
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
				var dialog = new DocumentDialog();
				dialog.SetReadOnly();
				dialog.ConnectTo(_application);
				dialog.EditValue = new ViewModels.Document(selectedItem);
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

		#region Товары

		private void reloadComplexProducts()
		{
			treeViewComplexProducts.ItemsSource = _application.Database.Products.Where(p => p.Children.Count > 0).ToList();
		}

		private void productAddClick(object sender, RoutedEventArgs e)
		{
			var viewModel = new ViewModels.Product();
			var dialog = new ProductDialog();
			dialog.ConnectTo(_application);
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var instance = viewModel.ConvertToBusinessLogic(_application.Database);
					_application.Settings.DatabaseDriver.Save(_application.Database);
					refreshGrid(productsGrid, instance);
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				reloadComplexProducts();
			}
		}

		private void productEditClick(object sender, RoutedEventArgs e)
		{
			var selectedItems = productsGrid.SelectedItems.OfType<Product>().ToList();
			if (selectedItems.Count > 0)
			{
				var instance = selectedItems[0];

				var message = instance.FindUsages(_application.Database);
				if (message.Length > 0 && MessageBox.Show(
						message.ToString(),
						"Редактирование приведёт к дополнительным изменениям. Продолжить?",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question) != MessageBoxResult.Yes)
				{
					return;
				}

				var viewModel = new ViewModels.Product(instance);
				var dialog = new ProductDialog();
				dialog.ConnectTo(_application);
				dialog.EditValue = viewModel;
				if (dialog.ShowDialog() == true)
				{
					try
					{
						instance = viewModel.ConvertToBusinessLogic(_application.Database);
						_application.Settings.DatabaseDriver.Save(_application.Database);
						refreshGrid(productsGrid, instance);
						reloadComplexProducts();
					}
					catch (Exception error)
					{
						MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		private void productDeleteClick(object sender, RoutedEventArgs e)
		{
			var selectedItems = productsGrid.SelectedItems.OfType<Product>().ToList();
			if (selectedItems.Count < 1) return;
			var message = new StringBuilder();
			foreach (var item in selectedItems)
			{
				message.Append(item.FindUsages(_application.Database));
			}
			if (message.Length > 0)
			{
				MessageBox.Show(
					message.ToString(),
					"Невозможно удалить выбранные сущности, так как они используются",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);
				return;
			}
			foreach (var item in selectedItems)
			{
				_application.Database.Products.Remove(item.ID);
			}
			_application.Settings.DatabaseDriver.Save(_application.Database);
			refreshGrid(productsGrid);
			reloadComplexProducts();
		}

		private void selectedProductsChanged(object sender, SelectionChangedEventArgs e)
		{
			updateButtonsAvailability(productsGrid, buttonEditProduct, buttonDeleteProduct);
		}

		#endregion

		#region Единицы измерения

		private void unitAddClick(object sender, RoutedEventArgs e)
		{
			var viewModel = new ViewModels.Unit();
			var dialog = new UnitDialog();
			dialog.ConnectTo(_application);
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var instance = viewModel.ConvertToBusinessLogic(_application.Database);
					_application.Settings.DatabaseDriver.Save(_application.Database);
					refreshGrid(unitsGrid, instance);
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void unitEditClick(object sender, RoutedEventArgs e)
		{
			var selectedItems = unitsGrid.SelectedItems.OfType<Unit>().ToList();
			if (selectedItems.Count > 0)
			{
				var instance = selectedItems[0];

				var message = instance.FindUsages(_application.Database);
				if (message.Length > 0 && MessageBox.Show(
					message.ToString(),
					"Редактирование приведёт к дополнительным изменениям. Продолжить?",
					MessageBoxButton.YesNo,
					MessageBoxImage.Question) != MessageBoxResult.Yes)
				{
					return;
				}

				var viewModel = new ViewModels.Unit(instance);
				var dialog = new UnitDialog();
				dialog.ConnectTo(_application);
				dialog.EditValue = viewModel;
				if (dialog.ShowDialog() == true)
				{
					try
					{
						instance = viewModel.ConvertToBusinessLogic(_application.Database);
						_application.Settings.DatabaseDriver.Save(_application.Database);
						refreshGrid(unitsGrid, instance);
					}
					catch (Exception error)
					{
						MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		private void unitDeleteClick(object sender, RoutedEventArgs e)
		{
			var selectedItems = unitsGrid.SelectedItems.OfType<Unit>().ToList();
			if (selectedItems.Count < 1) return;
			var message = new StringBuilder();
			foreach (var item in selectedItems)
			{
				message.Append(item.FindUsages(_application.Database));
			}
			if (message.Length > 0)
			{
				MessageBox.Show(
					message.ToString(),
					"Невозможно удалить выбранные сущности, так как они используются",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);
				return;
			}
			foreach (var item in selectedItems)
			{
				_application.Database.Units.Remove(item.ID);
			}
			_application.Settings.DatabaseDriver.Save(_application.Database);
			refreshGrid(unitsGrid);
		}

		private void selectedUnitsChanged(object sender, SelectionChangedEventArgs e)
		{
			updateButtonsAvailability(unitsGrid, buttonEditUnit, buttonDeleteUnit);
		}

		#endregion

		#endregion

		private void refreshGrid(DataGrid grid, object selectedItem = null)
		{
			grid.ItemsSource = null;
			grid.ItemsSource = (IEnumerable) grid.Tag;
			if (selectedItem != null)
			{
				grid.SelectedItem = selectedItem;
			}
		}

		private void updateButtonsAvailability(DataGrid grid, Button editButton, Button deleteButton)
		{
			int itemsCount = grid.SelectedItems != null
				? grid.SelectedItems.Count
				: (grid.SelectedItem != null ? 1 : 0);
			editButton.IsEnabled = itemsCount == 1;
			deleteButton.IsEnabled = itemsCount > 0;
		}

		private void newReportClick(object sender, MouseButtonEventArgs e)
		{
			var reportDescriptor = listReports.SelectedItem as ReportDescriptor;
			if (reportDescriptor != null)
			{
				IReport report;
				if (reportDescriptor.CreateReport(_application, _application.Database, out report))
				{
					reportControl.Report = report;
				}
			}
		}
	}
}

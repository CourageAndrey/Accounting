using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Dialogs;
using ComfortIsland.Helpers;
using ComfortIsland.Reports;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace ComfortIsland
{
	public partial class MainWindow
	{
		#region Инициализация

		public MainWindow()
		{
			InitializeComponent();
		}

		private Database _database;

		private void formLoaded(object sender, RoutedEventArgs e)
		{
			// вычитка базы данных
			var databaseXml = Xml.Database.TryLoad();
			_database = databaseXml.ConvertToBusinessLogic();

			// документы
			stateColumn.Visibility = checkBoxShowObsoleteDocuments.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
			documentsGrid.Columns[0].SortDirection = ListSortDirection.Ascending;
			documentsGrid.Items.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
			documentsWeekClick(null, null);
			// отчёты
			listReports.ItemsSource = ReportDescriptor.All;
			// справочники
			productsGrid.ItemsSource = _database.Products;
			reloadComplexProducts();
			unitsGrid.ItemsSource = _database.Units;
			documentTypesGrid.ItemsSource = DocumentType.AllTypes.Values;

			updateButtonsAvailability(productsGrid, buttonEditProduct, buttonDeleteProduct);
			updateButtonsAvailability(unitsGrid, buttonEditUnit, buttonDeleteUnit);
		}

		#endregion

		#region Работа с документами

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
			createDocument(DocumentType.Produce, dialog =>
			{
				dialog.ProductsGetter = db => db.Products.Where(p => p.Children.Count > 0);
			});
		}

		private void toWarehouseClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.ToWarehouse);
		}

		private void checkBalanceClick(object sender, RoutedEventArgs e)
		{
			var dialog = new SelectProductDialog();
			dialog.Initialize(_database);
			if (dialog.ShowDialog() == true)
			{
				var product = dialog.EditValue;
				var balance = _database.Balance;
				var getBalance = new Func<Product, decimal>(p =>
				{
					decimal count;
					return balance.TryGetValue(p.ID, out count)
						? count
						: 0;
				});

				var message = new StringBuilder();
				message.AppendLine("Имеется на складе: " + getBalance(product));

				if (product.Children.Count > 0)
				{
					decimal minCount = decimal.MaxValue;
					var children = new StringBuilder();
					foreach (var child in product.Children)
					{
						decimal childBalance = getBalance(child.Key);
						decimal canProduce = childBalance / child.Value;
						minCount = Math.Min(minCount, canProduce);
						children.AppendLine(string.Format(
							CultureInfo.InvariantCulture,
							"... {0} х \"{1}\", что хватит для производства {2} единиц товара",
							childBalance,
							child.Key.DisplayMember,
							canProduce));
					}
					message.AppendLine("Может быть произведено: " + minCount);
					message.AppendLine();
					message.AppendLine("Комплектующие на складе:");
					message.Append(children);
				}

				MessageBox.Show(
					message.ToString(),
					"Товар - " + product.DisplayMember,
					MessageBoxButton.OK,
					MessageBoxImage.Information);
			}
		}

		private void deleteDocumentsClick(object sender, RoutedEventArgs e)
		{
			var documentsToDelete = documentsGrid.SelectedItems.OfType<Document>().ToList();
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
			if (Settings.BalanceValidationStrategy.VerifyDelete(_database, documentsToDelete, errors))
			{
				foreach (var document in documentsToDelete)
				{
					document.Rollback(_database);
				}
				new Xml.Database(_database).Save();
				documentStateFilterChecked(this, null);
				reportHeader.Text = string.Empty;
				reportGrid.ItemsSource = null;
				buttonPrintReport.IsEnabled = false;
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
			dialog.Initialize(_database);
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					instance = viewModel.ConvertToBusinessLogic(_database);
					new Xml.Database(_database).Save();

					documentStateFilterChecked(this, null);
					documentsGrid.SelectedItem = instance;
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				reportGrid.ItemsSource = null;
				buttonPrintReport.IsEnabled = false;
			}
		}

		private void selectedDocumentsChanged(object sender, SelectionChangedEventArgs e)
		{
			var documents = documentsGrid.SelectedItems.OfType<Document>().ToList();
			deleteDocumentsButton.IsEnabled = documents.Count > 0 && documents.All(d => d.State == DocumentState.Active);
			editDocumentButton.IsEnabled = documents.Count == 1;
		}

		private void createDocument(DocumentType type, Action<DocumentDialog> dialogSetup = null)
		{
			var viewModel = new ViewModels.Document(type);
			var dialog = new DocumentDialog();
			dialog.Initialize(_database);
			if (dialogSetup != null)
			{
				dialogSetup(dialog);
			}
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var instance = viewModel.ConvertToBusinessLogic(_database);
					new Xml.Database(_database).Save();
					documentStateFilterChecked(this, null);
					documentsGrid.SelectedItem = instance;
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				reportGrid.ItemsSource = null;
				buttonPrintReport.IsEnabled = false;
			}
		}

		private void documentsGridDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var selectedItem = documentsGrid.SelectedItems.OfType<Document>().FirstOrDefault();
			if (selectedItem != null)
			{
				var dialog = new DocumentDialog();
				dialog.SetReadOnly();
				dialog.Initialize(_database);
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
			if (!_suppressDocumentChangeFilter)
			{
				refreshDocuments();
			}
		}

		private bool _suppressDocumentChangeFilter;

		private void refreshDocuments()
		{
			var sortDescriptors = documentsGrid.Items.SortDescriptions.ToList();
			var sortColumns = documentsGrid.Columns.Select(c => c.SortDirection).ToList();

			documentsGrid.ItemsSource = null;
			IEnumerable<Document> documents = _database.Documents;
			if (checkBoxShowObsoleteDocuments.IsChecked != true)
			{
				documents = documents.Where(d => d.State == DocumentState.Active);
			}
			if (documentsFromDatePicker.SelectedDate.HasValue)
			{
				documents = documents.Where(d => d.Date >= documentsFromDatePicker.SelectedDate.Value.Date);
			}
			if (documentsToDatePicker.SelectedDate.HasValue)
			{
				documents = documents.Where(d => d.Date < documentsToDatePicker.SelectedDate.Value.Date.AddDays(1));
			}
			documentsGrid.ItemsSource = documents.ToList();

			foreach (var sortDescription in sortDescriptors)
			{
				documentsGrid.Items.SortDescriptions.Add(sortDescription);
			}
			for (int c = 0; c < sortColumns.Count; c++)
			{
				documentsGrid.Columns[c].SortDirection = sortColumns[c];
			}
		}

		private void documentsTodayClick(object sender, RoutedEventArgs e)
		{
			_suppressDocumentChangeFilter = true;
			documentsToDatePicker.SelectedDate = documentsFromDatePicker.SelectedDate = DateTime.Now;
			_suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		private void documentsWeekClick(object sender, RoutedEventArgs e)
		{
			_suppressDocumentChangeFilter = true;
			var beginDate = DateTime.Now;
			switch (beginDate.DayOfWeek)
			{
				case DayOfWeek.Monday:
					break;
				case DayOfWeek.Tuesday:
					beginDate = beginDate.AddDays(-1);
					break;
				case DayOfWeek.Wednesday:
					beginDate = beginDate.AddDays(-2);
					break;
				case DayOfWeek.Thursday:
					beginDate = beginDate.AddDays(-3);
					break;
				case DayOfWeek.Friday:
					beginDate = beginDate.AddDays(-4);
					break;
				case DayOfWeek.Saturday:
					beginDate = beginDate.AddDays(-5);
					break;
				case DayOfWeek.Sunday:
					beginDate = beginDate.AddDays(-6);
					break;
				default:
					throw new Exception("Ошибка календаря: сегодня неизвестный день недели.");
			}
			documentsFromDatePicker.SelectedDate = beginDate;
			documentsToDatePicker.SelectedDate = beginDate.AddDays(6);
			_suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		private void documentsMonthClick(object sender, RoutedEventArgs e)
		{
			_suppressDocumentChangeFilter = true;
			var beginDate = DateTime.Now;
			beginDate = new DateTime(beginDate.Year, beginDate.Month, 1);
			documentsFromDatePicker.SelectedDate = beginDate;
			documentsToDatePicker.SelectedDate = beginDate.AddMonths(1).AddSeconds(-1);
			_suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		private void documentsYearClick(object sender, RoutedEventArgs e)
		{
			_suppressDocumentChangeFilter = true;
			var beginDate = DateTime.Now;
			beginDate = new DateTime(beginDate.Year, 1, 1);
			documentsFromDatePicker.SelectedDate = beginDate;
			documentsToDatePicker.SelectedDate = beginDate.AddYears(1).AddSeconds(-1);
			_suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		#endregion

		#region Работа со справочниками

		#region Товары

		private void reloadComplexProducts()
		{
			treeViewComplexProducts.ItemsSource = _database.Products.Where(p => p.Children.Count > 0).ToList();
		}

		private void productAddClick(object sender, RoutedEventArgs e)
		{
			var viewModel = new ViewModels.Product();
			var dialog = new ProductDialog();
			dialog.Initialize(_database);
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var instance = viewModel.ConvertToBusinessLogic(_database);
					new Xml.Database(_database).Save();
					productsGrid.ItemsSource = null;
					productsGrid.ItemsSource = _database.Products;
					productsGrid.SelectedItem = instance;
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

				var message = instance.FindUsages(_database);
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
				dialog.Initialize(_database);
				dialog.EditValue = viewModel;
				if (dialog.ShowDialog() == true)
				{
					try
					{
						instance = viewModel.ConvertToBusinessLogic(_database);
						new Xml.Database(_database).Save();
						productsGrid.ItemsSource = null;
						productsGrid.ItemsSource = _database.Products;
						productsGrid.SelectedItem = instance;
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
				message.Append(item.FindUsages(_database));
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
				_database.Products.Remove(item.ID);
			}
			new Xml.Database(_database).Save();
			productsGrid.ItemsSource = null;
			productsGrid.ItemsSource = _database.Products;
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
			dialog.Initialize(_database);
			dialog.EditValue = viewModel;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var instance = viewModel.ConvertToBusinessLogic(_database);
					new Xml.Database(_database).Save();
					unitsGrid.ItemsSource = null;
					unitsGrid.ItemsSource = _database.Units;
					unitsGrid.SelectedItem = instance;
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

				var message = instance.FindUsages(_database);
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
				dialog.Initialize(_database);
				dialog.EditValue = viewModel;
				if (dialog.ShowDialog() == true)
				{
					try
					{
						instance = viewModel.ConvertToBusinessLogic(_database);
						new Xml.Database(_database).Save();
						unitsGrid.ItemsSource = null;
						unitsGrid.ItemsSource = _database.Units;
						unitsGrid.SelectedItem = instance;
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
				message.Append(item.FindUsages(_database));
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
				_database.Units.Remove(item.ID);
			}
			new Xml.Database(_database).Save();
			unitsGrid.ItemsSource = null;
			unitsGrid.ItemsSource = _database.Units;
		}

		private void selectedUnitsChanged(object sender, SelectionChangedEventArgs e)
		{
			updateButtonsAvailability(unitsGrid, buttonEditUnit, buttonDeleteUnit);
		}

		#endregion

		#endregion

		#region Helpers

		/*private void addItem<ItemT, DialogT>(
			DataGrid grid,
			List<ItemT> table,
			Func<ItemT> createItem = null,
			Action<ItemT> beforeSave = null,
			Action<ItemT> afterSave = null,
			Action<DialogT> dialogSetup = null,
			Action updateGrid = null)
			where ItemT : IEntity,  IEditable<ItemT>, new()
			where DialogT : Window, IEditDialog<ItemT>, new()
		{
			var newItem = createItem != null
				? createItem()
				: new ItemT();
			var dialog = new DialogT();
			if (dialogSetup != null)
			{
				dialogSetup(dialog);
			}
			dialog.Initialize(database);
			dialog.EditValue = newItem;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					newItem.ID = IdHelper.GenerateNewId(table);
					newItem.AfterEdit(database);
					table.Add(newItem);
					if (beforeSave != null)
					{
						beforeSave(newItem);
					}
					new Xml.Database(database).Save();
					if (updateGrid == null)
					{
						updateGrid = () =>
						{
							grid.ItemsSource = null;
							grid.ItemsSource = table;
						};
					}
					updateGrid();
					if (afterSave != null)
					{
						afterSave(newItem);
					}
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void editItem<ItemT, DialogT>(
			DataGrid grid,
			IEnumerable<ItemT> table,
			Func<ItemT, bool> beforeEdit = null,
			Action<DialogT> dialogSetup = null)
			where ItemT : IEditable<ItemT>, new()
			where DialogT : Window, IEditDialog<ItemT>, new()
		{
			var selectedItems = grid.SelectedItems.OfType<ItemT>().ToList();
			if (selectedItems.Count > 0)
			{
				var editItem = selectedItems[0];
				if (beforeEdit != null && !beforeEdit(editItem))
				{
					return;
				}
				var copyItem = new ItemT();
				copyItem.Update(editItem);
				copyItem.BeforeEdit(database);
				var dialog = new DialogT();
				if (dialogSetup != null)
				{
					dialogSetup(dialog);
				}
				dialog.Initialize(database);
				dialog.EditValue = copyItem;
				if (dialog.ShowDialog() == true)
				{
					try
					{
						copyItem.AfterEdit(database);
						editItem.Update(copyItem);
						new Xml.Database(database).Save();
						grid.ItemsSource = null;
						grid.ItemsSource = table;
					}
					catch (Exception error)
					{
						MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		private void deleteItem<ItemT>(
			DataGrid grid,
			List<ItemT> table,
			Func<List<ItemT>, bool> beforeDelete = null)
		{
			var selectedItems = grid.SelectedItems.OfType<ItemT>().ToList();
			if (beforeDelete != null && !beforeDelete(selectedItems))
			{
				return;
			}
			if (selectedItems.Count > 0)
			{
				try
				{
					foreach (var item in selectedItems)
					{
						table.Remove(item);
					}
					new Xml.Database(database).Save();
					grid.ItemsSource = null;
					grid.ItemsSource = table;
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}*/

		private void updateButtonsAvailability(DataGrid grid, Button editButton, Button deleteButton)
		{
			int itemsCount = grid.SelectedItems != null
				? grid.SelectedItems.Count
				: (grid.SelectedItem != null ? 1 : 0);
			editButton.IsEnabled = itemsCount == 1;
			deleteButton.IsEnabled = itemsCount > 0;
		}

		#endregion

		#region Отчёты

		private void newReportClick(object sender, MouseButtonEventArgs e)
		{
			var reportDescriptor = listReports.SelectedItem as ReportDescriptor;
			if (reportDescriptor != null)
			{
				IReport report;
				if (reportDescriptor.CreateReport(_database, out report))
				{
					reportGrid.ItemsSource = null;
					reportGrid.Columns.Clear();
					reportHeader.Text = report.Title;
					foreach (var column in reportDescriptor.GetColumns())
					{
						reportGrid.Columns.Add(column);
					}
					reportGrid.ItemsSource = report.Items;
					buttonPrintReport.IsEnabled = true;
				}
			}
		}

		private static readonly Microsoft.Win32.SaveFileDialog _saveDocumentDialog = ExcelHelper.CreateSaveDialog();

		private void printReportClick(object sender, RoutedEventArgs e)
		{
			if (_saveDocumentDialog.ShowDialog() == true)
			{
				using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(_saveDocumentDialog.FileName, SpreadsheetDocumentType.Workbook))
				{
					ExcelHelper.ExportReport(spreadsheet, reportHeader.Text, reportGrid);
				}
				_saveDocumentDialog.FileName.ShellOpen();
			}
		}

		#endregion
	}
}

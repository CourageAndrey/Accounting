using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ComfortIsland.Database;
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

		private Database.Database database;

		private void formLoaded(object sender, RoutedEventArgs e)
		{
			// вычитка базы данных
			database = Database.Database.TryLoad();

			// документы
			stateColumn.Visibility = checkBoxShowObsoleteDocuments.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
			documentsGrid.Columns[0].SortDirection = ListSortDirection.Ascending;
			documentsGrid.Items.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
			documentsWeekClick(null, null);
			// отчёты
			listReports.ItemsSource = ReportDescriptor.All;
			// справочники
			productsGrid.ItemsSource = database.Products;
			reloadComplexProducts();
			unitsGrid.ItemsSource = database.Units;
			documentTypesGrid.ItemsSource = DocumentTypeImplementation.AllTypes.Values;

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
			dialog.Initialize(database);
			if (dialog.ShowDialog() == true)
			{
				var product = dialog.EditValue;
				var balance = database.Balance;
				var getBalance = new Func<Product, double>(p =>
				{
					var b = balance.FirstOrDefault(bb => bb.ProductId == p.ID);
					return b != null ? b.Count : 0;
				});

				var message = new StringBuilder();
				message.AppendLine("Имеется на складе: " + getBalance(product));

				if (product.Children.Count > 0)
				{
					double minCount = double.MaxValue;
					var children = new StringBuilder();
					foreach (var child in product.Children)
					{
						double childBalance = getBalance(child.Key);
						double canProduce = childBalance / child.Value;
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
			var balanceTable = database.Balance.Select(b => new Balance(b)).ToList();
			if (Document.TryDelete(database, documentsGrid.SelectedItems.OfType<Document>().ToList(), balanceTable))
			{
				database.Balance = balanceTable;
				Database.Database.Save();
				documentStateFilterChecked(this, null);
				reportHeader.Text = string.Empty;
				reportGrid.ItemsSource = null;
				buttonPrintReport.IsEnabled = false;
			}
		}

		private void editDocumentClick(object sender, RoutedEventArgs e)
		{
			var originalDocument = documentsGrid.SelectedItems.OfType<Document>().Single();
			var editedDocument = new Document();
			editedDocument.Update(originalDocument);
			editedDocument.ID = IdHelper.GenerateNewId(database.Documents);
			editedDocument.PreviousVersionId = originalDocument.ID;
			editedDocument.BeforeEdit();
			var dialog = new DocumentDialog();
			if (originalDocument.Type == DocumentType.Produce)
			{
				dialog.ProductsGetter = db => db.Products.Where(p => p.Children.Count > 0);
			}
			dialog.EditValue = editedDocument;
			dialog.IgnoreValidation = true;
			if (dialog.ShowDialog() == true)
			{
				editedDocument.AfterEdit(database);
				var balanceTable = database.Balance.Select(b => new Balance(b)).ToList();
				var documentsToApplyAgain = new Stack<Document>();

				// последовательный откат документов
				DateTime minDocDate = editedDocument.Date > originalDocument.Date ? originalDocument.Date : editedDocument.Date;
				bool originalDeleted = false;
				foreach (var document in database.Documents.Where(d => d.State == DocumentState.Active).OrderByDescending(d => d.Date).Where(d => d.Date >= minDocDate))
				{
					document.Rollback(balanceTable);
					/* удалено, так как в настоящей базе есть реальные ошибки
					if (!document.CheckBalance(balanceTable, "удалении", "редактировать"))
					{
						return;
					}*/
					
					if (document != originalDocument)
					{
						documentsToApplyAgain.Push(document);
					}
					else
					{
						originalDeleted = true;
					}
				}

				// если нужно - откат оригинала
				if (!originalDeleted)
				{
					originalDocument.Rollback(balanceTable);
					if (!originalDocument.CheckBalance(database, balanceTable, "отмене старой версии отредактированного", "редактировать"))
					{
						return;
					}
				}

				// накат неудалённых документов и отредактированной версии обратно
				bool editedApplied = false;
				while (documentsToApplyAgain.Count > 0)
				{
					var document = documentsToApplyAgain.Pop();
					if (!editedApplied && document.Date > editedDocument.Date)
					{ // применение отредактированной версии документа
						editedApplied = true;
						editedDocument.Apply(balanceTable);
						if (!editedDocument.CheckBalance(database, balanceTable, "применении новой версии отредактированного", "редактировать"))
						{
							return;
						}
					}
					document.Apply(balanceTable);
					if (!document.CheckBalance(database, balanceTable, "применении", "редактировать"))
					{
						return;
					}
				}

				// применение отредактированной версии документа, если она ещё не была применена
				if (!editedApplied)
				{ 
					editedDocument.Apply(balanceTable);
					if (!editedDocument.CheckBalance(database, balanceTable, "применении новой версии отредактированного", "редактировать"))
					{
						return;
					}
				}

				// если всё хорошо - применяем изменения в БД и на экране
				if (originalDocument.State == DocumentState.Active)
				{
					originalDocument.State = DocumentState.Edited;
				}
				database.Documents.Add(editedDocument);
				database.Balance = balanceTable;
				Database.Database.Save();
				documentStateFilterChecked(this, null);
				reportHeader.Text = string.Empty;
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
			addItem<Document, DocumentDialog>(
				documentsGrid,
				database.Documents,
				() => new Document { Date = DateTime.Now, Type = type, State = DocumentState.Active },
				document => document.Apply(database.Balance),
				item =>
				{
					reportHeader.Text = string.Empty;
					reportGrid.ItemsSource = null;
					buttonPrintReport.IsEnabled = false;
				},
				dialogSetup,
				() =>
				{
					documentStateFilterChecked(this, null);
				});
		}

		private void documentsGridDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var selectedItem = documentsGrid.SelectedItems.OfType<Document>().FirstOrDefault();
			if (selectedItem != null)
			{
				selectedItem.BeforeEdit();
				var dialog = new DocumentDialog();
				dialog.SetReadOnly();
				dialog.EditValue = selectedItem;
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
			if (!suppressDocumentChangeFilter)
			{
				refreshDocuments();
			}
		}

		private bool suppressDocumentChangeFilter;

		private void refreshDocuments()
		{
			var sortDescriptors = documentsGrid.Items.SortDescriptions.ToList();
			var sortColumns = documentsGrid.Columns.Select(c => c.SortDirection).ToList();

			documentsGrid.ItemsSource = null;
			IEnumerable<Document> documents = database.Documents;
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
			suppressDocumentChangeFilter = true;
			documentsToDatePicker.SelectedDate = documentsFromDatePicker.SelectedDate = DateTime.Now;
			suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		private void documentsWeekClick(object sender, RoutedEventArgs e)
		{
			suppressDocumentChangeFilter = true;
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
			suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		private void documentsMonthClick(object sender, RoutedEventArgs e)
		{
			suppressDocumentChangeFilter = true;
			var beginDate = DateTime.Now;
			beginDate = new DateTime(beginDate.Year, beginDate.Month, 1);
			documentsFromDatePicker.SelectedDate = beginDate;
			documentsToDatePicker.SelectedDate = beginDate.AddMonths(1).AddSeconds(-1);
			suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		private void documentsYearClick(object sender, RoutedEventArgs e)
		{
			suppressDocumentChangeFilter = true;
			var beginDate = DateTime.Now;
			beginDate = new DateTime(beginDate.Year, 1, 1);
			documentsFromDatePicker.SelectedDate = beginDate;
			documentsToDatePicker.SelectedDate = beginDate.AddYears(1).AddSeconds(-1);
			suppressDocumentChangeFilter = false;
			refreshDocuments();
		}

		#endregion

		#region Работа со справочниками

		#region Товары

		private void reloadComplexProducts()
		{
			var complexProducts = database.Products.Where(p => p.Children.Count > 0).ToList();
			foreach (var product in complexProducts)
			{
				product.BeforeEdit();
				foreach (var position in product.ChildrenToSerialize)
				{
					position.SetProduct(database);
				}
			}
			treeViewComplexProducts.ItemsSource = complexProducts;
		}

		private void productAddClick(object sender, RoutedEventArgs e)
		{
			addItem<Product, ProductDialog>(productsGrid, database.Products);
			reloadComplexProducts();
		}

		private void productEditClick(object sender, RoutedEventArgs e)
		{
			editItem<Product, ProductDialog>(productsGrid, database.Products, product =>
			{
				var documents = database.Documents.Where(d => DocumentTypeImplementation.AllTypes[d.Type].GetBalanceDelta(d).Keys.Contains(product.ID)).ToList();
				var parentProducts = database.Products.Where(p => p.Children.Keys.Contains(product)).ToList();
				if (documents.Count == 0 && parentProducts.Count == 0)
				{
					return true;
				}
				else
				{
					var message = new StringBuilder();
					if (documents.Count > 0)
					{
						message.AppendLine("Данный товар используется в следующих документах:");
						message.AppendLine();
						foreach (var document in documents)
						{
							message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0} {1} от {2}",
								document.TypeName,
								!string.IsNullOrEmpty(document.Number) ? "\"" + document.Number + "\"" : string.Empty,
								document.Date.ToShortDateString()));
						}
						message.AppendLine();
					}
					if (parentProducts.Count > 0)
					{
						message.AppendLine("Данный товар используется как составная часть в следующих товарах:");
						message.AppendLine();
						foreach (var parent in parentProducts)
						{
							message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0}", parent.DisplayMember));
						}
						message.AppendLine();
					}
					message.AppendLine("После изменения выбранного товара все они будут содержать новую исправленную версию. Продолжить редактирование?");
					return MessageBox.Show(
						message.ToString(),
						"Внимание",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question) == MessageBoxResult.Yes;
				}
			});
			reloadComplexProducts();
		}

		private void productDeleteClick(object sender, RoutedEventArgs e)
		{
			deleteItem(productsGrid, database.Products, products =>
			{
				var checkIds = products.Select(u => u.ID).ToList();
				var documents = database.Documents.Where(d => DocumentTypeImplementation.AllTypes[d.Type].GetBalanceDelta(d).Keys.Any(id => checkIds.Contains(id))).ToList();
				var parentProducts = database.Products.Where(p => p.Children.Keys.Any(pp => checkIds.Contains(pp.ID))).ToList();
				if (documents.Count == 0 && parentProducts.Count == 0)
				{
					return true;
				}
				else
				{
					var message = new StringBuilder();
					if (documents.Count > 0)
					{
						message.AppendLine("Выбранные товары используются в следующих документах и не могут быть удалены:");
						message.AppendLine();
						foreach (var document in documents)
						{
							message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0} {1} от {2}",
								document.TypeName,
								!string.IsNullOrEmpty(document.Number) ? "\"" + document.Number + "\"" : string.Empty,
								document.Date.ToShortDateString()));
						}
						message.AppendLine();
					}
					if (parentProducts.Count > 0)
					{
						message.AppendLine("Выбранные товары используются как составные части в других товарах:");
						message.AppendLine();
						foreach (var parent in parentProducts)
						{
							foreach (var child in parent.Children.Keys.Where(p => checkIds.Contains(p.ID)))
							{
								message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... \"{0}\" содержит \"{1}\"", parent.DisplayMember, child.DisplayMember));
							}
						}
						message.AppendLine();
					}
					MessageBox.Show(
						message.ToString(),
						"Невозможно удалить выбранные записи",
						MessageBoxButton.OK,
						MessageBoxImage.Warning);
					return false;
				}
			});
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
			addItem<Unit, UnitDialog>(unitsGrid, database.Units);
		}

		private void unitEditClick(object sender, RoutedEventArgs e)
		{
			editItem<Unit, UnitDialog>(unitsGrid, database.Units, unit =>
			{
				var products = database.Products.Where(p => p.Unit.ID == unit.ID).ToList();
				if (products.Count == 0)
				{
					return true;
				}
				else
				{
					var message = new StringBuilder();
					message.AppendLine("Следующие товары содержат ссылку на данную единицу измерения:");
					message.AppendLine();
					foreach (var product in products)
					{
						message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... {0}", product.Name));
					}
					message.AppendLine("После её изменения они будут содержать новую исправленную версию. Продолжить редактирование?");
					return MessageBox.Show(
						message.ToString(),
						"Внимание",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question) == MessageBoxResult.Yes;
				}
			});
		}

		private void unitDeleteClick(object sender, RoutedEventArgs e)
		{
			deleteItem(unitsGrid, database.Units, units =>
			{
				var checkIds = units.Select(u => u.ID).ToList();
				var products = database.Products.Where(p => checkIds.Contains(p.Unit.ID)).ToList();
				if (products.Count == 0)
				{
					return true;
				}
				else
				{
					var message = new StringBuilder();
					message.AppendLine("Выбранные единицы измерения используются в товарах и не могут быть удалены:");
					message.AppendLine();
					foreach (var product in products)
					{
						message.AppendLine(string.Format(CultureInfo.InvariantCulture, "... \"{1}\" является единицей измерения \"{0}\"", product.Name, product.Unit.Name ?? product.Unit.ShortName));
					}
					MessageBox.Show(
						message.ToString(),
						"Невозможно удалить выбранные записи",
						MessageBoxButton.OK,
						MessageBoxImage.Warning);
					return false;
				}
			});
		}

		private void selectedUnitsChanged(object sender, SelectionChangedEventArgs e)
		{
			updateButtonsAvailability(unitsGrid, buttonEditUnit, buttonDeleteUnit);
		}

		#endregion

		#endregion

		#region Helpers

		private void addItem<ItemT, DialogT>(
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
					Database.Database.Save();
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
				copyItem.BeforeEdit();
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
						Database.Database.Save();
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
					Database.Database.Save();
					grid.ItemsSource = null;
					grid.ItemsSource = table;
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
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

		#endregion

		#region Отчёты

		private void newReportClick(object sender, MouseButtonEventArgs e)
		{
			var reportDescriptor = listReports.SelectedItem as ReportDescriptor;
			if (reportDescriptor != null)
			{
				IReport report;
				if (reportDescriptor.CreateReport(database, out report))
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

		private static readonly Microsoft.Win32.SaveFileDialog saveDocumentDialog = ExcelHelper.CreateSaveDialog();

		private void printReportClick(object sender, RoutedEventArgs e)
		{
			if (saveDocumentDialog.ShowDialog() == true)
			{
				using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(saveDocumentDialog.FileName, SpreadsheetDocumentType.Workbook))
				{
					ExcelHelper.ExportReport(spreadsheet, reportHeader.Text, reportGrid);
				}
				saveDocumentDialog.FileName.ShellOpen();
			}
		}

		#endregion
	}
}

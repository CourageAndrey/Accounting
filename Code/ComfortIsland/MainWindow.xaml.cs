using System;
using System.Data.Objects;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ComfortIsland.Database;
using ComfortIsland.Dialogs;

namespace ComfortIsland
{
	public partial class MainWindow
	{
		#region Инициализация

		public MainWindow()
		{
			InitializeComponent();
		}

		private void formLoaded(object sender, RoutedEventArgs e)
		{
			using (var database = new ComfortIslandDatabase())
			{
				// документы
				documentsGrid.ItemsSource = database.Document.Execute(MergeOption.NoTracking).Select(d => d.PrepareToDisplay(database));
				// остатки
				balanceGrid.ItemsSource = database.Balance.Execute(MergeOption.NoTracking).Select(b => b.PrepareToDisplay(database));
				// справочники
				productsGrid.ItemsSource = database.Product.Execute(MergeOption.NoTracking).Select(p => p.PrepareToDisplay(database));
				unitsGrid.ItemsSource = database.Unit.Execute(MergeOption.NoTracking).Select(u => u.PrepareToDisplay(database));
				documentTypesGrid.ItemsSource = database.DocumentType.Execute(MergeOption.NoTracking);
			}

			updateButtonsAvailability(productsGrid, buttonEditProduct, buttonDeleteProduct);
			updateButtonsAvailability(unitsGrid, buttonEditUnit, buttonDeleteUnit);
		}

		#endregion

		#region Работа с документами

		private void incomeClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentTypeEnum.Income, (database, document) =>
			{
				foreach (var position in document.BindingPositions)
				{
					position.IncreaseBalance(database);
				}
				return true;
			});
		}

		private void outcomeClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentTypeEnum.Outcome, (database, document) =>
			{
				foreach (var position in document.BindingPositions)
				{
					position.DecreaseBalance(database);
				}
				return true;
			});
		}

		private void produceClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentTypeEnum.Produce, (database, document) =>
			{
				foreach (var position in document.BindingPositions)
				{
					position.IncreaseBalance(database);
					var product = database.Product.Execute(MergeOption.NoTracking).First(p => p.ID == position.ProductId);
					if (!product.Children.IsLoaded)
					{
						product.Children.Load(MergeOption.NoTracking);
					}
					foreach (var child in product.Children)
					{
						new DocumentPosition
						{
							ProductId = child.ChildID,
							Count = position.Count * child.Count,
						}.DecreaseBalance(database);
					}
				}
				return true;
			});
		}

		private void createDocument(DocumentTypeEnum type, Func<ComfortIslandDatabase, Document, bool> processFunction)
		{
			addItem<Document, DocumentDialog>(
				documentsGrid,
				database => database.Document,
				(table, item) => item.ID = table.Execute(MergeOption.NoTracking).Count() + 1,
				(database, item) =>
				{
					database.AddToDocument(item);
					return processFunction(database, item);
				},
				database => new Document
				{
					Date = DateTime.Now,
					DocumentTypeEnum = type,
					TypeName = database.DocumentType.Execute(MergeOption.NoTracking).First(t => t.ID == (short) type).Name,
				},
				(database, document) =>
				{
					balanceGrid.ItemsSource = database.Balance.Execute(MergeOption.NoTracking).Select(b => b.PrepareToDisplay(database));
				});
		}

		#endregion

		#region Работа со справочниками

		#region Товары

		private void productAddClick(object sender, RoutedEventArgs e)
		{
			addItem<Product, ProductDialog>(
				productsGrid,
				database => database.Product,
				(table, item) => item.ID = table.Execute(MergeOption.NoTracking).Count() + 1,
				(database, item) =>
				{
					database.AddToProduct(item);
					return true;
				});
		}

		private void productEditClick(object sender, RoutedEventArgs e)
		{
			editItem<Product, ProductDialog>(productsGrid, database => database.Product, (table, item) => table.FirstOrDefault(o => o.ID == item.ID));
		}

		private void productDeleteClick(object sender, RoutedEventArgs e)
		{
			deleteItem(productsGrid, database => database.Product, (table, item) => table.FirstOrDefault(o => o.ID == item.ID));
		}

		private void selectedProductsChanged(object sender, SelectionChangedEventArgs e)
		{
			updateButtonsAvailability(productsGrid, buttonEditProduct, buttonDeleteProduct);
		}

		#endregion

		#region Единицы измерения

		private void unitAddClick(object sender, RoutedEventArgs e)
		{
			addItem<Unit, UnitDialog>(
				unitsGrid,
				database => database.Unit,
				(table, item) => item.ID = (short)(table.Execute(MergeOption.NoTracking).Count() + 1),
				(database, item) =>
				{
					database.AddToUnit(item);
					return true;
				});
		}

		private void unitEditClick(object sender, RoutedEventArgs e)
		{
			editItem<Unit, UnitDialog>(unitsGrid, database => database.Unit, (table, item) => table.FirstOrDefault(o => o.ID == item.ID));
		}

		private void unitDeleteClick(object sender, RoutedEventArgs e)
		{
			deleteItem(unitsGrid, database => database.Unit, (table, item) => table.FirstOrDefault(o => o.ID == item.ID));
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
			Func<ComfortIslandDatabase, ObjectSet<ItemT>> tableSelector,
			Action<ObjectSet<ItemT>, ItemT> updateIdMethod,
			Func<ComfortIslandDatabase, ItemT, bool> addMethod,
			Func<ComfortIslandDatabase, ItemT> createItem = null,
			Action<ComfortIslandDatabase, ItemT> performAfterUpdate = null)
			where ItemT : class, IEditable<ItemT>, new()
			where DialogT : Window, IEditDialog<ItemT>, new()
		{
			using (var database = new ComfortIslandDatabase())
			{
				var newItem = createItem != null
					? createItem(database)
					: new ItemT();
				var dialog = new DialogT();
				dialog.Initialize(database);
				dialog.EditValue = newItem;
				if (dialog.ShowDialog() == true)
				{
					try
					{
						var table = tableSelector(database);
						updateIdMethod(table, newItem);
						newItem.PrepareToSave(database);
						if (addMethod(database, newItem))
						{
							database.SaveChanges();
							grid.ItemsSource = table.Execute(MergeOption.NoTracking).Select(p => p.PrepareToDisplay(database));
						}
						if (performAfterUpdate != null)
						{
							performAfterUpdate(database, newItem);
						}
					}
					catch (Exception error)
					{
						MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		private void editItem<ItemT, DialogT>(
			DataGrid grid,
			Func<ComfortIslandDatabase, ObjectSet<ItemT>> tableSelector,
			Func<ObjectSet<ItemT>, ItemT, ItemT> itemSelector)
			where ItemT : class, IEditable<ItemT>, new()
			where DialogT : Window, IEditDialog<ItemT>, new()
		{
			var selectedItems = grid.SelectedItems.OfType<ItemT>().ToList();
			if (selectedItems.Count > 0)
			{
				using (var database = new ComfortIslandDatabase())
				{
					var editItem = new ItemT();
					editItem.Update(selectedItems[0]);
					var dialog = new DialogT();
					dialog.Initialize(database);
					dialog.EditValue = editItem;
					if (dialog.ShowDialog() == true)
					{
						try
						{
							var table = tableSelector(database);
							var dbItem = itemSelector(table, editItem);
							dbItem.Update(editItem);
							dbItem.PrepareToSave(database);
							database.SaveChanges();
							grid.ItemsSource = table.Execute(MergeOption.NoTracking).Select(p => p.PrepareToDisplay(database));
						}
						catch (Exception error)
						{
							MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
				}
			}
		}

		private void deleteItem<ItemT>(
			DataGrid grid,
			Func<ComfortIslandDatabase, ObjectSet<ItemT>> tableSelector,
			Func<ObjectSet<ItemT>, ItemT, ItemT> itemSelector)
			where ItemT : class, IEditable<ItemT>
		{
			var selectedItems = grid.SelectedItems.OfType<ItemT>().ToList();
			if (selectedItems.Count > 0)
			{
				try
				{
					using (var database = new ComfortIslandDatabase())
					{
						var table = tableSelector(database);
						foreach (var item in selectedItems)
						{
							table.DeleteObject(itemSelector(table, item));
						}
						database.SaveChanges();
						grid.ItemsSource = table.Execute(MergeOption.NoTracking).Select(p => p.PrepareToDisplay(database));
					}
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
	}
}

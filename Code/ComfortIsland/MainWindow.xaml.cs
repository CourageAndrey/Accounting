﻿using System;
using System.Collections.Generic;
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
			// вычитка базы данных
			var database = Database.Database.TryLoad();

			// документы
			documentsGrid.ItemsSource = database.Documents;
			// остатки
			balanceGrid.ItemsSource = database.Balance;
			// справочники
			productsGrid.ItemsSource = database.Products;
			unitsGrid.ItemsSource = database.Units;
			documentTypesGrid.ItemsSource = DocumentTypeHelper.AllTypes;

			updateButtonsAvailability(productsGrid, buttonEditProduct, buttonDeleteProduct);
			updateButtonsAvailability(unitsGrid, buttonEditUnit, buttonDeleteUnit);
		}

		#endregion

		#region Работа с документами

		private void incomeClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.Income, document =>
			{
				var balanceTable = Database.Database.Instance.Balance;
				foreach (var position in document.Positions)
				{
					var balance = balanceTable.FirstOrDefault(b => b.ProductId == position.Key.ID);
					if (balance != null)
					{
						balance.Count += position.Value;
					}
					else
					{
						balanceTable.Add(new Balance(position.Key, position.Value));
					}
				}
			});
		}

		private void outcomeClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.Outcome, document =>
			{
				var balanceTable = Database.Database.Instance.Balance;
				foreach (var position in document.Positions)
				{
					var balance = balanceTable.First(b => b.ProductId == position.Key.ID);
					balance.Count -= position.Value;
				}
			});
		}

		private void produceClick(object sender, RoutedEventArgs e)
		{
			createDocument(DocumentType.Produce, document =>
			{
				var database = Database.Database.Instance;
				var balanceTable = database.Balance;
				foreach (var position in document.Positions)
				{
					var product = position.Key;

					// increase balance
					var balance = balanceTable.FirstOrDefault(b => b.ProductId == product.ID);
					if (balance != null)
					{
						balance.Count += position.Value;
					}
					else
					{
						balanceTable.Add(new Balance(position.Key, position.Value));
					}

					// decrease balance
					foreach (var child in product.Children)
					{
						balance = balanceTable.First(b => b.ProductId == child.Key.ID);
						balance.Count -= (position.Value * child.Value);
					}
				}
			});
		}

		private void createDocument(DocumentType type, Action<Document> processFunction)
		{
			addItem<Document, DocumentDialog>(
				documentsGrid,
				Database.Database.Instance.Documents,
				() => new Document { Date = DateTime.Now, Type = type, },
				processFunction,
				item =>
				{
					balanceGrid.ItemsSource = null;
					balanceGrid.ItemsSource = Database.Database.Instance.Balance;
				});
		}

		#endregion

		#region Работа со справочниками

		#region Товары

		private void productAddClick(object sender, RoutedEventArgs e)
		{
			addItem<Product, ProductDialog>(productsGrid, Database.Database.Instance.Products);
		}

		private void productEditClick(object sender, RoutedEventArgs e)
		{
			editItem<Product, ProductDialog>(productsGrid, Database.Database.Instance.Products);
		}

		private void productDeleteClick(object sender, RoutedEventArgs e)
		{
			deleteItem(productsGrid, Database.Database.Instance.Products);
		}

		private void selectedProductsChanged(object sender, SelectionChangedEventArgs e)
		{
			updateButtonsAvailability(productsGrid, buttonEditProduct, buttonDeleteProduct);
		}

		#endregion

		#region Единицы измерения

		private void unitAddClick(object sender, RoutedEventArgs e)
		{
			addItem<Unit, UnitDialog>(unitsGrid, Database.Database.Instance.Units);
		}

		private void unitEditClick(object sender, RoutedEventArgs e)
		{
			editItem<Unit, UnitDialog>(unitsGrid, Database.Database.Instance.Units);
		}

		private void unitDeleteClick(object sender, RoutedEventArgs e)
		{
			deleteItem(unitsGrid, Database.Database.Instance.Units);
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
			Action<ItemT> afterSave = null)
			where ItemT : IEntity,  IEditable<ItemT>, new()
			where DialogT : Window, IEditDialog<ItemT>, new()
		{
			var newItem = createItem != null
				? createItem()
				: new ItemT();
			var dialog = new DialogT { EditValue = newItem };
			if (dialog.ShowDialog() == true)
			{
				try
				{
					newItem.ID = table.Count + 1;
					newItem.AfterEdit();
					table.Add(newItem);
					if (beforeSave != null)
					{
						beforeSave(newItem);
					}
					Database.Database.Save();
					grid.ItemsSource = null;
					grid.ItemsSource = table;
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
			IEnumerable<ItemT> table)
			where ItemT : IEditable<ItemT>, new()
			where DialogT : Window, IEditDialog<ItemT>, new()
		{
			var selectedItems = grid.SelectedItems.OfType<ItemT>().ToList();
			if (selectedItems.Count > 0)
			{
				var editItem = selectedItems[0];
				var copyItem = new ItemT();
				copyItem.Update(editItem);
				copyItem.BeforeEdit();
				var dialog = new DialogT { EditValue = copyItem };
				if (dialog.ShowDialog() == true)
				{
					try
					{
						copyItem.AfterEdit();
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
			List<ItemT> table)
		{
			var selectedItems = grid.SelectedItems.OfType<ItemT>().ToList();
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
	}
}

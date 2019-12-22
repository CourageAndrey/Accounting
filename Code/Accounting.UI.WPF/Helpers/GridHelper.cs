using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Accounting.Core.BusinessLogic;
using Accounting.UI.WPF.Dialogs;

namespace Accounting.UI.WPF.Helpers
{
	public static class GridHelper
	{
		public static void RefreshGrid(this DataGrid grid, object selectedItem = null)
		{
			grid.ItemsSource = null;
			grid.ItemsSource = (IEnumerable) grid.Tag;
			if (selectedItem != null)
			{
				grid.SelectedItem = selectedItem;
			}
		}

		public static void UpdateButtonsAvailability(this DataGrid grid, Button editButton, Button deleteButton)
		{
			editButton.IsEnabled = grid.SelectedItems.Count == 1;
			deleteButton.IsEnabled = grid.SelectedItems.Count > 0;
		}

		public static bool AddNewEntity(this DataGrid grid, IViewModel viewModel, WpfAccountingApplication application, Type entityType)
		{
			bool isChanged = false;

			var dialog = application.UiFactory.CreateEditDialog(viewModel, application);
			if (((Window) dialog).ShowDialog() == true)
			{
				try
				{
					var instance = viewModel.ConvertToEntity(application.Database);
					application.DatabaseDriver.Save(application.Database);
					grid.RefreshGrid(instance);
					isChanged = true;
				}
				catch (Exception error)
				{
					MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}

			return isChanged;
		}

		public static bool EditEntity(this DataGrid grid, WpfAccountingApplication application, Type entityType)
		{
			bool isChanged = false;

			var selectedItems = grid.SelectedItems.OfType<IEntity>().ToList();
			if (selectedItems.Count > 0)
			{
				var instance = selectedItems[0];

				var message = instance.FindUsages(application.Database);
				if (message.Length > 0 && !LongTextDialog.Ask(
					message.ToString(),
					"Редактирование приведёт к дополнительным изменениям. Продолжить?"))
				{
					return isChanged;
				}

				var viewModel = application.UiFactory.CreateViewModel(instance);
				var dialog = application.UiFactory.CreateEditDialog(viewModel, application);
				if (((Window) dialog).ShowDialog() == true)
				{
					try
					{
						instance = viewModel.ConvertToEntity(application.Database);
						application.DatabaseDriver.Save(application.Database);
						grid.RefreshGrid(instance);
						isChanged = true;
					}
					catch (Exception error)
					{
						MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}

			return isChanged;
		}
	}
}

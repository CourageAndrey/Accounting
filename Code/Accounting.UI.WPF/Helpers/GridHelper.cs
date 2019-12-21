using System.Collections;
using System.Windows.Controls;

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
	}
}

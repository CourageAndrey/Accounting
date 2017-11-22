using System.Windows;

using ComfortIsland.Database;

namespace ComfortIsland.Dialogs
{
	public partial class SelectProductDialog : IEditDialog<Product>
	{
		public SelectProductDialog()
		{
			InitializeComponent();
		}

		public Product EditValue
		{
			get { return productsList.SelectedItem as Product; }
			set { productsList.SelectedItem = value; }
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			if (productsList.SelectedItem != null)
			{
				DialogResult = true;
			}
			else
			{
				MessageBox.Show("Не выбран товар", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void cancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void windowLoaded(object sender, RoutedEventArgs e)
		{
			productsList.ItemsSource = Database.Database.Instance.Products;
		}
	}
}

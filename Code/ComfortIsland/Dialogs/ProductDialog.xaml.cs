using System.Text;
using System.Windows;

using ComfortIsland.Database;

namespace ComfortIsland.Dialogs
{
	public partial class ProductDialog : IEditDialog<Product>
	{
		public ProductDialog()
		{
			InitializeComponent();
		}

		public Product EditValue
		{
			get { return (Product) contextControl.DataContext; }
			set { contextControl.DataContext = value; }
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			StringBuilder errors;
			if (EditValue.Validate(out errors))
			{
				DialogResult = true;
			}
			else
			{
				MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void cancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void dialogLoaded(object sender, RoutedEventArgs e)
		{
			comboBoxUnit.ItemsSource = Database.Database.Instance.Units;
			comboBoxProducts.ItemsSource = Database.Database.Instance.Products;
		}
	}
}

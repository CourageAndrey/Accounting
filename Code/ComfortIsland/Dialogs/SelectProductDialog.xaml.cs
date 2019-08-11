using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

namespace ComfortIsland.Dialogs
{
	public partial class SelectProductDialog : IEditDialog<Product>
	{
		public SelectProductDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IApplication application)
		{
			_application = application;
			FontSize = application.Settings.UserInterface.FontSize;
			productsList.ItemsSource = _application.Database.Products;
		}

		private IApplication _application;

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

		#region ComboBox autocomplete

		private void previewTextInput(object sender, TextCompositionEventArgs e)
		{
			AutoCompleteHelper.PreviewTextInput((ComboBox)sender, e);
		}

		private void pasting(object sender, DataObjectPastingEventArgs e)
		{
			AutoCompleteHelper.Pasting((ComboBox)sender, e);
		}

		private void previewKeyUp(object sender, KeyEventArgs e)
		{
			AutoCompleteHelper.PreviewKeyUp((ComboBox)sender, e);
		}

		#endregion
	}
}

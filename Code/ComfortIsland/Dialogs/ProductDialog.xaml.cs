using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

namespace ComfortIsland.Dialogs
{
	public partial class ProductDialog : IEditDialog<ViewModels.Product>
	{
		public ProductDialog()
		{
			InitializeComponent();
		}

		public ViewModels.Product EditValue
		{
			get { return (ViewModels.Product) contextControl.DataContext; }
			set { contextControl.DataContext = value; }
		}

		private Database database;

		public void Initialize(Database database)
		{
			this.database = database;
			comboBoxUnit.ItemsSource = database.Units;
			comboBoxProducts.ItemsSource = database.Products;
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			StringBuilder errors;
#warning if (EditValue.Validate(database, out errors))
			{
				DialogResult = true;
			}
			/*else
			{
				MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
			}*/
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

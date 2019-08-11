using System.Linq;
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

		public void ConnectTo(IApplication application)
		{
			_application = application;
			FontSize = application.Settings.UserInterface.FontSize;
			comboBoxUnit.ItemsSource = application.Database.Units;
			comboBoxProducts.ItemsSource = application.Database.Products;
		}

		private IApplication _application;

		public ViewModels.Product EditValue
		{
			get { return (ViewModels.Product) contextControl.DataContext; }
			set
			{
				buttonOk.IsEnabled = !value.HasErrors;
				value.ErrorsChanged += (sender, args) => { buttonOk.IsEnabled = !value.HasErrors; };
				contextControl.DataContext = value;
			}
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			if (!EditValue.HasErrors)
			{
				var errors = new StringBuilder();
				bool isValid = Position.PositionsDoNotDuplicate(EditValue.Children, "составляющие части", errors);
				for (int line = 0; line < EditValue.Children.Count; line++)
				{
					isValid &= Position.ProductIsSet(EditValue.Children[line].ID, line + 1, errors);
					isValid &= Position.CountIsPositive(EditValue.Children[line].Count, line + 1, errors);
				}
				if (isValid & Product.ChildrenAreNotRecursive(EditValue.ID, EditValue.Children.Select(position => _application.Database.Products[position.ID]), errors))
				{
					DialogResult = true;
				}
				else
				{
					MessageBox.Show(errors.ToString(), "Ошибка в списке дочерних продуктов", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
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

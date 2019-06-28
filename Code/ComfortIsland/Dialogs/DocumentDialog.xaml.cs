using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

namespace ComfortIsland.Dialogs
{
	public partial class DocumentDialog : IEditDialog<ViewModels.Document>
	{
		public DocumentDialog()
		{
			InitializeComponent();
		}

		public ViewModels.Document EditValue
		{
			get { return (ViewModels.Document) contextControl.DataContext; }
			set { contextControl.DataContext = value; }
		}

		private Database database;

		public void Initialize(Database database)
		{
			this.database = database;
			comboBoxProducts.ItemsSource = ProductsGetter != null
				? ProductsGetter(database)
				: database.Products;
		}

		public Func<Database, IEnumerable<Product>> ProductsGetter
		{ get; set; }

		public bool IgnoreValidation
		{ get; set; }

		private void okClick(object sender, RoutedEventArgs e)
		{
			if (IgnoreValidation)
			{
				DialogResult = true;
			}
			else
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
		}

		private void cancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		public void SetReadOnly()
		{
			foreach (var child in contextControl.Children)
			{
				if (child is TextBox)
				{
					(child as TextBox).IsReadOnly = true;
				}
				else if (child is DatePicker)
				{
					(child as DatePicker).IsEnabled = false;
				}
				else if (child is GroupBox)
				{
					var grid = (child as GroupBox).Content as DataGrid;
					if (grid != null)
					{
						grid.IsReadOnly = true;
					}
				}
			}
			buttonOk.Visibility = Visibility.Hidden;
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using ComfortIsland.Database;

namespace ComfortIsland.Dialogs
{
	public partial class DocumentDialog : IEditDialog<Document>
	{
		public DocumentDialog()
		{
			InitializeComponent();
		}

		public Document EditValue
		{
			get { return (Document) contextControl.DataContext; }
			set { contextControl.DataContext = value; }
		}

		public Func<IEnumerable<Product>> ProductsGetter
		{ get; set; }

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
			comboBoxProducts.ItemsSource = (ProductsGetter ?? (() => Database.Database.Instance.Products))();
		}
	}
}

using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Windows;

using ComfortIsland.Database;

namespace ComfortIsland.Dialogs
{
	public partial class PriceDialog : IEditDialog<Price>
	{
		public PriceDialog()
		{
			InitializeComponent();
		}

		private ComfortIslandDatabase database;

		public void Initialize(ComfortIslandDatabase database)
		{
			this.database = database;
			comboBoxProduct.ItemsSource = database.Product.Execute(MergeOption.NoTracking).Select(p => p.PrepareToDisplay(database));
		}

		public Price EditValue
		{
			get { return (Price) contextControl.DataContext; }
			set { contextControl.DataContext = value; }
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			StringBuilder errors;
			if (EditValue.Validate(database, out errors))
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
	}
}

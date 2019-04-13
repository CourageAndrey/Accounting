using System.Text;
using System.Windows;

using ComfortIsland.Database;

namespace ComfortIsland.Dialogs
{
	public partial class UnitDialog : IEditDialog<Unit>
	{
		public UnitDialog()
		{
			InitializeComponent();
		}

		public Unit EditValue
		{
			get { return (Unit) contextControl.DataContext; }
			set { contextControl.DataContext = value; }
		}

		private Database.Database database;

		public void Initialize(Database.Database database)
		{
			this.database = database;
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

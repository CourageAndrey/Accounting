using System.Text;
using System.Windows;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.Dialogs
{
	public partial class UnitDialog : IEditDialog<ViewModels.Unit>
	{
		public UnitDialog()
		{
			InitializeComponent();
		}

		public ViewModels.Unit EditValue
		{
			get { return (ViewModels.Unit) contextControl.DataContext; }
			set { contextControl.DataContext = value; }
		}

		private Database database;

		public void Initialize(Database database)
		{
			this.database = database;
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
	}
}

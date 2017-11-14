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

		private ComfortIslandDatabase database;

		public void Initialize(ComfortIslandDatabase database)
		{
			this.database = database;
		}

		public Document EditValue
		{
			get { return (Document) contextControl.DataContext; }
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

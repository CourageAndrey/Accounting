using System;
using System.Windows;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.Dialogs
{
	public partial class SelectDateDialog : IEditDialog<DateTime>, IApplicationClient
	{
		public SelectDateDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IApplication application)
		{
			_application = application;
			FontSize = application.Settings.FontSize;
		}

		private IApplication _application;

		public DateTime EditValue
		{
			get { return datePicker.SelectedDate.Value; }
			set { datePicker.SelectedDate = value; }
		}

		public void Initialize(Database database)
		{ }

		public bool IncludeAllProducts
		{
			get { return checkBox.IsChecked.Value; }
			set { checkBox.IsChecked = value; }
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			if (datePicker.SelectedDate.HasValue)
			{
				DialogResult = true;
			}
			else
			{
				MessageBox.Show("Не выбрана дата", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void cancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}

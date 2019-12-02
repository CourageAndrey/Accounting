using System.Windows;

using ComfortIsland.Reports.Params;

using Accounting.Core.Application;

namespace ComfortIsland.Dialogs
{
	public partial class SelectDateDialog : IEditDialog<BalanceReportParams>
	{
		public SelectDateDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IAccountingApplication application)
		{
			_application = application;
			FontSize = application.Settings.UserInterface.FontSize;
		}

		private IAccountingApplication _application;

		public BalanceReportParams EditValue
		{
			get { return new BalanceReportParams(datePicker.SelectedDate.Value, checkBox.IsChecked.Value); }
			set
			{
				datePicker.SelectedDate = value.Date;
				checkBox.IsChecked = value.IncludeAllProducts;
			}
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

using System.Windows;

using Accounting.Core.Application;
using Accounting.Core.Reports.Params;

namespace Accounting.UI.WPF.Dialogs
{
	public partial class SelectPeriodDialog : IEditDialog<PeriodParams>
	{
		public SelectPeriodDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IAccountingApplication application)
		{
			_application = application;
			FontSize = application.Settings.UserInterface.FontSize;
		}

		private IAccountingApplication _application;

		public PeriodParams EditValue
		{
			get { return new PeriodParams(fromDatePicker.SelectedDate.Value, toDatePicker.SelectedDate.Value); }
			set
			{
				fromDatePicker.SelectedDate = value.Period.From;
				toDatePicker.SelectedDate = value.Period.To;
			}
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			if (fromDatePicker.SelectedDate.HasValue && toDatePicker.SelectedDate.HasValue)
			{
				DialogResult = true;
			}
			else
			{
				MessageBox.Show("Должны быть выбраны даты начала и конца периода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void cancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}

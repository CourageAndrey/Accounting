using System;
using System.Windows;

namespace ComfortIsland.Dialogs
{
	public partial class SelectPeriodDialog : IEditDialog<Tuple<DateTime, DateTime>>
	{
		public SelectPeriodDialog()
		{
			InitializeComponent();
		}

		public Tuple<DateTime, DateTime> EditValue
		{
			get { return new Tuple<DateTime, DateTime>(fromDatePicker.SelectedDate.Value, toDatePicker.SelectedDate.Value); }
			set
			{
				fromDatePicker.SelectedDate = value.Item1;
				toDatePicker.SelectedDate = value.Item2;
			}
		}

		public void Initialize(Database.Database database)
		{ }

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

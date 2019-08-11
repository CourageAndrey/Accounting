﻿using System.Windows;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

namespace ComfortIsland.Dialogs
{
	public partial class SelectPeriodDialog : IEditDialog<Period>, IApplicationClient
	{
		public SelectPeriodDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IApplication application)
		{
			_application = application;
			FontSize = application.Settings.FontSize;
		}

		private IApplication _application;

		public Period EditValue
		{
			get { return new Period(fromDatePicker.SelectedDate.Value, toDatePicker.SelectedDate.Value); }
			set
			{
				fromDatePicker.SelectedDate = value.From;
				toDatePicker.SelectedDate = value.To;
			}
		}

		public void Initialize(Database database)
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

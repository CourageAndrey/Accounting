﻿using System.Text;
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

		private ComfortIslandDatabase database;

		public void Initialize(ComfortIslandDatabase database)
		{
			this.database = database;
		}

		public Unit EditValue
		{
			get { return (Unit) contextControl.DataContext; }
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

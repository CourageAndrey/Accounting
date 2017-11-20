using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ComfortIsland.Dialogs;

namespace ComfortIsland.Reports
{
	public class ReportDescriptor
	{
		#region Properties

		public string Title
		{ get; private set; }

		private readonly Func<IEnumerable<DataGridColumn>> columnsGetter;
		private delegate bool ReportCreator(out IReport report);
		private readonly ReportCreator reportCreator;

		#endregion

		private ReportDescriptor(string title, Func<IEnumerable<DataGridColumn>> columnsGetter, ReportCreator reportCreator)
		{
			Title = title;
			this.columnsGetter = columnsGetter;
			this.reportCreator = reportCreator;
		}

		public IEnumerable<DataGridColumn> GetColumns()
		{
			return columnsGetter();
		}

		public bool CreateReport(out IReport report)
		{
			return reportCreator(out report);
		}

		#region Список

		public static readonly ReportDescriptor Balance = new ReportDescriptor("Складские остатки", () => new List<DataGridColumn>
		{
			new DataGridTextColumn
			{
				Header = "Код",
				Binding = new Binding { Path = new PropertyPath("ProductCode"), Mode = BindingMode.OneTime },
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Товар",
				Binding = new Binding { Path = new PropertyPath("ProductName"), Mode = BindingMode.OneTime },
				MinWidth = 300,
			},
			new DataGridTextColumn
			{
				Header = "Ед/изм",
				Binding = new Binding { Path = new PropertyPath("ProductUnit"), Mode = BindingMode.OneTime },
				MinWidth = 50,
			},
			new DataGridTextColumn
			{
				Header = "Остатки",
				Binding = new Binding { Path = new PropertyPath("Count"), Mode = BindingMode.OneTime },
				MinWidth = 100,
			},
		}, createBalanceReport);

		public static readonly IEnumerable<ReportDescriptor> All = new ReadOnlyCollection<ReportDescriptor>(new List<ReportDescriptor>
		{
			Balance,
		});

		private static bool createBalanceReport(out IReport report)
		{
			var dialog = new SelectDateDialog { EditValue = DateTime.Now };
			if (dialog.ShowDialog() == true)
			{
				report = new BalanceReport(dialog.EditValue);
				return true;
			}
			else
			{
				report = null;
				return false;
			}
			
		}

		#endregion
	}
}

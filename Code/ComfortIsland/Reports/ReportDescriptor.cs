using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Dialogs;
using ComfortIsland.Helpers;

namespace ComfortIsland.Reports
{
	public class ReportDescriptor
	{
		#region Properties

		public string Title
		{ get; private set; }

		private readonly Func<IEnumerable<DataGridColumn>> _columnsGetter;
		private delegate bool ReportCreator(Database database, out IReport report);
		private readonly ReportCreator _reportCreator;

		#endregion

		private ReportDescriptor(string title, Func<IEnumerable<DataGridColumn>> columnsGetter, ReportCreator reportCreator)
		{
			Title = title;
			_columnsGetter = columnsGetter;
			_reportCreator = reportCreator;
		}

		public IEnumerable<DataGridColumn> GetColumns()
		{
			var columns = _columnsGetter().ToList();
			foreach (var column in columns.OfType<DataGridTextColumn>())
			{
				var binding = column.Binding as Binding;
				if (binding != null && binding.Converter == DigitRoundingConverter.Instance)
				{
					if (column.CellStyle == null)
					{
						column.CellStyle = new Style();
					}
					column.CellStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right));
				}
			}
			return columns;
		}

		public bool CreateReport(Database database, out IReport report)
		{
			return _reportCreator(database, out report);
		}

		#region Список

		public static readonly ReportDescriptor Balance = new ReportDescriptor("Складские остатки", () => new List<DataGridColumn>
		{
			new DataGridTextColumn
			{
				Header = "Товар",
				Binding = new Binding { Path = new PropertyPath("BoundProduct.Name"), Mode = BindingMode.OneTime },
				MinWidth = 300,
			},
			new DataGridTextColumn
			{
				Header = "Ед/изм",
				Binding = new Binding { Path = new PropertyPath("BoundProduct.Unit.Name"), Mode = BindingMode.OneTime },
				MinWidth = 50,
			},
			new DataGridTextColumn
			{
				Header = "Остатки",
				Binding = new Binding { Path = new PropertyPath("Count"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
		}, createBalanceReport);

		public static readonly ReportDescriptor Trade = new ReportDescriptor("Товарный отчёт", () => new List<DataGridColumn>
		{
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
				Header = "На начало периода",
				Binding = new Binding { Path = new PropertyPath("InitialBalance"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Приобретено",
				Binding = new Binding { Path = new PropertyPath("Income"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Произведено",
				Binding = new Binding { Path = new PropertyPath("Produced"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Продано",
				Binding = new Binding { Path = new PropertyPath("Selled"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Израсходовано",
				Binding = new Binding { Path = new PropertyPath("UsedToProduce"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Отправлено на склад",
				Binding = new Binding { Path = new PropertyPath("SentToWarehouse"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "На конец периода",
				Binding = new Binding { Path = new PropertyPath("FinalBalance"), Mode = BindingMode.OneTime, Converter = DigitRoundingConverter.Instance },
				MinWidth = 100,
			},
		}, createTradeReport);


		public static readonly IEnumerable<ReportDescriptor> All = new ReadOnlyCollection<ReportDescriptor>(new List<ReportDescriptor>
		{
			Balance,
			Trade,
		});

		private static bool createBalanceReport(Database database, out IReport report)
		{
			var dialog = new SelectDateDialog { EditValue = DateTime.Now };
			if (dialog.ShowDialog() == true)
			{
				report = new BalanceReport(database, dialog.EditValue, dialog.IncludeAllProducts);
				return true;
			}
			else
			{
				report = null;
				return false;
			}
		}

		private static bool createTradeReport(Database database, out IReport report)
		{
			var dialog = new SelectPeriodDialog { EditValue = new Period(DateTime.Now.AddDays(-7), DateTime.Now) };
			if (dialog.ShowDialog() == true)
			{
				report = new TradeReport(database, dialog.EditValue);
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

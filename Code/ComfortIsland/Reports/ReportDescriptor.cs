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
		private delegate bool ReportCreator(IApplication application, out IReport report);
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
					column.CellStyle = column.CellStyle ?? new Style();
					column.CellStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right));
				}
			}
			return columns;
		}

		public bool CreateReport(IApplication application, Database database, out IReport report)
		{
			return _reportCreator(application, out report);
		}

		#region Список

		public static readonly ReportDescriptor Balance = new ReportDescriptor("Складские остатки", () => new List<DataGridColumn>
		{
			new DataGridTextColumn
			{
				Header = "Товар",
				Binding = bind("BoundProduct.Name"),
				MinWidth = 300,
			},
			new DataGridTextColumn
			{
				Header = "Ед/изм",
				Binding = bind("BoundProduct.Unit.Name"),
				MinWidth = 50,
			},
			new DataGridTextColumn
			{
				Header = "Остатки",
				Binding = bind("Count", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
		}, createBalanceReport);

		public static readonly ReportDescriptor Trade = new ReportDescriptor("Товарный отчёт", () => new List<DataGridColumn>
		{
			new DataGridTextColumn
			{
				Header = "Товар",
				Binding = bind("ProductName"),
				MinWidth = 300,
			},
			new DataGridTextColumn
			{
				Header = "Ед/изм",
				Binding = bind("ProductUnit"),
				MinWidth = 50,
			},
			new DataGridTextColumn
			{
				Header = "На начало периода",
				Binding = bind("InitialBalance", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Приобретено",
				Binding = bind("Income", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Произведено",
				Binding = bind("Produced", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Продано",
				Binding = bind("Selled", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Израсходовано",
				Binding = bind("UsedToProduce", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "Отправлено на склад",
				Binding = bind("SentToWarehouse", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
			new DataGridTextColumn
			{
				Header = "На конец периода",
				Binding = bind("FinalBalance", DigitRoundingConverter.Instance),
				MinWidth = 100,
			},
		}, createTradeReport);


		public static readonly IEnumerable<ReportDescriptor> All = new ReadOnlyCollection<ReportDescriptor>(new List<ReportDescriptor>
		{
			Balance,
			Trade,
		});

		private static bool createBalanceReport(IApplication application, out IReport report)
		{
			var dialog = new SelectDateDialog { EditValue = DateTime.Now };
			dialog.ConnectTo(application);
			if (dialog.ShowDialog() == true)
			{
				report = new BalanceReport(application.Database, dialog.EditValue, dialog.IncludeAllProducts);
				return true;
			}
			else
			{
				report = null;
				return false;
			}
		}

		private static bool createTradeReport(IApplication application, out IReport report)
		{
			var dialog = new SelectPeriodDialog { EditValue = new Period(DateTime.Now.AddDays(-7), DateTime.Now) };
			dialog.ConnectTo(application);
			if (dialog.ShowDialog() == true)
			{
				report = new TradeReport(application.Database, dialog.EditValue);
				return true;
			}
			else
			{
				report = null;
				return false;
			}
		}

		private static Binding bind(string propertyPath, IValueConverter valueConverter = null)
		{
			var binding = new Binding
			{
				Path = new PropertyPath(propertyPath),
				Mode = BindingMode.OneTime,
			};
			if (valueConverter != null)
			{
				binding.Converter = valueConverter;
			}
			return binding;
		}

		#endregion
	}
}

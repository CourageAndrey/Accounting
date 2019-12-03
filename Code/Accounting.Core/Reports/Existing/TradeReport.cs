using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using ComfortIsland.BusinessLogic;

using Accounting.Core.Helpers;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.Reports.Existing
{
	public class TradeReport : IReport
	{
		#region Properties

		public string Title
		{ get { return string.Format(CultureInfo.InvariantCulture, "Товарный отчёт с {0} по {1}", FromDate.ToLongDateString(), ToDate.ToLongDateString()); } }

		public ReportDescriptor Descriptor
		{ get { return ReportDescriptor.Trade; } }

		public IReadOnlyList<IReportItem> Items
		{ get; private set; }

		public DateTime FromDate
		{ get; private set; }

		public DateTime ToDate
		{ get; private set; }

		#endregion

		public TradeReport(Database database, PeriodParams parameters)
		{
			FromDate = parameters.Period.From.Date;
			ToDate = parameters.Period.To.EndOfDay();
			var items = database.Products.ToDictionary(p => p.ID, p => new TradeItem(p));
			var balance = database.Balance.Clone();
			var activeDocuments = database.GetActiveDocuments().ToList();

			// открутили остатки на конец периода
			foreach (var document in activeDocuments.Where(d => d.Date > ToDate))
			{
				document.RollbackBalanceChanges(balance);
			}
			foreach (var position in balance)
			{
				items[position.Key].FinalBalance = position.Value;
			}

			// обработка всех документов по списку
			foreach (var document in activeDocuments.Where(d => d.Date <= ToDate && d.Date >= FromDate))
			{
				document.RollbackBalanceChanges(balance);
				DocumentTypes[document.Type](items, document.Positions);
			}

			// сохраняем баланс на начало периода
			foreach (var position in balance)
			{
				items[position.Key].InitialBalance = position.Value;
			}

			Items = new ReadOnlyCollection<IReportItem>(items.Values.OfType<IReportItem>().ToList());
		}

		private delegate void UpdateTradeItem(IDictionary<long, TradeItem> items, IDictionary<Product, decimal> positions);
		private static readonly IDictionary<DocumentType, UpdateTradeItem> DocumentTypes = new Dictionary<DocumentType, UpdateTradeItem>
		{
			{
				DocumentType.Income, (items, positions) =>
				{
					foreach (var position in positions)
					{
						items[position.Key.ID].Income += position.Value;
					}
				}
			},
			{
				DocumentType.Outcome, (items, positions) =>
				{
					foreach (var position in positions)
					{
						items[position.Key.ID].Selled += position.Value;
					}
				}
			},
			{
				DocumentType.Produce, (items, positions) =>
				{
					foreach (var position in positions)
					{
						var product = position.Key;
						items[product.ID].Produced += position.Value;
						foreach (var child in product.Children)
						{
							items[child.Key.ID].UsedToProduce += (position.Value * child.Value);
						}
					}
				}
			},
			{
				DocumentType.ToWarehouse, (items, positions) =>
				{
					foreach (var position in positions)
					{
						items[position.Key.ID].SentToWarehouse += position.Value;
					}
				}
			},
		};
	}
}

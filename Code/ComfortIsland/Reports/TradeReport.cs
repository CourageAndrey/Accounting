using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

namespace ComfortIsland.Reports
{
	public class TradeReport : IReport
	{
		#region Properties

		public string Title
		{ get { return string.Format(CultureInfo.InvariantCulture, "Товарный отчёт с {0} по {1}", FromDate.ToLongDateString(), ToDate.ToLongDateString()); } }

		public ReportDescriptor Descriptor
		{ get { return ReportDescriptor.Trade; } }

		public System.Collections.IEnumerable Items
		{ get { return TradeItems; } }

		public DateTime FromDate
		{ get; private set; }

		public DateTime ToDate
		{ get; private set; }

		public IEnumerable<TradeItem> TradeItems
		{ get; private set; }

		#endregion

		public TradeReport(Database database, Period period)
		{
			FromDate = period.From.Date;
			ToDate = period.To.EndOfDay();
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
				DocumentTypes[document.Type.Enum](items, document.Positions);
			}

			// сохраняем баланс на начало периода
			foreach (var position in balance)
			{
				items[position.Key].InitialBalance = position.Value;
			}

			TradeItems = items.Values;
		}

		private delegate void UpdateTradeItem(IDictionary<long, TradeItem> items, IDictionary<Product, decimal> positions);
		private static readonly IDictionary<Xml.DocumentType, UpdateTradeItem> DocumentTypes = new Dictionary<Xml.DocumentType, UpdateTradeItem>
		{
			{
				Xml.DocumentType.Income, (items, positions) =>
				{
					foreach (var position in positions)
					{
						items[position.Key.ID].Income += position.Value;
					}
				}
			},
			{
				Xml.DocumentType.Outcome, (items, positions) =>
				{
					foreach (var position in positions)
					{
						items[position.Key.ID].Selled += position.Value;
					}
				}
			},
			{
				Xml.DocumentType.Produce, (items, positions) =>
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
				Xml.DocumentType.ToWarehouse, (items, positions) =>
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

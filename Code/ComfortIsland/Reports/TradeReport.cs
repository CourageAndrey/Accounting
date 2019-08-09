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

			foreach (var document in activeDocuments.Where(d => d.Date <= ToDate && d.Date >= FromDate))
			{
				document.RollbackBalanceChanges(balance);
				switch (document.Type.Enum)
				{
					case Xml.DocumentType.Income:
						foreach (var position in document.Positions)
						{
							items[position.Key.ID].Income += position.Value;
						}
						break;
					case Xml.DocumentType.Outcome:
						foreach (var position in document.Positions)
						{
							items[position.Key.ID].Selled += position.Value;
						}
						break;
					case Xml.DocumentType.Produce:
						foreach (var position in document.Positions)
						{
							var product = position.Key;
							items[product.ID].Produced += position.Value;
							foreach (var child in product.Children)
							{
								items[child.Key.ID].UsedToProduce += (position.Value * child.Value);
							}
						}
						break;
					case Xml.DocumentType.ToWarehouse:
						foreach (var position in document.Positions)
						{
							items[position.Key.ID].SentToWarehouse += position.Value;
						}
						break;
					default:
						throw new NotSupportedException();
				}
			}

			// сохраняем баланс на начало периода
			foreach (var position in balance)
			{
				items[position.Key].InitialBalance = position.Value;
			}

			TradeItems = items.Values;
		}
	}
}

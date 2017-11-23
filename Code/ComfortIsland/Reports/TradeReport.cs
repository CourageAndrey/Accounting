﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ComfortIsland.Database;

namespace ComfortIsland.Reports
{
	public class TradeReport : IReport
	{
		#region Properties

		public string Title
		{ get { return string.Format(CultureInfo.InvariantCulture, "Товарный отчёт с {0:D} по {1:D}", FromDate, ToDate); } }

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

		public TradeReport(DateTime fromDate, DateTime toDate)
		{
			FromDate = fromDate.Date;
			ToDate = toDate.Date.AddDays(1).AddMilliseconds(-1);
			var database = Database.Database.Instance;
			var items = database.Products.ToDictionary(p => p.ID, p => new TradeItem(p));
			var balanceList = database.Balance.Select(b => new Balance(b)).ToList();

			// открутили остатки на конец периода
			foreach (var document in database.Documents.Where(d => d.Date > ToDate).OrderByDescending(d => d.Date))
			{
				document.ProcessBack(balanceList);
			}
			foreach (var balance in balanceList)
			{
				items[balance.ProductId].FinalBalance = balance.Count;
			}

			foreach (var document in database.Documents.Where(d => d.Date <= ToDate && d.Date >= FromDate).OrderByDescending(d => d.Date))
			{
				document.ProcessBack(balanceList);
				switch (document.Type)
				{
					case DocumentType.Income:
						foreach (var position in document.Positions)
						{
							items[position.Key.ID].Income += position.Value;
						}
						break;
					case DocumentType.Outcome:
						foreach (var position in document.Positions)
						{
							items[position.Key.ID].Selled += position.Value;
						}
						break;
					case DocumentType.Produce:
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
					default:
						throw new NotSupportedException();
				}
			}

			// сохраняем баланс на начало периода
			foreach (var balance in balanceList)
			{
				items[balance.ProductId].InitialBalance = balance.Count;
			}

			TradeItems = items.Values;
		}
	}

	public class TradeItem
	{
		#region Properties

		public long ProductId
		{ get; private set; }

		public string ProductCode
		{ get; private set; }

		public string ProductName
		{ get; private set; }

		public string ProductUnit
		{ get; private set; }

		public double InitialBalance
		{ get; internal set; }

		public double Income
		{ get; internal set; }

		public double Produced
		{ get; internal set; }

		public double Selled
		{ get; internal set; }

		public double UsedToProduce
		{ get; internal set; }

		public double FinalBalance
		{ get; internal set; }

		#endregion

		public TradeItem(Product product)
		{
			ProductId = product.ID;
			ProductCode = product.Code;
			ProductName = product.Name;
			ProductUnit = product.Unit.Name;
			InitialBalance = Income = Produced = Selled = UsedToProduce = FinalBalance = 0;
		}
	}
}

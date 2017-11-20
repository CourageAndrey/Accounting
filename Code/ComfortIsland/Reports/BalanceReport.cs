﻿using System;
using System.Collections.Generic;
using System.Linq;

using ComfortIsland.Database;

namespace ComfortIsland.Reports
{
	public class BalanceReport : IReport
	{
		#region Properties

		public string Title
		{ get { return "Складские остатки на конец дня " + Date.ToLongDateString(); } }

		public ReportDescriptor Descriptor
		{ get { return ReportDescriptor.Balance; } }

		public System.Collections.IEnumerable Items
		{ get { return BalanceItems; } }

		public DateTime Date
		{ get; private set; }

		public IEnumerable<Balance> BalanceItems
		{ get; private set; }

		#endregion

		public BalanceReport(DateTime date)
		{
			Date = date.Date.AddDays(1).AddMilliseconds(-1);
			var database = Database.Database.Instance;
			var balanceList = database.Balance.Select(b => new Balance(b)).ToList();

			foreach (var document in database.Documents.Where(d => d.Date > Date).OrderByDescending(d => d.Date))
			{
				DocumentTypeImplementation.AllTypes[document.Type].ProcessBack(document, balanceList);
			}

			BalanceItems = balanceList;
		}
	}
}

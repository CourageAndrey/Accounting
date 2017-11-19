using System;
using System.Collections.Generic;
using System.Linq;

using ComfortIsland.Database;

namespace ComfortIsland.Reports
{
	public class BalanceReport : IReport
	{
		#region Properties

		public System.Collections.IEnumerable Items
		{ get { return BalanceItems; } }

		public DateTime Date
		{ get; private set; }

		public IEnumerable<Balance> BalanceItems
		{ get; private set; }

		#endregion

		public BalanceReport(DateTime date)
		{
			Date = date.AddDays(1).AddSeconds(-1).Date;
			BalanceItems = Database.Database.Instance.Balance.ToList();
		}
	}
}

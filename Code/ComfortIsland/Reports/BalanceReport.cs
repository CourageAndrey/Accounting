using System;
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
			Date = date.AddDays(1).AddSeconds(-1).Date;
#warning Реализовать правильный откат документов по конкретным датам.
			BalanceItems = Database.Database.Instance.Balance.ToList();
		}
	}
}

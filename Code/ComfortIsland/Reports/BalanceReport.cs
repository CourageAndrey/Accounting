using System;
using System.Collections.Generic;
using System.Linq;

using ComfortIsland.BusinessLogic;

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

		public IEnumerable<Position> BalanceItems
		{ get; private set; }

		#endregion

		public BalanceReport(Database database, DateTime date, bool showAllProducts)
		{
			Date = date.Date.AddDays(1).AddMilliseconds(-1);
			var balanceList = database.Balance.Select(b => new Position(b.Key, b.Value)).ToList();
			var activeDocuments = database.Documents.Where(d => d.State == DocumentState.Active).OrderByDescending(d => d.Date).ToList();

			foreach (var document in activeDocuments.Where(d => d.Date > Date))
			{
				document.Rollback(database);
			}

			var products = database.Products.ToDictionary(product => product.ID, product => (double?) null);
			foreach (var balance in balanceList)
			{
				products[balance.ID] = balance.Count;
			}

			BalanceItems = products
				.Where(item => showAllProducts || item.Value > 0)
				.Select(item =>
				{
					var position = new Position(item.Key, item.Value.HasValue ? item.Value.Value : 0);
					position.SetProduct(database);
					return position;
				})
				.ToList();
		}
	}
}

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

			var databaseMock = new Database(
				new Unit[0],
				new Product[0],
				database.Balance.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
				new Document[0]);
			var activeDocuments = database.GetActiveDocuments().ToList();

			foreach (var document in activeDocuments.Where(d => d.Date > Date))
			{
				document.Rollback(databaseMock);
			}

			var products = database.Products.ToDictionary(product => product.ID, product => (decimal?) null);
			foreach (var position in databaseMock.Balance)
			{
				products[position.Key] = position.Value;
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

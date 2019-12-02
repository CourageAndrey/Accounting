using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;
using ComfortIsland.Reports.Params;

namespace ComfortIsland.Reports
{
	public class BalanceReport : IReport
	{
		#region Properties

		public string Title
		{ get { return "Складские остатки на конец дня " + Date.ToLongDateString(); } }

		public ReportDescriptor Descriptor
		{ get { return ReportDescriptor.Balance; } }

		public IReadOnlyList<IReportItem> Items
		{ get; private set; }

		public DateTime Date
		{ get; private set; }

		#endregion

		public BalanceReport(Database database, BalanceReportParams parameters)
		{
			Date = parameters.Date.EndOfDay();

			var balance = database.Balance.Clone();
			var activeDocuments = database.GetActiveDocuments().ToList();

			foreach (var document in activeDocuments.Where(d => d.Date > Date))
			{
				document.RollbackBalanceChanges(balance);
			}

			var products = database.Products.ToDictionary(product => product.ID, product => (decimal?) null);
			foreach (var position in balance)
			{
				products[position.Key] = position.Value;
			}

			Items = new ReadOnlyCollection<IReportItem>(products
				.Where(item => parameters.IncludeAllProducts || item.Value > 0)
				.Select(item =>
				{
					var position = new Position(item.Key, item.Value.HasValue ? item.Value.Value : 0);
					position.SetProduct(database);
					return position as IReportItem;
				})
				.ToList());
		}
	}
}

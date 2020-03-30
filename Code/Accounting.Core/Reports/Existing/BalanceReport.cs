using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Accounting.Core.BusinessLogic;
using Accounting.Core.Helpers;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.Reports.Existing
{
	public class BalanceReport : IReport
	{
		#region Properties

		public string Title
		{ get { return "Складские остатки на конец дня " + Date.ToLongDateString(); } }

		public ReportDescriptor Descriptor
		{ get { return ReportDescriptor.Balance; } }

		public IReadOnlyList<IReportItem> Items
		{ get; }

		public DateTime Date
		{ get; }

		#endregion

		public BalanceReport(IDatabase database, BalanceReportParams parameters)
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

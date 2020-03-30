using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Accounting.Core.BusinessLogic;
using Accounting.Core.Helpers;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.Reports.Existing
{
	public class FinanceReport : IReport
	{
		#region Properties

		public string Title
		{ get { return string.Format(CultureInfo.InvariantCulture, "Финансовый отчёт с {0} по {1}", FromDate.ToLongDateString(), ToDate.ToLongDateString()); } }

		public ReportDescriptor Descriptor
		{ get { return ReportDescriptor.Finance; } }

		public IReadOnlyList<IReportItem> Items
		{ get; }

		public DateTime FromDate
		{ get; }

		public DateTime ToDate
		{ get; }

		#endregion

		public FinanceReport(IDatabase database, PeriodParams parameters)
		{
			FromDate = parameters.Period.From.Date;
			ToDate = parameters.Period.To.EndOfDay();
			var activeDocuments = database.GetActiveDocuments().Where(d => d.Date <= ToDate && d.Date >= FromDate);
			TradeItem balance = new TradeItem(new Product { Unit = new Unit() });

			foreach (var document in activeDocuments)
			{
				DocumentTypes[document.Type](balance, document);
			}

			Items = new IReportItem[]
			{
				new FinanceItem("Всего приобретено на", balance.Income),
				new FinanceItem("Всего продано на", balance.Selled),
			};
		}

		private delegate void UpdateTradeItem(TradeItem balance, Document document);
		private static readonly IDictionary<DocumentType, UpdateTradeItem> DocumentTypes = new Dictionary<DocumentType, UpdateTradeItem>
		{
			{
				DocumentType.Income, (balance, document) =>
				{
					balance.Income += document.Summ;
				}
			},
			{
				DocumentType.Outcome, (balance, document) =>
				{
					balance.Selled += document.Summ;
				}
			},
			{
				DocumentType.Produce, (balance, document) =>
				{
					balance.Produced += document.Summ;
				}
			},
			{
				DocumentType.ToWarehouse, (balance, document) =>
				{
					balance.SentToWarehouse += document.Summ;
				}
			},
		};
	}
}

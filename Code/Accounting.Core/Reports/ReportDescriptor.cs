using System.Collections.Generic;
using System.Collections.ObjectModel;

using Accounting.Core.Application;

namespace ComfortIsland.Reports
{
	public abstract class ReportDescriptor
	{
		public abstract string Title
		{ get; }

		public abstract IEnumerable<ReportColumn> GetColumns();

		public abstract bool CreateReport(IAccountingApplication application, out IReport report);

		#region Список

		public static readonly ReportDescriptor Balance = new BalanceReportDescriptor();

		public static readonly ReportDescriptor Trade = new TradeReportDescriptor();

		public static readonly IEnumerable<ReportDescriptor> All = new ReadOnlyCollection<ReportDescriptor>(new List<ReportDescriptor>
		{
			Balance,
			Trade,
		});

		#endregion
	}
}

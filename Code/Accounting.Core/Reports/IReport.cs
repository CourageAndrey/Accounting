using System.Collections.Generic;

namespace Accounting.Core.Reports
{
	public interface IReport
	{
		IReadOnlyList<IReportItem> Items { get; }

		string Title { get; }

		ReportDescriptor Descriptor { get; }
	}
}

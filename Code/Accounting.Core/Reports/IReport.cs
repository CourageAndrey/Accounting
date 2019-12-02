using System.Collections.Generic;

using ComfortIsland.Reports;

namespace ComfortIsland
{
	public interface IReport
	{
		IReadOnlyList<IReportItem> Items { get; }

		string Title { get; }

		ReportDescriptor Descriptor { get; }
	}
}

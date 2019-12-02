using System.Collections;

using ComfortIsland.Reports;

namespace ComfortIsland
{
	public interface IReport
	{
		IEnumerable Items { get; }

		string Title { get; }

		ReportDescriptor Descriptor { get; }
	}
}

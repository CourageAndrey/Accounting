using System.Collections;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Reports;

namespace ComfortIsland
{
	public interface IEditDialog<T> : IAccountingApplicationClient
	{
		T EditValue { get; set; }
	}

	public interface IReport
	{
		IEnumerable Items { get; }

		string Title { get; }

		ReportDescriptor Descriptor { get; }
	}

	public interface IViewModel<out T>
	{
		long? ID
		{ get; }

		T ConvertToBusinessLogic(Database database);
	}
}

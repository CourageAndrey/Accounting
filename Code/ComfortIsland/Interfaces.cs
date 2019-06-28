using System.Collections;
using System.Text;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Reports;

namespace ComfortIsland
{
	public interface IEntity
	{
		long ID { get; set; }

		StringBuilder FindUsages(Database database);

		bool Validate(Database database, out StringBuilder errors);
	}

	public interface IEditDialog<T>
	{
		T EditValue { get; set; }

		void Initialize(Database database);
	}

	public interface IReport
	{
		IEnumerable Items { get; }

		string Title { get; }

		ReportDescriptor Descriptor { get; }
	}

	public interface IListBoxItem
	{
		string DisplayMember { get; }
	}
 
	public interface IViewModel<T>
	{
		T ConvertToBusinessLogic(Database database);
	}
}

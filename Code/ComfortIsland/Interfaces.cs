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
 
	public interface IViewModel<out T>
	{
		long? ID
		{ get; }

		T ConvertToBusinessLogic(Database database);
	}

	public interface IDatabaseDriver
	{
		void Save(Database database);

		Database TryLoad();
	}
}

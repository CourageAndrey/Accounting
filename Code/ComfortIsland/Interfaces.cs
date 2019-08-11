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

	public interface IEditDialog<T> : IApplicationClient
	{
		T EditValue { get; set; }
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

	public interface IApplication
	{
		string StartupPath { get; }

		Settings Settings { get; }

		Database Database { get; }
	}

	public interface IApplicationClient
	{
		void ConnectTo(IApplication application);
	}
}

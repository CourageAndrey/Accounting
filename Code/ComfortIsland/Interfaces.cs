using System.Collections;
using System.Text;

using ComfortIsland.Reports;

namespace ComfortIsland
{
	public interface IEntity
	{
		long ID { get; set; }
	}

	public interface IEditable<in T>
	{
		void Update(T other);

		bool Validate(Database.Database database, out StringBuilder errors);

		void BeforeSerialization();

		void AfterDeserialization(Database.Database database);

		void BeforeEdit();

		void AfterEdit();
	}

	public interface IEditDialog<T>
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
}

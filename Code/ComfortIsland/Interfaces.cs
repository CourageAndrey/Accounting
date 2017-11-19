using System.Text;

namespace ComfortIsland
{
	public interface IEntity
	{
		long ID { get; set; }
	}

	public interface IEditable<in T>
	{
		void Update(T other);

		bool Validate(out StringBuilder errors);

		void BeforeSerialization();

		void AfterDeserialization();

		void BeforeEdit();

		void AfterEdit();
	}

	interface IEditDialog<T>
	{
		T EditValue { get; set; }
	}
}

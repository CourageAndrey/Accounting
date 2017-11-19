using System.Text;

namespace ComfortIsland.Database
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
}

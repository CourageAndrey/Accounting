using System.Text;

using ComfortIsland.Database;

namespace ComfortIsland
{
	public interface IEditable<T>
	{
		void Update(T other);

		bool Validate(ComfortIslandDatabase database, out StringBuilder errors);

		T PrepareToDisplay(ComfortIslandDatabase database);

		void PrepareToSave(ComfortIslandDatabase database);
	}
}

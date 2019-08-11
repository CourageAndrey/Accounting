using ComfortIsland.BusinessLogic;

namespace ComfortIsland.DataAccessLayer
{
	public interface IDatabaseDriver
	{
		void Save(Database database);

		Database TryLoad();
	}
}

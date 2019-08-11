using ComfortIsland.DataAccessLayer.Xml;

namespace ComfortIsland.DataAccessLayer
{
	public static class DatabaseDriverFactory
	{
		public static IDatabaseDriver CreateDatabaseDriver(string databaseFileName)
		{
			return new DatabaseDriver(databaseFileName);
		}
	}
}

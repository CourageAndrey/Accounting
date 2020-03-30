using Accounting.Core.BusinessLogic;

namespace Accounting.Core.Application
{
	public static class DatabaseDriverExtensions
	{
		public static IDatabase TryLoad(this IDatabaseDriver databaseDriver)
		{
			IDatabase database;
			if (databaseDriver.CanLoad)
			{
				database = databaseDriver.Load();
			}
			else
			{
				database = Database.CreateDefault();
				databaseDriver.Save(database);
			}
			return database;
		}
	}
}
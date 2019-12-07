using Accounting.Core.BusinessLogic;

namespace Accounting.Core.Application
{
	public static class DatabaseDriverExtensions
	{
		public static Database TryLoad(this IDatabaseDriver databaseDriver)
		{
			Database database;
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
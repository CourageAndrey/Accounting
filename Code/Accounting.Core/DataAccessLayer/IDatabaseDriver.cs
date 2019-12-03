using Accounting.Core.BusinessLogic;

namespace Accounting.Core.DataAccessLayer
{
	public interface IDatabaseDriver
	{
		bool CanLoad { get; }

		Database Load();

		void Save(Database database);
	}
}

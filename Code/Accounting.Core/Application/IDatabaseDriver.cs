using Accounting.Core.BusinessLogic;

namespace Accounting.Core.Application
{
	public interface IDatabaseDriver
	{
		bool CanLoad { get; }

		IDatabase Load();

		void Save(IDatabase database);
	}
}

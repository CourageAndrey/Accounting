using Accounting.Core.Application;
using Accounting.Core.Configuration.Xml;

namespace Accounting.DAL.EntityFramework
{
	internal class EntityFrameworkDatabasePlugin : IAccountingPlugin
	{
		public void Setup(IAccountingApplication application)
		{
			InternalEnginesExtensions.RegisterDatabaseEngine<EntityFrameworkDatabaseDriver>("EntityFrameworkDatabaseDriver");
		}
	}
}

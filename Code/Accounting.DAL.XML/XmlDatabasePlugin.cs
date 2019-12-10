using Accounting.Core.Application;
using Accounting.Core.Configuration.Xml;

namespace Accounting.DAL.XML
{
	internal class XmlDatabasePlugin : IAccountingPlugin
	{
		public void Setup(IAccountingApplication application)
		{
			InternalEnginesExtensions.RegisterDatabaseEngine<XmlDatabaseDriver>("XmlDatabaseDriver");
		}
	}
}

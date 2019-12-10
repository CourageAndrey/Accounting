using Accounting.Core.Application;
using Accounting.Core.Configuration.Xml;

namespace Accounting.UI.WPF
{
	internal class WpfUiPlugin : IAccountingPlugin
	{
		public void Setup(IAccountingApplication application)
		{
			InternalEnginesExtensions.RegisterUserInterfaceEngine<WpfUserInterfaceDriver>("WpfUserInterfaceDriver");
		}
	}
}

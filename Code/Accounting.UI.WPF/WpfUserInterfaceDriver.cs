using Accounting.Core.Application;
using Accounting.Core.Configuration.Xml;

namespace Accounting.UI.WPF
{
	public class WpfUserInterfaceDriver : UserInterfaceDriver
	{
		public override IUserInterface CreateUserInterface()
		{
			return new WpfUserInterface();
		}
	}
}

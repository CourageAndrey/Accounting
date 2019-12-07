using Accounting.Core.Application;

namespace Accounting.Core.Configuration
{
	public class UserInterfaceSettings
	{
		public double FontSize
		{ get; }

		public IUserInterface Engine
		{ get; }

		internal UserInterfaceSettings(Xml.UserInterfaceSettings xmlSettings)
		{
			FontSize = xmlSettings.FontSize;

			Engine = xmlSettings.UserInterfaceDriver.CreateUserInterface();
		}
	}
}

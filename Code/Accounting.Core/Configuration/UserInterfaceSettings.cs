namespace Accounting.Core.Configuration
{
	public class UserInterfaceSettings
	{
		public double FontSize
		{ get; }

		internal UserInterfaceSettings(Xml.UserInterfaceSettings xmlSettings)
		{
			FontSize = xmlSettings.FontSize;
		}
	}
}

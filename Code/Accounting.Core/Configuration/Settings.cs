namespace Accounting.Core.Configuration
{
	public class Settings
	{
		#region Свойства

		public UserInterfaceSettings UserInterface
		{ get; }

		public DataAccessLayerSettings DataAccessLayer
		{ get; }

		public BusinessLogicSettings BusinessLogic
		{ get; }

		public ReportingSettings Reporting
		{ get; }

		#endregion

		public Settings(Xml.Settings xmlSettings)
		{
			UserInterface = new UserInterfaceSettings(xmlSettings.UserInterface);
			DataAccessLayer = new DataAccessLayerSettings(xmlSettings.DataAccessLayer);
			BusinessLogic = new BusinessLogicSettings(xmlSettings.BusinessLogic);
			Reporting = new ReportingSettings(xmlSettings.Reporting);
		}
	}
}

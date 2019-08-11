namespace ComfortIsland.Configuration
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

		#endregion

		internal Settings(Xml.Settings xmlSettings)
		{
			UserInterface = new UserInterfaceSettings(xmlSettings.UserInterface);
			DataAccessLayer = new DataAccessLayerSettings(xmlSettings.DataAccessLayer);
			BusinessLogic = new BusinessLogicSettings(xmlSettings.BusinessLogic);
		}
	}
}

using Accounting.Core.DataAccessLayer;

namespace Accounting.Core.Configuration
{
	public class DataAccessLayerSettings
	{
		public IDatabaseDriver DatabaseDriver
		{ get; }

		internal DataAccessLayerSettings(Xml.DataAccessLayerSettings xmlSettings)
		{
			DatabaseDriver = xmlSettings.DatabaseDriver.CreateDataAccessLayer();
		}
	}
}

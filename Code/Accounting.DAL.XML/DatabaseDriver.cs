using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ComfortIsland.DataAccessLayer.Xml
{
	public class DatabaseDriver : IDatabaseDriver
	{
		private readonly string _filePath;
		private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(Database));

		internal DatabaseDriver(string filePath)
		{
			_filePath = filePath;
		}

		public bool CanLoad
		{ get { return File.Exists(_filePath); } }

		public BusinessLogic.Database Load()
		{
			using (var xmlReader = XmlReader.Create(_filePath))
			{
				return ((Database) _xmlSerializer.Deserialize(xmlReader)).ConvertToBusinessLogic();
			}
		}

		public void Save(BusinessLogic.Database database)
		{
			var snapshot = new Database(database);
			var xmlDocument = new XmlDocument();
			using (var writer = new StringWriter())
			{
				_xmlSerializer.Serialize(writer, snapshot);
				xmlDocument.LoadXml(writer.ToString());
				xmlDocument.Save(_filePath);
			}
		}
	}
}

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ComfortIsland.Xml
{
	public class DatabaseDriver : IDatabaseDriver
	{
		private readonly string _filePath;
		private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(Database));

		public DatabaseDriver(string filePath)
		{
			_filePath = filePath;
		}

		private BusinessLogic.Database load()
		{
			using (var xmlReader = XmlReader.Create(_filePath))
			{
				return ((Database) _xmlSerializer.Deserialize(xmlReader)).ConvertToBusinessLogic();
			}
		}

		public BusinessLogic.Database TryLoad()
		{
			BusinessLogic.Database database;
			if (File.Exists(_filePath))
			{
				database = load();
			}
			else
			{
				database = BusinessLogic.Database.CreateDefault();
				Save(database);
			}
			return database;
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

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Accounting.Core.Application;

namespace Accounting.DAL.XML
{
	public class DatabaseDriver : IDatabaseDriver
	{
		private readonly string _filePath;
		private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(Entities.Database));

		internal DatabaseDriver(string filePath)
		{
			_filePath = filePath;
		}

		public bool CanLoad
		{ get { return File.Exists(_filePath); } }

		public Core.BusinessLogic.Database Load()
		{
			using (var xmlReader = XmlReader.Create(_filePath))
			{
				return ((Entities.Database) _xmlSerializer.Deserialize(xmlReader)).ConvertToBusinessLogic();
			}
		}

		public void Save(Core.BusinessLogic.Database database)
		{
			var snapshot = new Entities.Database(database);
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

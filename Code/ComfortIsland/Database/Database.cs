using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ComfortIsland.Database
{
	[XmlType, XmlRoot]
	public class Database
	{
		#region Properties

		[XmlArray("Documents"), XmlArrayItem("Document")]
		public List<Document> Documents
		{ get; set; }

		[XmlArray("Balance"), XmlArrayItem("Item")]
		public List<Balance> Balance
		{ get; set; }

		[XmlArray("Products"), XmlArrayItem("Product")]
		public List<Product> Products
		{ get; set; }

		[XmlArray("Units"), XmlArrayItem("Unit")]
		public List<Unit> Units
		{ get; set; }

		#endregion

		public Database()
		{
			Documents = new List<Document>();
			Balance = new List<Balance>();
			Products = new List<Product>();
			Units = new List<Unit>();
		}

		#region [De]Serialization

		[XmlIgnore]
		private static readonly string filePath;
		[XmlIgnore]
		private static readonly XmlSerializer xmlSerializer;

		static Database()
		{
			filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.xml");
			xmlSerializer = new XmlSerializer(typeof(Database));
		}

		public static Database TryLoad()
		{
			Database database;
			if (File.Exists(filePath))
			{
				using (var xmlReader = XmlReader.Create(filePath))
				{
					database = (Database) xmlSerializer.Deserialize(xmlReader);
					database.AfterDeserialization();
				}
			}
			else
			{
				database = new Database
				{
					Units =
					{
						new Unit{ ID = 1, Name = "штука", ShortName = "шт" },
						new Unit{ ID = 2, Name = "метр погонный", ShortName = "м/пог" },
					}
				};
				database.Save();
			}
			return database;
		}

		public void Save()
		{
			BeforeSerialization();
			var document = new XmlDocument();
			using (var writer = new StringWriter())
			{
				xmlSerializer.Serialize(writer, this);
				document.LoadXml(writer.ToString());
				document.Save(filePath);
			}
		}

		private void BeforeSerialization()
		{
			foreach (var product in Products)
			{
				product.BeforeSerialization();
			}
			foreach (var document in Documents)
			{
				document.BeforeSerialization();
			}
		}

		private void AfterDeserialization()
		{
			foreach (var product in Products)
			{
				product.AfterDeserialization(this);
			}
			foreach (var document in Documents)
			{
				document.AfterDeserialization(this);
			}
			foreach (var balance in Balance)
			{
				balance.AfterDeserialization(this);
			}
		}

		#endregion
	}
}

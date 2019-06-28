using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ComfortIsland.Xml
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

		#region Constructors

		public Database()
		{
			Documents = new List<Document>();
			Balance = new List<Balance>();
			Products = new List<Product>();
			Units = new List<Unit>();
		}

		public Database(BusinessLogic.Database database)
		{
			Documents = database.Documents.Values.Select(document => new Document(document)).ToList();
			Balance = database.Balance.Select(balance => new Balance(balance.Key, balance.Value)).ToList();
			Products = database.Products.Values.Select(product => new Product(product)).ToList();
			Units = database.Units.Values.Select(unit => new Unit(unit)).ToList();
		}

		#endregion

		public BusinessLogic.Database ConvertToBusinessLogic()
		{
			var database = new BusinessLogic.Database(
				Units.ToDictionary(
					unit => unit.ID,
					unit => unit.ConvertToBusinessLogic()),
				Products.ToDictionary(
					product => product.ID,
					product => product.ConvertToBusinessLogic()),
				Balance.ToDictionary(
					balance => balance.Product,
					balance => balance.Count),
				Documents.ToDictionary(
					document => document.ID,
					document => document.ConvertToBusinessLogic()));

			foreach (var product in Products)
			{
				var dbProduct = database.Products[product.ID];
				dbProduct.Unit = database.Units[product.UnitID];
				dbProduct.Children = product.Children.ToDictionary(
					child => database.Products[child.ID],
					child => child.Count);
			}

			foreach (var document in Documents)
			{
				database.Documents[document.ID].Positions = document.Positions.ToDictionary(
					position => database.Products[position.ID],
					position => position.Count);
			}

			return database;
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
				}
			}
			else
			{
				database = new Database
				{
					Units =
					{
						new Unit { ID = 1, Name = "штука", ShortName = "шт" },
						new Unit { ID = 2, Name = "метр погонный", ShortName = "м/пог" },
					}
				};
				database.Save();
			}
			return database;
		}

		public void Save()
		{
			var document = new XmlDocument();
			using (var writer = new StringWriter())
			{
				xmlSerializer.Serialize(writer, this);
				document.LoadXml(writer.ToString());
				document.Save(filePath);
			}
		}

		#endregion
	}
}

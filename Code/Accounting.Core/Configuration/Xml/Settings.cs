using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Accounting.Core.Configuration.Extensions;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public class Settings
	{
		#region Свойства

		[XmlElement]
		public UserInterfaceSettings UserInterface
		{ get; set; }

		[XmlElement]
		public DataAccessLayerSettings DataAccessLayer
		{ get; set; }

		[XmlElement]
		public BusinessLogicSettings BusinessLogic
		{ get; set; }

		#endregion

		#region Сериализация

		private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(Settings), getXmlAttributeOverrides());

		private static XmlAttributeOverrides getXmlAttributeOverrides()
		{
			var databaseDriverTypeAttributes = new XmlAttributes();
			foreach (var implementation in DatabaseDriverExtensions.GetRegisteredImplementations())
			{
				databaseDriverTypeAttributes.XmlElements.Add(new XmlElementAttribute(implementation.Key, implementation.Value));
			}
			var overrides = new XmlAttributeOverrides();
			overrides.Add(typeof(DataAccessLayerSettings), "DatabaseDriver", databaseDriverTypeAttributes);
			return overrides;
		}

		public static Settings Load(string startupPath)
		{
			string fileName = Path.Combine(startupPath, "Settings.xml");

			XmlReader xmlReader;
			try
			{
				xmlReader = XmlReader.Create(fileName);
			}
			catch (FileNotFoundException)
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(DefaultConfigurationXml);
				xmlDocument.Save(fileName);
				xmlReader = XmlReader.Create(fileName);
			}

			using (xmlReader)
			{
				return (Settings) _xmlSerializer.Deserialize(xmlReader);
			}
		}

		private const string DefaultConfigurationXml =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Settings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
	<!-- Данный раздел хранит настройки пользовательского интерфейса -->
	<UserInterface>
		<!-- Размер шрифтов, используемых в окнах программы. Можно увеличить либо умешьшить для полкчение комфортного вида всех текстовых надписей. Значение по умолчанию - 16. -->
		<FontSize>16</FontSize>
	</UserInterface>

	<!-- Данный раздел хранит настройки пользовательского интерфейса -->
	<DataAccessLayer>
		<!-- Драйвер доступа к базе данных. В настоящий момент есть только один - хранение базы в XML-файле с заданным именем. Имя может включать "".."" и наклонную черту. Значение по умолчанию - Database.xml. -->
		<XmlDatabaseDriver FileName=""Database.xml"" />
	</DataAccessLayer>

	<!-- Данный раздел хранит настройки бизнес-логики и правил -->
	<BusinessLogic>
		<!-- Правила проверки баланса при создании, редактировании и удалении документов. Доступны следующие режимы:
			 * PerDocument - баланс по всем позициям должен быть неотрицательный после каждого документа;
			 * PerDay - баланс по всем позициям должен быть неотрицательный в конце каждого дня (но в процессе дня может уходить в минус);
			 * FinalOnly - итоговой баланс по всем позициям должен быть неотрицательный (но за некоторые дни может уходить в минус);
			 * NoVerify - проверка баланса не производится вообще, на складе может образоваться отрицательное количество товара.
			Значение по умолчанию - FinalOnly.-->
		<BalanceValidationStrategy>FinalOnly</BalanceValidationStrategy>
	</BusinessLogic>
</Settings>";

		#endregion
	}
}

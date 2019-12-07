using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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

		[XmlElement]
		public ReportingSettings Reporting
		{ get; set; }

		#endregion

		#region Сериализация

		private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(Settings), getXmlAttributeOverrides());

		private static XmlAttributeOverrides getXmlAttributeOverrides()
		{
			var overrides = new XmlAttributeOverrides();

			registerExtensions(overrides, InternalEnginesExtensions.GetRegisteredDatabaseEngines(), typeof(DataAccessLayerSettings), "DatabaseDriver");
			registerExtensions(overrides, InternalEnginesExtensions.GetRegisteredReportExportEngines(), typeof(ReportingSettings), "ReportExportDriver");
			registerExtensions(overrides, InternalEnginesExtensions.GetRegisteredUserInterfaceEngines(), typeof(UserInterfaceSettings), "UserInterfaceDriver");

			return overrides;
		}

		private static void registerExtensions(XmlAttributeOverrides overrides, IReadOnlyDictionary<string, Type> implementations, Type type, string propertyName)
		{
			var attributes = new XmlAttributes();
			foreach (var implementation in implementations)
			{
				attributes.XmlElements.Add(new XmlElementAttribute(implementation.Key, implementation.Value));
			}
			overrides.Add(type, propertyName, attributes);
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

		<!-- Движок пользовательского интерфеса - не изменять этот параметр! -->
		<WpfUserInterfaceDriver />
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

	<!-- Данный раздел хранит настройки подсистемы отчётов -->
	<Reporting>
		<!-- Движок эскпорта отчётов в формат Excel Open XML - не изменять этот параметр! -->
		<ExcelOpenXmlReportExportDriver />
	</Reporting>
</Settings>";

		#endregion
	}
}

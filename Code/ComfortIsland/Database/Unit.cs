using System.Text;
using System.Xml.Serialization;

namespace ComfortIsland.Database
{
	[XmlType]
	public class Unit : IEntity, IEditable<Unit>
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlAttribute]
		public string Name
		{ get; set; }

		[XmlAttribute]
		public string ShortName
		{ get; set; }

		#endregion

		public void Update(Unit other)
		{
			this.ID = other.ID;
			this.Name = other.Name;
			this.ShortName = other.ShortName;
		}

		public bool Validate(Database database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (string.IsNullOrEmpty(Name))
			{
				errors.AppendLine("Наименование не может быть пустой строкой.");
			}
			if (string.IsNullOrEmpty(ShortName))
			{
				errors.AppendLine("Сокращение не может быть пустой строкой.");
			}
			return errors.Length == 0;
		}

		#region [De]Serialization

		public void BeforeSerialization()
		{ }

		public void AfterDeserialization(Database database)
		{ }

		#endregion

		public void BeforeEdit()
		{ }

		public void AfterEdit()
		{ }
	}
}

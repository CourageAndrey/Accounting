using System.Xml.Serialization;

namespace ComfortIsland.Xml
{
	[XmlType]
	public class Unit
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

		#region Constructors

		public Unit()
		{ }

		public Unit(BusinessLogic.Unit unit)
		{
			ID = unit.ID;
			Name = unit.Name;
			ShortName = unit.ShortName;
		}

		#endregion

		public BusinessLogic.Unit ConvertToBusinessLogic()
		{
			return new BusinessLogic.Unit
			{
				ID = ID,
				Name = Name,
				ShortName = ShortName,
			};
		}
	}
}

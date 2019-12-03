using System.Xml.Serialization;

namespace ComfortIsland.DataAccessLayer.Xml
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

		public Unit(Accounting.Core.BusinessLogic.Unit unit)
		{
			ID = unit.ID;
			Name = unit.Name;
			ShortName = unit.ShortName;
		}

		#endregion

		public Accounting.Core.BusinessLogic.Unit ConvertToBusinessLogic()
		{
			return new Accounting.Core.BusinessLogic.Unit
			{
				ID = ID,
				Name = Name,
				ShortName = ShortName,
			};
		}
	}
}

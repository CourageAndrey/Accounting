using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ComfortIsland.Xml
{
	[XmlType]
	public class Product
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlAttribute]
		public string Name
		{ get; set; }

		[XmlAttribute]
		public long UnitID
		{ get; set; }

		[XmlArray("Children"), XmlArrayItem("Product")]
		public List<Position> Children
		{ get; set; }

		#endregion

		#region Constructors

		public Product()
		{
			Children = new List<Position>();
		}

		public Product(BusinessLogic.Product product)
		{
			ID = product.ID;
			Name = product.Name;
			UnitID = product.UnitID;
			Children = product.ChildrenToSerialize.Select(child => new Position(child)).ToList();
		}

		#endregion

		public BusinessLogic.Product ConvertToBusinessLogic()
		{
			return new BusinessLogic.Product
			{
				ID = ID,
				Name = Name,
				UnitID = UnitID,
				ChildrenToSerialize = Children.Select(child => child.ConvertToBusinessLogic()).ToList(),
			};
		}
	}
}

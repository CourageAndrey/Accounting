using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Accounting.DAL.XML.Entities
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

		public Product(Accounting.Core.BusinessLogic.Product product)
		{
			ID = product.ID;
			Name = product.Name;
			UnitID = product.Unit.ID;
			Children = product.Children.Select(child => new Position(new Accounting.Core.BusinessLogic.Position(child.Key.ID, child.Value))).ToList();
		}

		#endregion

		public Accounting.Core.BusinessLogic.Product ConvertToBusinessLogic()
		{
			return new Accounting.Core.BusinessLogic.Product
			{
				ID = ID,
				Name = Name,
			};
		}
	}
}

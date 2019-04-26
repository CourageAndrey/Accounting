using System.Linq;
using System.Xml.Serialization;

namespace ComfortIsland.BusinessLogic
{
	[XmlType]
	public class Position
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlAttribute]
		public double Count
		{ get; set; }

		[XmlIgnore]
		public Product BoundProduct
		{ get; private set; }

		#endregion

		#region Constructors

		public Position()
		{ }

		public Position(long id, double count)
		{
			ID = id;
			Count = count;
		}

		public Position(Position other)
		{
			ID = other.ID;
			Count = other.Count;
		}

		#endregion

		internal void SetProduct(Database database)
		{
			BoundProduct = database.Products.First(p => p.ID == ID);
		}
	}
}

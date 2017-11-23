using System.Xml.Serialization;

namespace ComfortIsland.Database
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
	}
}

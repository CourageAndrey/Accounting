using System.Linq;

namespace ComfortIsland.BusinessLogic
{
	public class Position
	{
		#region Properties

		public long ID
		{ get; set; }

		public double Count
		{ get; set; }

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

namespace ComfortIsland.BusinessLogic
{
	public class Position
	{
		#region Properties

		public long ID
		{ get; set; }

		public decimal Count
		{ get; set; }

		public Product BoundProduct
		{ get; private set; }

		#endregion

		#region Constructors

		public Position()
		{ }

		public Position(long id, decimal count)
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
			BoundProduct = database.Products[ID];
		}
	}
}

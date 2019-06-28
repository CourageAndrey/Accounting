using ComfortIsland.Helpers;

namespace ComfortIsland.ViewModels
{
	public class Unit : IViewModel<BusinessLogic.Unit>
	{
		#region Properties

		private readonly long? id;

		public string Name
		{ get; set; }

		public string ShortName
		{ get; set; }

		#endregion

		#region Constructors

		public Unit()
		{ }

		public Unit(BusinessLogic.Unit instance)
		{
			id = instance.ID;
			Name = instance.Name;
			ShortName = instance.ShortName;
		}

		#endregion

		public BusinessLogic.Unit ConvertToBusinessLogic(BusinessLogic.Database database)
		{
			BusinessLogic.Unit instance;
			if (id.HasValue)
			{
				instance = database.Units[id.Value];
			}
			else
			{
				instance = new BusinessLogic.Unit { ID = IdHelper.GenerateNewId(database.Units.Values) };
				database.Units[instance.ID] = instance;
			}
			instance.Name = Name;
			instance.ShortName = ShortName;
			return instance;
		}
	}
}

using System.Linq;

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
				instance = database.Units.First(i => i.ID == id.Value);
			}
			else
			{
				database.Units.Add(instance = new BusinessLogic.Unit { ID = IdHelper.GenerateNewId(database.Units) });
			}
			instance.Name = Name;
			instance.ShortName = ShortName;
			return instance;
		}
	}
}

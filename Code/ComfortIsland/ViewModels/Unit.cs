using System.Text;

namespace ComfortIsland.ViewModels
{
	public class Unit : NotifyDataErrorInfo, IViewModel<BusinessLogic.Unit>
	{
		#region Properties

		public long? ID
		{ get; }

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				var errors = new StringBuilder();
				if (BusinessLogic.Unit.NameIsNotNullOrEmpty(value, errors))
				{
					ClearErrors();
				}
				else
				{
					AddError(errors.ToString());
				}
			}
		}

		public string ShortName
		{
			get { return shortName; }
			set
			{
				shortName = value;
				var errors = new StringBuilder();
				if (BusinessLogic.Unit.ShortNameIsNotNullOrEmpty(value, errors))
				{
					ClearErrors();
				}
				else
				{
					SetError(errors.ToString());
				}
			}
		}

		private string name, shortName;

		#endregion

		#region Constructors

		private Unit(long? id, string name, string shortName)
		{
			ID = id;
			Name = name;
			ShortName = shortName;
		}

		public Unit()
			: this(null, string.Empty, string.Empty)
		{ }

		public Unit(BusinessLogic.Unit instance)
			: this(instance.ID, instance.Name, instance.ShortName)
		{ }

		#endregion

		public BusinessLogic.Unit ConvertToBusinessLogic(BusinessLogic.Database database)
		{
			BusinessLogic.Unit instance;
			if (ID.HasValue)
			{
				instance = database.Units[ID.Value];
			}
			else
			{
				database.Units.Add(instance = new BusinessLogic.Unit());
			}
			instance.Name = Name;
			instance.ShortName = ShortName;
			return instance;
		}
	}
}

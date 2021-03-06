﻿using System.Text;

namespace Accounting.UI.WPF.ViewModels
{
	public class Unit : ViewModelBase<Accounting.Core.BusinessLogic.Unit>
	{
		#region Properties

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				var errors = new StringBuilder();
				if (Accounting.Core.BusinessLogic.Unit.NameIsNotNullOrEmpty(value, errors))
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
			get { return _shortName; }
			set
			{
				_shortName = value;
				var errors = new StringBuilder();
				if (Accounting.Core.BusinessLogic.Unit.ShortNameIsNotNullOrEmpty(value, errors))
				{
					ClearErrors();
				}
				else
				{
					SetError(errors.ToString());
				}
			}
		}

		private string _name, _shortName;

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

		public Unit(Accounting.Core.BusinessLogic.Unit instance)
			: this(instance.ID, instance.Name, instance.ShortName)
		{ }

		#endregion

		public override Accounting.Core.BusinessLogic.Unit CreateNewEntity()
		{
			return new Accounting.Core.BusinessLogic.Unit();
		}

		public override void UpdateProperties(Accounting.Core.BusinessLogic.Unit entity, Accounting.Core.BusinessLogic.IDatabase database)
		{
			entity.Name = Name;
			entity.ShortName = ShortName;
		}
	}
}

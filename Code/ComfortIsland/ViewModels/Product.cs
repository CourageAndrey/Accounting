using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComfortIsland.ViewModels
{
	public class Product : NotifyDataErrorInfo, IViewModel<BusinessLogic.Product>
	{
		#region Properties

		public long? ID
		{ get; }

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				var errors = new StringBuilder();
				if (BusinessLogic.Product.NameIsNotNullOrEmpty(value, errors))
				{
					ClearErrors();
				}
				else
				{
					AddError(errors.ToString());
				}
			}
		}

		public BusinessLogic.Unit Unit
		{
			get { return _unit; }
			set
			{
				_unit = value;
				var errors = new StringBuilder();
				if (BusinessLogic.Product.UnitIsNotNull(value, errors))
				{
					ClearErrors();
				}
				else
				{
					AddError(errors.ToString());
				}
			}
		}

		public List<BusinessLogic.Position> Children
		{ get; }

		private string _name;
		private BusinessLogic.Unit _unit;

		#endregion

		#region Constructors

		private Product(long? id, string name, BusinessLogic.Unit unit, List<BusinessLogic.Position> children)
		{
			ID = id;
			Name = name;
			Unit = unit;
			Children = children;
		}

		public Product()
			: this(null, string.Empty, null, new List<BusinessLogic.Position>())
		{ }

		public Product(BusinessLogic.Product instance)
			: this(
				instance.ID,
				instance.Name,
				instance.Unit,
				instance.Children.Select(child => new BusinessLogic.Position(child.Key.ID, child.Value)).ToList())
		{ }

		#endregion

		public BusinessLogic.Product ConvertToBusinessLogic(BusinessLogic.Database database)
		{
			BusinessLogic.Product instance;
			if (ID.HasValue)
			{
				instance = database.Products[ID.Value];
			}
			else
			{
				database.Products.Add(instance = new BusinessLogic.Product());
			}
			instance.Name = Name;
			instance.Unit = Unit;
			instance.Children = Children.ToDictionary(
				child => database.Products[child.ID],
				child => child.Count);
			return instance;
		}
	}
}

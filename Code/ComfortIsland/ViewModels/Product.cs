using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComfortIsland.ViewModels
{
	public class Product : NotifyDataErrorInfo, IViewModel<BusinessLogic.Product>
	{
		#region Properties

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
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
			get { return unit; }
			set
			{
				unit = value;
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
		{
			get { return children; }
			set
			{
				children = value;
				var errors = new StringBuilder();
#warning Dictionary
				/*var valueDictionary = value.ToDictionary(, );
				if (BusinessLogic.Product.ChildrenAreNotRecursive(id, valueDictionary, errors) &
					BusinessLogic.Product.ChildrenCountsArePositive(valueDictionary, errors) &
					BusinessLogic.Product.ChildrenDoNotDuplicate(valueDictionary, errors))
				{
					ClearErrors();
				}
				else
				{
					AddError(errors.ToString());
				}*/
			}
		}

		private readonly long? id;
		private string name;
		private BusinessLogic.Unit unit;
		private List<BusinessLogic.Position> children;

		#endregion

		#region Constructors

		private Product(long? id, string name, BusinessLogic.Unit unit, List<BusinessLogic.Position> children)
		{
			this.id = id;
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
			if (id.HasValue)
			{
				instance = database.Products[id.Value];
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

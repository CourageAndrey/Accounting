using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.UI.WPF.ViewModels
{
	public class Product : ViewModelBase<Accounting.Core.BusinessLogic.Product>
	{
		#region Properties

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				var errors = new StringBuilder();
				if (Accounting.Core.BusinessLogic.Product.NameIsNotNullOrEmpty(value, errors))
				{
					ClearErrors();
				}
				else
				{
					AddError(errors.ToString());
				}
			}
		}

		public Accounting.Core.BusinessLogic.Unit Unit
		{
			get { return _unit; }
			set
			{
				_unit = value;
				var errors = new StringBuilder();
				if (Accounting.Core.BusinessLogic.Product.UnitIsNotNull(value, errors))
				{
					ClearErrors();
				}
				else
				{
					AddError(errors.ToString());
				}
			}
		}

		public List<Accounting.Core.BusinessLogic.Position> Children
		{ get; }

		private string _name;
		private Accounting.Core.BusinessLogic.Unit _unit;

		#endregion

		#region Constructors

		private Product(long? id, string name, Accounting.Core.BusinessLogic.Unit unit, List<Accounting.Core.BusinessLogic.Position> children)
		{
			ID = id;
			Name = name;
			Unit = unit;
			Children = children;
		}

		public Product()
			: this(null, string.Empty, null, new List<Accounting.Core.BusinessLogic.Position>())
		{ }

		public Product(Accounting.Core.BusinessLogic.Product instance)
			: this(
				instance.ID,
				instance.Name,
				instance.Unit,
				instance.Children.Select(child => new Accounting.Core.BusinessLogic.Position(child.Key.ID, child.Value)).ToList())
		{ }

		#endregion

		public override Accounting.Core.BusinessLogic.Product ConvertToBusinessLogic(Accounting.Core.BusinessLogic.Database database)
		{
			Accounting.Core.BusinessLogic.Product instance;
			if (ID.HasValue)
			{
				instance = database.Products[ID.Value];
			}
			else
			{
				database.Products.Add(instance = new Accounting.Core.BusinessLogic.Product());
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

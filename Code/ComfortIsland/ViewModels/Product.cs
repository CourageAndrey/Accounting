using System.Collections.Generic;
using System.Linq;

using ComfortIsland.Helpers;

namespace ComfortIsland.ViewModels
{
	public class Product : IViewModel<BusinessLogic.Product>
	{
		#region Properties

		private readonly long? id;

		public string Name
		{ get; set; }

		public BusinessLogic.Unit Unit
		{ get; set; }

		public List<BusinessLogic.Position> Children
		{ get; }

		#endregion

		public Product()
		{
			Children = new List<BusinessLogic.Position>();
		}

		public Product(BusinessLogic.Product instance)
		{
			id = instance.ID;
			Name = instance.Name;
			Unit = instance.Unit;
			Children = instance.Children.Select(child => new BusinessLogic.Position(child.Key.ID, child.Value)).ToList();
		}

		public BusinessLogic.Product ConvertToBusinessLogic(BusinessLogic.Database database)
		{
			BusinessLogic.Product instance;
			if (id.HasValue)
			{
				instance = database.Products.First(i => i.ID == id.Value);
			}
			else
			{
				database.Products.Add(instance = new BusinessLogic.Product { ID = IdHelper.GenerateNewId(database.Products) });
			}
			instance.Name = Name;
			instance.Unit = Unit;
			instance.Children = Children.ToDictionary(
				child => database.Products.First(p => p.ID == child.ID),
				child => child.Count);
			return instance;
		}
	}
}

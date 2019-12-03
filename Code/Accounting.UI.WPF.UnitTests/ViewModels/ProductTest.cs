using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Accounting.UI.WPF.ViewModels;

namespace Accounting.UI.WPF.UnitTests.ViewModels
{
	public class ProductTest
	{
		[Test]
		public void Constructor()
		{
			// arrange
			var unit = new Accounting.Core.BusinessLogic.Unit
			{
				ID = 1,
				Name = "unit",
				ShortName = "u",
			};
			var child = new Accounting.Core.BusinessLogic.Product
			{
				ID = 1,
				Name = "Child",
				Unit = unit,
			};
			var businessObject = new Accounting.Core.BusinessLogic.Product
			{
				Name = "Product",
				Unit = unit,
				Children = new Dictionary<Accounting.Core.BusinessLogic.Product, decimal> { { child, 75 } },
			};

			// act
			var viewModel = new Product(businessObject);

			// assert
			Assert.AreEqual("Product", viewModel.Name);
			Assert.AreSame(unit, viewModel.Unit);
			var position = viewModel.Children.Single();
			Assert.AreEqual(child.ID, position.ID);
			Assert.AreEqual(75, position.Count);
		}

		[Test]
		public void CreateNew()
		{
			// arrange
			var unit = new Accounting.Core.BusinessLogic.Unit
			{
				ID = 1,
				Name = "unit",
				ShortName = "u",
			};
			var child = new Accounting.Core.BusinessLogic.Product
			{
				ID = 1,
				Name = "Child product",
				Unit = unit,
				Children = new Dictionary<Accounting.Core.BusinessLogic.Product, decimal>(),
			};
			var database = new Accounting.Core.BusinessLogic.Database(
				new[] { unit },
				new[] { child },
				new Dictionary<long, decimal>(),
				new Accounting.Core.BusinessLogic.Document[0]);

			// act
			var viewModel = new Product
			{
				Name = "Product",
				Unit = unit,
			};
			viewModel.Children.Add(new Accounting.Core.BusinessLogic.Position(child.ID, 5));
			var businessObject = viewModel.ConvertToBusinessLogic(database);

			// assert
			Assert.AreSame(businessObject, database.Products.Last());
			Assert.AreEqual(2, businessObject.ID);
			Assert.AreEqual("Product", businessObject.Name);
			Assert.AreSame(unit, businessObject.Unit);
			Assert.AreEqual(1, businessObject.Children.Count);
			Assert.AreEqual(5D, businessObject.Children[child]);
		}

		[Test]
		public void EditExisting()
		{
			// arrange
			var unitOld = new Accounting.Core.BusinessLogic.Unit
			{
				ID = 1,
				Name = "old unit",
				ShortName = "o/u",
			};
			var unitNew = new Accounting.Core.BusinessLogic.Unit
			{
				ID = 2,
				Name = "new unit",
				ShortName = "n/u",
			};
			var childOld = new Accounting.Core.BusinessLogic.Product
			{
				ID = 3,
				Name = "old child",
				Unit = unitOld,
				Children = new Dictionary<Accounting.Core.BusinessLogic.Product, decimal>(),
			};
			var childNew = new Accounting.Core.BusinessLogic.Product
			{
				ID = 4,
				Name = "new child",
				Unit = unitNew,
				Children = new Dictionary<Accounting.Core.BusinessLogic.Product, decimal>(),
			};
			var initialBusinessObject = new Accounting.Core.BusinessLogic.Product
			{
				ID = 5,
				Name = "Product",
				Unit = unitOld,
				Children = new Dictionary<Accounting.Core.BusinessLogic.Product, decimal> { { childOld, 6 } },
			};
			var database = new Accounting.Core.BusinessLogic.Database(
				new[] { unitOld, unitNew },
				new[] { childOld, childNew, initialBusinessObject },
				new Dictionary<long, decimal>(),
				new Accounting.Core.BusinessLogic.Document[0]);

			// act
			var viewModel = new Product(initialBusinessObject);
			viewModel.Name = "Changed";
			viewModel.Unit = unitNew;
			viewModel.Children.Clear();
			viewModel.Children.Add(new Accounting.Core.BusinessLogic.Position(childNew.ID, 7));
			var resultBusinessObject = viewModel.ConvertToBusinessLogic(database);

			// assert
			Assert.AreSame(initialBusinessObject, resultBusinessObject);
			Assert.AreSame(database.Products[5], resultBusinessObject);
			Assert.AreEqual("Changed", resultBusinessObject.Name);
			Assert.AreSame(unitNew, resultBusinessObject.Unit);
			Assert.AreSame(childNew, resultBusinessObject.Children.Keys.Single());
			Assert.AreEqual(7d, resultBusinessObject.Children.Values.Single());
		}
	}
}

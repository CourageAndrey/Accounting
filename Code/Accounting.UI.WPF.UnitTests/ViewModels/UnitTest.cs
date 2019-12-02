using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using ComfortIsland.ViewModels;

namespace ComfortIsland.UnitTests.ViewModels
{
	public class UnitTest
	{
		[Test]
		public void Constructor()
		{
			// arrange
			var businessObject = new ComfortIsland.BusinessLogic.Unit
			{
				Name = "unit",
				ShortName = "u",
			};

			// act
			var viewModel = new Unit(businessObject);

			// assert
			Assert.AreEqual("unit", viewModel.Name);
			Assert.AreEqual("u", viewModel.ShortName);
		}

		[Test]
		public void CreateNew()
		{
			// arrange
			var database = new ComfortIsland.BusinessLogic.Database(
				new ComfortIsland.BusinessLogic.Unit[0],
				new ComfortIsland.BusinessLogic.Product[0],
				new Dictionary<long, decimal>(),
				new ComfortIsland.BusinessLogic.Document[0]);

			// act
			var viewModel = new Unit
			{
				Name = "unit",
				ShortName = "u",
			};
			var businessObject = viewModel.ConvertToBusinessLogic(database);

			// assert
			Assert.AreSame(businessObject, database.Units.Single());
			Assert.AreEqual(1, businessObject.ID);
			Assert.AreEqual("unit", businessObject.Name);
			Assert.AreEqual("u", businessObject.ShortName);
		}

		[Test]
		public void EditExisting()
		{
			// arrange
			var initialBusinessObject = new ComfortIsland.BusinessLogic.Unit
			{
				ID = 1,
				Name = "unit",
				ShortName = "u",
			};
			var database = new ComfortIsland.BusinessLogic.Database(
				new[] { initialBusinessObject },
				new ComfortIsland.BusinessLogic.Product[0],
				new Dictionary<long, decimal>(),
				new ComfortIsland.BusinessLogic.Document[0]);

			// act
			var viewModel = new Unit(initialBusinessObject);
			viewModel.Name = "changed";
			viewModel.ShortName = "c";
			var resultBusinessObject = viewModel.ConvertToBusinessLogic(database);

			// assert
			Assert.AreSame(initialBusinessObject, resultBusinessObject);
			Assert.AreSame(resultBusinessObject, database.Units.Single());
			Assert.AreEqual("changed", resultBusinessObject.Name);
			Assert.AreEqual("c", resultBusinessObject.ShortName);
		}
	}
}

using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Accounting.Core.BusinessLogic;

namespace Accounting.Core.UnitTests.BusinessLogic
{
	public class ProductBalanceTest
	{
		[Test]
		public void ProductsWithoutChildren()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);

			// act
			var reportChild1 = new ProductBalance(productChild1, database.Balance);
			var reportChild2 = new ProductBalance(productChild2, database.Balance);

			// assert
			Assert.AreSame(productChild1, reportChild1.Product);
			Assert.AreSame(productChild2, reportChild2.Product);
			Assert.AreEqual(20, reportChild1.Count);
			Assert.AreEqual(10, reportChild2.Count);
			Assert.AreEqual(0, reportChild1.CanBeProduced);
			Assert.AreEqual(0, reportChild2.CanBeProduced);
			Assert.AreEqual(0, reportChild1.Children.Count);
			Assert.AreEqual(0, reportChild2.Children.Count);
		}

		[Test]
		public void ProductsWithChildren()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);

			// act
			var reportParent = new ProductBalance(productParent, database.Balance);

			// assert
			Assert.AreSame(productParent, reportParent.Product);
			Assert.AreEqual(30, reportParent.Count);
			Assert.AreEqual(5, reportParent.CanBeProduced);
			Assert.AreEqual(2, reportParent.Children.Count);
			var reportChild1 = reportParent.Children.Single(child => child.Child == productChild1);
			var reportChild2 = reportParent.Children.Single(child => child.Child == productChild2);
			Assert.AreSame(productParent, reportChild1.Parent);
			Assert.AreSame(productParent, reportChild2.Parent);
			Assert.AreEqual(20, reportChild1.ChildCount);
			Assert.AreEqual(10, reportChild2.ChildCount);
			Assert.AreEqual(20, reportChild1.ParentsCount);
			Assert.AreEqual(5, reportChild2.ParentsCount);
		}

		private IDatabase createTestBase(out Product productChild1, out Product productChild2, out Product productParent)
		{
			var unit = new Unit
			{
				ID = 1,
				Name = "Full Name",
				ShortName = "short",
			};
			productChild1 = new Product
			{
				ID = 1,
				Name = "1",
				Unit = unit,
			};
			productChild2 = new Product
			{
				ID = 2,
				Name = "2",
				Unit = unit,
			};
			productParent = new Product
			{
				ID = 3,
				Children =
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
				},
				Name = "P",
				Unit = unit,
			};
			var database = new Database
			(
				new[] { unit },
				new[]
				{
					productChild1,
					productChild2,
					productParent,
				},
				new Dictionary<long, decimal>
				{
					{ productChild1.ID, 20 },
					{ productChild2.ID, 10 },
					{ productParent.ID, 30 },
				},
				new Document[0]
			);
			return database;
		}
	}
}

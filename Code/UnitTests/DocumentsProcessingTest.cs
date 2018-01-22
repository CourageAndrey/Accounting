using System;
using System.Linq;

using ComfortIsland.Database;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class DocumentsProcessingTest
	{
		[TestMethod]
		public void ProcessAllTypesAndRollback()
		{
			// create database
			var unit = new Unit
			{
				ID = 1,
				Name = string.Empty,
				ShortName = string.Empty,
			};
			var productChild1 = new Product
			{
				ID = 1,
				Code = string.Empty,
				Name = string.Empty,
				Unit = unit,
			};
			productChild1.BeforeEdit();
			var productChild2 = new Product
			{
				ID = 2,
				Code = string.Empty,
				Name = string.Empty,
				Unit = unit,
			};
			productChild2.BeforeEdit();
			var productParent = new Product
			{
				ID = 3,
				Children =
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
				},
				Code = string.Empty,
				Name = string.Empty,
				Unit = unit,
			};
			productParent.BeforeEdit();
			var database = new Database
			{
				Units = { unit },
				Products =
				{
					productChild1,
					productChild2,
					productParent,
				},
				Balance =
				{
					new Balance(productChild1, 10),
					new Balance(productChild2, 20),
					new Balance(productParent, 1),
				},
			};
			Database.SetTestBase(database);

			// apply documents
			var income = new Document
			{
				ID = 1,
				Date = new DateTime(),
				Number = string.Empty,
				Type = DocumentType.Income,
				Positions =
				{
					{ productChild1, 10 },
					{ productChild2, 20 },
				}
			};
			var outcome = new Document
			{
				ID = 2,
				Date = new DateTime(),
				Number = string.Empty,
				Type = DocumentType.Outcome,
				Positions =
				{
					{ productChild1, 5 },
					{ productChild2, 10 },
					{ productParent, 1 },
				}
			};
			var produce = new Document
			{
				ID = 1,
				Date = new DateTime(),
				Number = string.Empty,
				Type = DocumentType.Produce,
				Positions =
				{
					{ productParent, 15 },
				}
			};
			income.BeforeEdit();
			outcome.BeforeEdit();
			produce.BeforeEdit();

			income.Apply(database.Balance);
			Assert.AreEqual(20, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(40, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			produce.Apply(database.Balance);
			Assert.AreEqual(5, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(10, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(16, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			outcome.Apply(database.Balance);
			Assert.AreEqual(0, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(0, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(15, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			outcome.Rollback(database.Balance);
			Assert.AreEqual(5, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(10, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(16, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			produce.Rollback(database.Balance);
			Assert.AreEqual(20, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(40, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			income.Rollback(database.Balance);
			Assert.AreEqual(10, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(20, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ProductId == productParent.ID).Count);
		}
	}
}

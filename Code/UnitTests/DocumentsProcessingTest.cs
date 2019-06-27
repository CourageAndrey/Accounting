using System;
using System.Linq;

using ComfortIsland.BusinessLogic;

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
				Name = string.Empty,
				Unit = unit,
			};
			var productChild2 = new Product
			{
				ID = 2,
				Name = string.Empty,
				Unit = unit,
			};
			var productParent = new Product
			{
				ID = 3,
				Children =
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
				},
				Name = string.Empty,
				Unit = unit,
			};
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
					new Position(productChild1.ID, 10),
					new Position(productChild2.ID, 20),
					new Position(productParent.ID, 1),
				},
			};
			productChild1.BeforeEdit(database);
			productChild2.BeforeEdit(database);
			productParent.BeforeEdit(database);

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
			income.BeforeEdit(database);
			outcome.BeforeEdit(database);
			produce.BeforeEdit(database);

			income.Apply(database, database.Balance);
			Assert.AreEqual(20, database.Balance.First(b => b.ID == productChild1.ID).Count);
			Assert.AreEqual(40, database.Balance.First(b => b.ID == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ID == productParent.ID).Count);

			produce.Apply(database, database.Balance);
			Assert.AreEqual(5, database.Balance.First(b => b.ID == productChild1.ID).Count);
			Assert.AreEqual(10, database.Balance.First(b => b.ID == productChild2.ID).Count);
			Assert.AreEqual(16, database.Balance.First(b => b.ID == productParent.ID).Count);

			outcome.Apply(database, database.Balance);
			Assert.AreEqual(0, database.Balance.First(b => b.ID == productChild1.ID).Count);
			Assert.AreEqual(0, database.Balance.First(b => b.ID == productChild2.ID).Count);
			Assert.AreEqual(15, database.Balance.First(b => b.ID == productParent.ID).Count);

			outcome.Rollback(database, database.Balance);
			Assert.AreEqual(5, database.Balance.First(b => b.ID == productChild1.ID).Count);
			Assert.AreEqual(10, database.Balance.First(b => b.ID == productChild2.ID).Count);
			Assert.AreEqual(16, database.Balance.First(b => b.ID == productParent.ID).Count);

			produce.Rollback(database, database.Balance);
			Assert.AreEqual(20, database.Balance.First(b => b.ID == productChild1.ID).Count);
			Assert.AreEqual(40, database.Balance.First(b => b.ID == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ID == productParent.ID).Count);

			income.Rollback(database, database.Balance);
			Assert.AreEqual(10, database.Balance.First(b => b.ID == productChild1.ID).Count);
			Assert.AreEqual(20, database.Balance.First(b => b.ID == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ID == productParent.ID).Count);
		}
	}
}

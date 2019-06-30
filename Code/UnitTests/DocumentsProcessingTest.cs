using System;
using System.Collections.Generic;

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
				Name = "Full Name",
				ShortName = "short",
			};
			var productChild1 = new Product
			{
				ID = 1,
				Name = "1",
				Unit = unit,
			};
			var productChild2 = new Product
			{
				ID = 2,
				Name = "2",
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
				new Dictionary<long, double>
				{
					{ productChild1.ID, 10 },
					{ productChild2.ID, 20 },
					{ productParent.ID, 1 },
				},
				new Document[0]
			);

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

			income.Apply(database);
			Assert.AreEqual(20, database.Balance[productChild1.ID]);
			Assert.AreEqual(40, database.Balance[productChild2.ID]);
			Assert.AreEqual(1, database.Balance[productParent.ID]);

			produce.Apply(database);
			Assert.AreEqual(5, database.Balance[productChild1.ID]);
			Assert.AreEqual(10, database.Balance[productChild2.ID]);
			Assert.AreEqual(16, database.Balance[productParent.ID]);

			outcome.Apply(database);
			Assert.AreEqual(0, database.Balance[productChild1.ID]);
			Assert.AreEqual(0, database.Balance[productChild2.ID]);
			Assert.AreEqual(15, database.Balance[productParent.ID]);

			outcome.Rollback(database);
			Assert.AreEqual(5, database.Balance[productChild1.ID]);
			Assert.AreEqual(10, database.Balance[productChild2.ID]);
			Assert.AreEqual(16, database.Balance[productParent.ID]);

			produce.Rollback(database);
			Assert.AreEqual(20, database.Balance[productChild1.ID]);
			Assert.AreEqual(40, database.Balance[productChild2.ID]);
			Assert.AreEqual(1, database.Balance[productParent.ID]);

			income.Rollback(database);
			Assert.AreEqual(10, database.Balance[productChild1.ID]);
			Assert.AreEqual(20, database.Balance[productChild2.ID]);
			Assert.AreEqual(1, database.Balance[productParent.ID]);
		}
	}
}

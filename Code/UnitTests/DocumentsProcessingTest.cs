﻿using System;
using System.Linq;
using System.Reflection;
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
			typeof(Database).GetFields(BindingFlags.Static | BindingFlags.NonPublic).OfType<FieldInfo>().First(f => f.FieldType == typeof(Database)).SetValue(null, database);

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

			var incomeType = DocumentTypeImplementation.AllTypes[income.Type];
			var outcomeType = DocumentTypeImplementation.AllTypes[outcome.Type];
			var produceType = DocumentTypeImplementation.AllTypes[produce.Type];
			Assert.AreEqual(DocumentType.Income, incomeType.Type);
			Assert.AreEqual(DocumentType.Outcome, outcomeType.Type);
			Assert.AreEqual(DocumentType.Produce, produceType.Type);

			incomeType.Process(income, database.Balance);
			Assert.AreEqual(20, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(40, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			produceType.Process(produce, database.Balance);
			Assert.AreEqual(5, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(10, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(16, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			outcomeType.Process(outcome, database.Balance);
			Assert.AreEqual(0, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(0, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(15, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			outcomeType.ProcessBack(outcome, database.Balance);
			Assert.AreEqual(5, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(10, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(16, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			produceType.ProcessBack(produce, database.Balance);
			Assert.AreEqual(20, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(40, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ProductId == productParent.ID).Count);

			incomeType.ProcessBack(income, database.Balance);
			Assert.AreEqual(10, database.Balance.First(b => b.ProductId == productChild1.ID).Count);
			Assert.AreEqual(20, database.Balance.First(b => b.ProductId == productChild2.ID).Count);
			Assert.AreEqual(1, database.Balance.First(b => b.ProductId == productParent.ID).Count);
		}
	}
}
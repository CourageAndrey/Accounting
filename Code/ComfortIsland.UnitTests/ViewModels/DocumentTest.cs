﻿using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using ComfortIsland.ViewModels;

namespace ComfortIsland.UnitTests.ViewModels
{
	public class DocumentTest
	{
		[Test]
		public void Constructor()
		{
			// arrange
			var unit = new BusinessLogic.Unit
			{
				ID = 1,
				Name = "unit",
				ShortName = "u",
			};
			var product = new BusinessLogic.Product
			{
				ID = 1,
				Name = "Product",
				Unit = unit,
			};
			var documentDate = DateTime.Now;
			var businessObject = new BusinessLogic.Document(BusinessLogic.DocumentType.Income)
			{
				Date = documentDate,
				Number = "1-bis",
				Positions = new Dictionary<BusinessLogic.Product, decimal> { { product, 7 } },
			};

			// act
			var viewModel = new Document(businessObject);

			// assert
			Assert.AreEqual(BusinessLogic.DocumentType.Income, viewModel.Type);
			Assert.AreEqual("1-bis", viewModel.Number);
			Assert.AreEqual(documentDate, viewModel.Date);
			var position = viewModel.Positions.Single();
			Assert.AreEqual(product.ID, position.ID);
			Assert.AreEqual(7, position.Count);
		}

		[Test]
		public void CreateNew()
		{
			// arrange
			var unit = new BusinessLogic.Unit
			{
				ID = 1,
				Name = "unit",
				ShortName = "u",
			};
			var product = new BusinessLogic.Product
			{
				ID = 1,
				Name = "Product",
				Unit = unit,
				Children = new Dictionary<BusinessLogic.Product, decimal>(),
			};
			var documentDate = DateTime.Now;
			var database = new BusinessLogic.Database(
				new[] { unit },
				new[] { product },
				new Dictionary<long, decimal>(),
				new BusinessLogic.Document[0]);

			// act
			var viewModel = new Document(BusinessLogic.DocumentType.Income)
			{
				Date = documentDate,
				Number = "1-bis",
			};
			viewModel.Positions.Add(new BusinessLogic.Position(product.ID, 5));
			var businessObject = viewModel.ConvertToBusinessLogic(database);

			// assert
			Assert.AreSame(businessObject, database.Documents.Single());
			Assert.AreEqual(1, businessObject.ID);
			Assert.AreEqual(documentDate, businessObject.Date);
			Assert.AreEqual("1-bis", businessObject.Number);
			Assert.AreSame(BusinessLogic.DocumentType.Income, businessObject.Type);
			Assert.AreEqual(BusinessLogic.DocumentState.Active, businessObject.State);
			Assert.AreEqual(1, businessObject.Positions.Count);
			Assert.AreEqual(5D, businessObject.Positions[product]);
		}

		[Test]
		public void EditExisting()
		{
			// arrange
			var unit = new BusinessLogic.Unit
			{
				ID = 1,
				Name = "unit",
				ShortName = "u",
			};
			var oldProduct = new BusinessLogic.Product
			{
				ID = 2,
				Name = "Old",
				Unit = unit,
				Children = new Dictionary<BusinessLogic.Product, decimal>(),
			};
			var newProduct = new BusinessLogic.Product
			{
				ID = 3,
				Name = "New",
				Unit = unit,
				Children = new Dictionary<BusinessLogic.Product, decimal>(),
			};
			var initialDate = DateTime.Now;
			var initialBusinessObject = new BusinessLogic.Document(BusinessLogic.DocumentType.Income)
			{
				ID = 4,
				Date = initialDate,
				Number = "1-bis",
				Positions = new Dictionary<BusinessLogic.Product, decimal> { { oldProduct, 5 } },
			};
			var database = new BusinessLogic.Database(
				new[] { unit },
				new[] { oldProduct, newProduct },
				new Dictionary<long, decimal> { { oldProduct.ID, 6 } },
				new[] { initialBusinessObject });

			// act
			var newDate = DateTime.Now;
			var viewModel = new Document(initialBusinessObject);
			viewModel.Date = newDate;
			viewModel.Number = "2-xyz";
			viewModel.Positions.Clear();
			viewModel.Positions.Add(new BusinessLogic.Position(newProduct.ID, 6));
			var resultBusinessObject = viewModel.ConvertToBusinessLogic(database);

			// assert
			Assert.AreNotSame(initialBusinessObject, resultBusinessObject);

			Assert.AreSame(database.Documents[4], initialBusinessObject);
			Assert.AreEqual(4, initialBusinessObject.ID);
			Assert.AreEqual(initialDate, initialBusinessObject.Date);
			Assert.AreEqual("1-bis", initialBusinessObject.Number);
			Assert.AreSame(BusinessLogic.DocumentType.Income, initialBusinessObject.Type);
			Assert.AreEqual(BusinessLogic.DocumentState.Edited, initialBusinessObject.State);
			Assert.AreSame(oldProduct, initialBusinessObject.Positions.Keys.Single());
			Assert.AreEqual(5d, initialBusinessObject.Positions.Values.Single());

			Assert.AreSame(database.Documents[5], resultBusinessObject);
			Assert.AreEqual(5, resultBusinessObject.ID);
			Assert.AreEqual(newDate, resultBusinessObject.Date);
			Assert.AreEqual("2-xyz", resultBusinessObject.Number);
			Assert.AreSame(BusinessLogic.DocumentType.Income, resultBusinessObject.Type);
			Assert.AreEqual(BusinessLogic.DocumentState.Active, resultBusinessObject.State);
			Assert.AreSame(newProduct, resultBusinessObject.Positions.Keys.Single());
			Assert.AreEqual(6d, resultBusinessObject.Positions.Values.Single());

			Assert.AreEqual(1d, database.Balance[oldProduct.ID]);
			Assert.AreEqual(6d, database.Balance[newProduct.ID]);
		}
	}
}

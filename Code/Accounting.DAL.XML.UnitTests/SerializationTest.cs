using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

using Accounting.Core.DataAccessLayer;

namespace Accounting.DAL.XML.UnitTests
{
	public class SerializationTest
	{
		[Test]
		public void HappyPass()
		{
			// arrange
			var date = DateTime.Now;
			var unit = new Unit
			{
				ID = 1,
				Name = "Full Name",
				ShortName = "short",
			};
			var childProduct1 = new Product
			{
				ID = 1,
				Name = "CHILD 1",
				Unit = unit,
			};
			var childProduct2 = new Product
			{
				ID = 2,
				Name = "CHILD 2",
				Unit = unit,
			};
			var parentProduct = new Product
			{
				ID = 3,
				Name = "PARENT",
				Unit = unit,
				Children =
				{
					{ childProduct1, 1 },
					{ childProduct2, 2 },
				},
			};
			Document originalIncome, originalOutcome, originalProduce, originalMovement;
			var originalDatabase = new Database
			(
				new[] { unit },
				new[] { childProduct1, childProduct2, parentProduct },
				new Dictionary<long, decimal>
				{
					{ parentProduct.ID, 1 },
					{ childProduct1.ID, 2 },
					{ childProduct2.ID, 3 },
				},
				new[]
				{
					originalIncome = new Document(DocumentType.Income)
					{
						ID = 1,
						Number = date.ToShortDateString(),
						Date = date,
						Positions =
						{
							{ childProduct1, 100 },
							{ childProduct2, 300 },
						},
					},
					originalOutcome = new Document(DocumentType.Outcome)
					{
						ID = 2,
						Number = date.AddDays(1).ToShortDateString(),
						Date = date.AddDays(1),
						Positions = { { childProduct2, 100 } },
					},
					originalProduce = new Document(DocumentType.Produce)
					{
						ID = 3,
						Number = date.AddDays(2).ToShortDateString(),
						Date = date.AddDays(2),
						Positions = { { parentProduct, 100 } },
					},
					originalMovement = new Document(DocumentType.ToWarehouse)
					{
						ID = 4,
						Number = date.AddDays(3).ToShortDateString(),
						Date = date.AddDays(3),
						Positions = { { parentProduct, 100 } },
					},
				});
			Func<IDictionary<Product, decimal>, IDictionary<long, decimal>> simplify = original => original.ToDictionary(
				position => position.Key.ID,
				position => position.Value);

			// act
			string fileName = Path.ChangeExtension(Path.GetTempFileName(), "xml");
			var xmlDriver = new ComfortIsland.DataAccessLayer.Xml.DatabaseDriver(fileName);
			Database deserialized = null;
			try
			{
				xmlDriver.Save(originalDatabase);
				deserialized = xmlDriver.TryLoad();
			}
			finally
			{
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
			}

			// assert
			var deserializedUnit = deserialized.Units.Single();
			Assert.IsTrue(	deserializedUnit.ID == unit.ID &&
							deserializedUnit.Name == unit.Name &&
							deserializedUnit.ShortName == unit.ShortName);

			Assert.AreEqual(originalDatabase.Products.Count, deserialized.Products.Count);

			var deserializedChildProduct1 = deserialized.Products[childProduct1.ID];
			Assert.IsTrue(	deserializedChildProduct1.Name == childProduct1.Name &&
							deserializedChildProduct1.Unit == deserializedUnit &&
							deserializedChildProduct1.Children.Count == 0);

			var deserializedChildProduct2 = deserialized.Products[childProduct2.ID];
			Assert.IsTrue(	deserializedChildProduct2.Name == childProduct2.Name &&
							deserializedChildProduct2.Unit == deserializedUnit &&
							deserializedChildProduct2.Children.Count == 0);

			var deserializedParentProduct = deserialized.Products[parentProduct.ID];
			Assert.IsTrue(	deserializedParentProduct.Name == parentProduct.Name &&
							deserializedParentProduct.Unit == deserializedUnit &&
							deserializedParentProduct.Children.Count == 2 &&
							deserializedParentProduct.Children[deserializedChildProduct1] == parentProduct.Children[childProduct1] &&
							deserializedParentProduct.Children[deserializedChildProduct2] == parentProduct.Children[childProduct2]);

			Assert.AreEqual(originalDatabase.Documents.Count, deserialized.Documents.Count);

			var deserializedIncome = deserialized.Documents[originalIncome.ID];
			Assert.IsTrue(	deserializedIncome.Number == originalIncome.Number &&
							deserializedIncome.Date == originalIncome.Date &&
							simplify(deserializedIncome.Positions).IsEqualTo(simplify(originalIncome.Positions)));

			var deserializedOutcome = deserialized.Documents[originalOutcome.ID];
			Assert.IsTrue(	deserializedOutcome.Number == originalOutcome.Number &&
							deserializedOutcome.Date == originalOutcome.Date &&
							simplify(deserializedOutcome.Positions).IsEqualTo(simplify(originalOutcome.Positions)));

			var deserializedProduce = deserialized.Documents[originalProduce.ID];
			Assert.IsTrue(	deserializedProduce.Number == originalProduce.Number &&
							deserializedProduce.Date == originalProduce.Date &&
							simplify(deserializedProduce.Positions).IsEqualTo(simplify(originalProduce.Positions)));

			var deserializedMovement = deserialized.Documents[originalMovement.ID];
			Assert.IsTrue(	deserializedMovement.Number == originalMovement.Number &&
							deserializedMovement.Date == originalMovement.Date &&
							simplify(deserializedMovement.Positions).IsEqualTo(simplify(originalMovement.Positions)));

			Assert.IsTrue(deserialized.Balance.SequenceEqual(originalDatabase.Balance));
		}
	}
}

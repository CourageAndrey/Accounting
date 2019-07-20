using System;
using System.Collections.Generic;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

namespace ComfortIsland.UnitTests
{
	public class WorkflowTest
	{
		[Test]
		public void ApplyIncomeIncreasesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.Income)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
					{ productParent, 3 },
				}
			};

			// act
			var delta = document.Apply(database);

			// assert
			Assert.AreEqual(11, database.Balance[productChild1.ID]);
			Assert.AreEqual(22, database.Balance[productChild2.ID]);
			Assert.AreEqual(33, database.Balance[productParent.ID]);
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, 1 },
				{ productChild2.ID, 2 },
				{ productParent.ID, 3 },
			}));
		}

		[Test]
		public void ApplyOutcomeDecreasesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.Outcome)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
					{ productParent, 3 },
				}
			};

			// act
			var delta = document.Apply(database);

			// assert
			Assert.AreEqual(9, database.Balance[productChild1.ID]);
			Assert.AreEqual(18, database.Balance[productChild2.ID]);
			Assert.AreEqual(27, database.Balance[productParent.ID]);
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, -1 },
				{ productChild2.ID, -2 },
				{ productParent.ID, -3 },
			}));
		}

		[Test]
		public void ApplyWarehouseDecreasesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.ToWarehouse)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
					{ productParent, 3 },
				}
			};

			// act
			var delta = document.Apply(database);

			// assert
			Assert.AreEqual(9, database.Balance[productChild1.ID]);
			Assert.AreEqual(18, database.Balance[productChild2.ID]);
			Assert.AreEqual(27, database.Balance[productParent.ID]);
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, -1 },
				{ productChild2.ID, -2 },
				{ productParent.ID, -3 },
			}));
		}

		[Test]
		public void ApplyProduceChangesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.Produce)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productParent, 5 },
				}
			};

			// act
			var delta = document.Apply(database);

			// assert
			Assert.AreEqual(5, database.Balance[productChild1.ID]);
			Assert.AreEqual(10, database.Balance[productChild2.ID]);
			Assert.AreEqual(35, database.Balance[productParent.ID]);
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, -5 },
				{ productChild2.ID, -10 },
				{ productParent.ID, 5 },
			}));
		}

		[Test]
		public void RollbackIncomeDecreasesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.Income)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
					{ productParent, 3 },
				}
			};

			// act
			var delta = document.Rollback(database);

			// assert
			Assert.AreEqual(9, database.Balance[productChild1.ID]);
			Assert.AreEqual(18, database.Balance[productChild2.ID]);
			Assert.AreEqual(27, database.Balance[productParent.ID]);
			
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, 1 },
				{ productChild2.ID, 2 },
				{ productParent.ID, 3 },
			}));
		}

		[Test]
		public void RollbackOutcomeIncreasesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.Outcome)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
					{ productParent, 3 },
				}
			};

			// act
			var delta = document.Rollback(database);

			// assert
			Assert.AreEqual(11, database.Balance[productChild1.ID]);
			Assert.AreEqual(22, database.Balance[productChild2.ID]);
			Assert.AreEqual(33, database.Balance[productParent.ID]);
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, -1 },
				{ productChild2.ID, -2 },
				{ productParent.ID, -3 },
			}));
		}

		[Test]
		public void RollbackWarehouseIncreasesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.ToWarehouse)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productChild1, 1 },
					{ productChild2, 2 },
					{ productParent, 3 },
				}
			};

			// act
			var delta = document.Rollback(database);

			// assert
			Assert.AreEqual(11, database.Balance[productChild1.ID]);
			Assert.AreEqual(22, database.Balance[productChild2.ID]);
			Assert.AreEqual(33, database.Balance[productParent.ID]);
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, -1 },
				{ productChild2.ID, -2 },
				{ productParent.ID, -3 },
			}));
		}

		[Test]
		public void RollbackProduceChangesBalance()
		{
			// arrange
			Product productChild1, productChild2, productParent;
			var database = createTestBase(out productChild1, out productChild2, out productParent);
			var document = new Document(DocumentType.Produce)
			{
				ID = 0,
				Date = new DateTime(),
				Number = string.Empty,
				Positions = new Dictionary<Product, decimal>
				{
					{ productParent, 5 },
				}
			};

			// act
			var delta = document.Rollback(database);

			// assert
			Assert.AreEqual(15, database.Balance[productChild1.ID]);
			Assert.AreEqual(30, database.Balance[productChild2.ID]);
			Assert.AreEqual(25, database.Balance[productParent.ID]);
			Assert.IsTrue(delta.IsEqualTo(new Dictionary<long, decimal>
			{
				{ productChild1.ID, -5 },
				{ productChild2.ID, -10 },
				{ productParent.ID, 5 },
			}));
		}

		private static Database createTestBase(out Product productChild1, out Product productChild2, out Product productParent)
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
					{ productChild1.ID, 10 },
					{ productChild2.ID, 20 },
					{ productParent.ID, 30 },
				},
				new Document[0]
			);
			return database;
		}
	}
}

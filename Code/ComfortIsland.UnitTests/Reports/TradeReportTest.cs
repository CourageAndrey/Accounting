using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;
using ComfortIsland.Reports;

namespace ComfortIsland.UnitTests.Reports
{
	public class TradeReportTest
	{
		[Test]
		public void ReportBeforeAllChangesLooksEmpty()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			var report = new TradeReport(database, new Period(beginDate.AddDays(-1), beginDate.AddDays(-1)));
			Assert.AreEqual(3, report.TradeItems.Count());
			TradeItem child1Item = report.TradeItems.First(item => item.ProductId == childProduct1.ID);
			Assert.AreEqual(0, child1Item.InitialBalance);
			Assert.AreEqual(0, child1Item.Income);
			Assert.AreEqual(0, child1Item.Produced);
			Assert.AreEqual(0, child1Item.Selled);
			Assert.AreEqual(0, child1Item.UsedToProduce);
			Assert.AreEqual(0, child1Item.SentToWarehouse);
			Assert.AreEqual(0, child1Item.FinalBalance);
			TradeItem child2Item = report.TradeItems.First(item => item.ProductId == childProduct2.ID);
			Assert.AreEqual(0, child2Item.InitialBalance);
			Assert.AreEqual(0, child2Item.Income);
			Assert.AreEqual(0, child2Item.Produced);
			Assert.AreEqual(0, child2Item.Selled);
			Assert.AreEqual(0, child2Item.UsedToProduce);
			Assert.AreEqual(0, child2Item.SentToWarehouse);
			Assert.AreEqual(0, child2Item.FinalBalance);
			TradeItem parentItem = report.TradeItems.First(item => item.ProductId == parentProduct.ID);
			Assert.AreEqual(0, parentItem.InitialBalance);
			Assert.AreEqual(0, parentItem.Income);
			Assert.AreEqual(0, parentItem.Produced);
			Assert.AreEqual(0, parentItem.Selled);
			Assert.AreEqual(0, parentItem.UsedToProduce);
			Assert.AreEqual(0, parentItem.SentToWarehouse);
			Assert.AreEqual(0, parentItem.FinalBalance);
		}

		[Test]
		public void ReportAfterAllChangesLooksEmpty()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			var report = new TradeReport(database, new Period(beginDate.AddDays(4), beginDate.AddDays(4)));
			Assert.AreEqual(3, report.TradeItems.Count());
			var child1Item = report.TradeItems.First(item => item.ProductId == childProduct1.ID);
			Assert.AreEqual(0, child1Item.InitialBalance);
			Assert.AreEqual(0, child1Item.Income);
			Assert.AreEqual(0, child1Item.Produced);
			Assert.AreEqual(0, child1Item.Selled);
			Assert.AreEqual(0, child1Item.UsedToProduce);
			Assert.AreEqual(0, child1Item.SentToWarehouse);
			Assert.AreEqual(0, child1Item.FinalBalance);
			var child2Item = report.TradeItems.First(item => item.ProductId == childProduct2.ID);
			Assert.AreEqual(0, child2Item.InitialBalance);
			Assert.AreEqual(0, child2Item.Income);
			Assert.AreEqual(0, child2Item.Produced);
			Assert.AreEqual(0, child2Item.Selled);
			Assert.AreEqual(0, child2Item.UsedToProduce);
			Assert.AreEqual(0, child2Item.SentToWarehouse);
			Assert.AreEqual(0, child2Item.FinalBalance);
			var parentItem = report.TradeItems.First(item => item.ProductId == parentProduct.ID);
			Assert.AreEqual(0, parentItem.InitialBalance);
			Assert.AreEqual(0, parentItem.Income);
			Assert.AreEqual(0, parentItem.Produced);
			Assert.AreEqual(0, parentItem.Selled);
			Assert.AreEqual(0, parentItem.UsedToProduce);
			Assert.AreEqual(0, parentItem.SentToWarehouse);
			Assert.AreEqual(0, parentItem.FinalBalance);
		}

		[Test]
		public void CheckAllChanges()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			var report = new TradeReport(database, new Period(beginDate.AddDays(-1), beginDate.AddDays(4)));
			Assert.AreEqual(3, report.TradeItems.Count());
			TradeItem child1Item = report.TradeItems.First(item => item.ProductId == childProduct1.ID);
			Assert.AreEqual(0, child1Item.InitialBalance);
			Assert.AreEqual(100, child1Item.Income);
			Assert.AreEqual(0, child1Item.Produced);
			Assert.AreEqual(0, child1Item.Selled);
			Assert.AreEqual(100, child1Item.UsedToProduce);
			Assert.AreEqual(0, child1Item.SentToWarehouse);
			Assert.AreEqual(0, child1Item.FinalBalance);
			TradeItem child2Item = report.TradeItems.First(item => item.ProductId == childProduct2.ID);
			Assert.AreEqual(0, child2Item.InitialBalance);
			Assert.AreEqual(300, child2Item.Income);
			Assert.AreEqual(0, child2Item.Produced);
			Assert.AreEqual(100, child2Item.Selled);
			Assert.AreEqual(200, child2Item.UsedToProduce);
			Assert.AreEqual(0, child2Item.SentToWarehouse);
			Assert.AreEqual(0, child2Item.FinalBalance);
			TradeItem parentItem = report.TradeItems.First(item => item.ProductId == parentProduct.ID);
			Assert.AreEqual(0, parentItem.InitialBalance);
			Assert.AreEqual(0, parentItem.Income);
			Assert.AreEqual(100, parentItem.Produced);
			Assert.AreEqual(0, parentItem.Selled);
			Assert.AreEqual(0, parentItem.UsedToProduce);
			Assert.AreEqual(100, parentItem.SentToWarehouse);
			Assert.AreEqual(0, parentItem.FinalBalance);
		}

		[Test]
		public void CheckIncome()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			var report = new TradeReport(database, new Period(beginDate, beginDate));
			Assert.AreEqual(3, report.TradeItems.Count());
			TradeItem child1Item = report.TradeItems.First(item => item.ProductId == childProduct1.ID);
			Assert.AreEqual(0, child1Item.InitialBalance);
			Assert.AreEqual(100, child1Item.Income);
			Assert.AreEqual(0, child1Item.Produced);
			Assert.AreEqual(0, child1Item.Selled);
			Assert.AreEqual(0, child1Item.UsedToProduce);
			Assert.AreEqual(0, child1Item.SentToWarehouse);
			Assert.AreEqual(100, child1Item.FinalBalance);
			TradeItem child2Item = report.TradeItems.First(item => item.ProductId == childProduct2.ID);
			Assert.AreEqual(0, child2Item.InitialBalance);
			Assert.AreEqual(300, child2Item.Income);
			Assert.AreEqual(0, child2Item.Produced);
			Assert.AreEqual(0, child2Item.Selled);
			Assert.AreEqual(0, child2Item.UsedToProduce);
			Assert.AreEqual(0, child2Item.SentToWarehouse);
			Assert.AreEqual(300, child2Item.FinalBalance);
			TradeItem parentItem = report.TradeItems.First(item => item.ProductId == parentProduct.ID);
			Assert.AreEqual(0, parentItem.InitialBalance);
			Assert.AreEqual(0, parentItem.Income);
			Assert.AreEqual(0, parentItem.Produced);
			Assert.AreEqual(0, parentItem.Selled);
			Assert.AreEqual(0, parentItem.UsedToProduce);
			Assert.AreEqual(0, parentItem.SentToWarehouse);
			Assert.AreEqual(0, parentItem.FinalBalance);
		}

		[Test]
		public void CheckOutcome()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			var report = new TradeReport(database, new Period(beginDate.AddDays(1), beginDate.AddDays(1)));
			Assert.AreEqual(3, report.TradeItems.Count());
			TradeItem child1Item = report.TradeItems.First(item => item.ProductId == childProduct1.ID);
			Assert.AreEqual(100, child1Item.InitialBalance);
			Assert.AreEqual(0, child1Item.Income);
			Assert.AreEqual(0, child1Item.Produced);
			Assert.AreEqual(0, child1Item.Selled);
			Assert.AreEqual(0, child1Item.UsedToProduce);
			Assert.AreEqual(0, child1Item.SentToWarehouse);
			Assert.AreEqual(100, child1Item.FinalBalance);
			TradeItem child2Item = report.TradeItems.First(item => item.ProductId == childProduct2.ID);
			Assert.AreEqual(300, child2Item.InitialBalance);
			Assert.AreEqual(0, child2Item.Income);
			Assert.AreEqual(0, child2Item.Produced);
			Assert.AreEqual(100, child2Item.Selled);
			Assert.AreEqual(0, child2Item.UsedToProduce);
			Assert.AreEqual(0, child2Item.SentToWarehouse);
			Assert.AreEqual(200, child2Item.FinalBalance);
			TradeItem parentItem = report.TradeItems.First(item => item.ProductId == parentProduct.ID);
			Assert.AreEqual(0, parentItem.InitialBalance);
			Assert.AreEqual(0, parentItem.Income);
			Assert.AreEqual(0, parentItem.Produced);
			Assert.AreEqual(0, parentItem.Selled);
			Assert.AreEqual(0, parentItem.UsedToProduce);
			Assert.AreEqual(0, parentItem.SentToWarehouse);
			Assert.AreEqual(0, parentItem.FinalBalance);
		}

		[Test]
		public void CheckProduced()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			var report = new TradeReport(database, new Period(beginDate.AddDays(2), beginDate.AddDays(2)));
			Assert.AreEqual(3, report.TradeItems.Count());
			TradeItem child1Item = report.TradeItems.First(item => item.ProductId == childProduct1.ID);
			Assert.AreEqual(100, child1Item.InitialBalance);
			Assert.AreEqual(0, child1Item.Income);
			Assert.AreEqual(0, child1Item.Produced);
			Assert.AreEqual(0, child1Item.Selled);
			Assert.AreEqual(100, child1Item.UsedToProduce);
			Assert.AreEqual(0, child1Item.SentToWarehouse);
			Assert.AreEqual(0, child1Item.FinalBalance);
			TradeItem child2Item = report.TradeItems.First(item => item.ProductId == childProduct2.ID);
			Assert.AreEqual(200, child2Item.InitialBalance);
			Assert.AreEqual(0, child2Item.Income);
			Assert.AreEqual(0, child2Item.Produced);
			Assert.AreEqual(0, child2Item.Selled);
			Assert.AreEqual(200, child2Item.UsedToProduce);
			Assert.AreEqual(0, child2Item.SentToWarehouse);
			Assert.AreEqual(0, child2Item.FinalBalance);
			TradeItem parentItem = report.TradeItems.First(item => item.ProductId == parentProduct.ID);
			Assert.AreEqual(0, parentItem.InitialBalance);
			Assert.AreEqual(0, parentItem.Income);
			Assert.AreEqual(100, parentItem.Produced);
			Assert.AreEqual(0, parentItem.Selled);
			Assert.AreEqual(0, parentItem.UsedToProduce);
			Assert.AreEqual(0, parentItem.SentToWarehouse);
			Assert.AreEqual(100, parentItem.FinalBalance);
		}

		[Test]
		public void CheckToWarehouse()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			var report = new TradeReport(database, new Period(beginDate.AddDays(3), beginDate.AddDays(3)));
			Assert.AreEqual(3, report.TradeItems.Count());
			TradeItem child1Item = report.TradeItems.First(item => item.ProductId == childProduct1.ID);
			Assert.AreEqual(0, child1Item.InitialBalance);
			Assert.AreEqual(0, child1Item.Income);
			Assert.AreEqual(0, child1Item.Produced);
			Assert.AreEqual(0, child1Item.Selled);
			Assert.AreEqual(0, child1Item.UsedToProduce);
			Assert.AreEqual(0, child1Item.SentToWarehouse);
			Assert.AreEqual(0, child1Item.FinalBalance);
			TradeItem child2Item = report.TradeItems.First(item => item.ProductId == childProduct2.ID);
			Assert.AreEqual(0, child2Item.InitialBalance);
			Assert.AreEqual(0, child2Item.Income);
			Assert.AreEqual(0, child2Item.Produced);
			Assert.AreEqual(0, child2Item.Selled);
			Assert.AreEqual(0, child2Item.UsedToProduce);
			Assert.AreEqual(0, child2Item.SentToWarehouse);
			Assert.AreEqual(0, child2Item.FinalBalance);
			TradeItem parentItem = report.TradeItems.First(item => item.ProductId == parentProduct.ID);
			Assert.AreEqual(100, parentItem.InitialBalance);
			Assert.AreEqual(0, parentItem.Income);
			Assert.AreEqual(0, parentItem.Produced);
			Assert.AreEqual(0, parentItem.Selled);
			Assert.AreEqual(0, parentItem.UsedToProduce);
			Assert.AreEqual(100, parentItem.SentToWarehouse);
			Assert.AreEqual(0, parentItem.FinalBalance);
		}

		[Test]
		public void ReportDoesNotRuinBalance()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product childProduct1, childProduct2, parentProduct;
			var database = createTestBase(beginDate, out childProduct1, out childProduct2, out parentProduct);

			// act and assert
			Assert.AreEqual(0, database.Balance.ToList().Count());

			for (int addBegin = -1; addBegin <= 3; addBegin++)
			{
				for (int addEnd = -1; addEnd <= 3; addEnd++)
				{
					new TradeReport(database, new Period(beginDate.AddDays(addBegin), beginDate.AddDays(addEnd)));
					Assert.AreEqual(0, database.Balance.ToList().Count());
				}
			}
		}

		private static Database createTestBase(DateTime date, out Product childProduct1, out Product childProduct2, out Product parentProduct)
		{
			var unit = new Unit
			{
				ID = 1,
				Name = "Full Name",
				ShortName = "short",
			};
			childProduct1 = new Product
			{
				ID = 1,
				Name = "CHILD 1",
				Unit = unit,
			};
			childProduct2 = new Product
			{
				ID = 2,
				Name = "CHILD 2",
				Unit = unit,
			};
			parentProduct = new Product
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
			var database = new Database
			(
				new[] { unit },
				new[] { childProduct1, childProduct2, parentProduct },
				new Dictionary<long, decimal>(),
				new[]
				{
					new Document(DocumentType.Income)
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
					new Document(DocumentType.Outcome)
					{
						ID = 2,
						Number = date.AddDays(1).ToShortDateString(),
						Date = date.AddDays(1),
						Positions = { { childProduct2, 100 } },
					},
					new Document(DocumentType.Produce)
					{
						ID = 3,
						Number = date.AddDays(2).ToShortDateString(),
						Date = date.AddDays(2),
						Positions = { { parentProduct, 100 } },
					},
					new Document(DocumentType.ToWarehouse)
					{
						ID = 4,
						Number = date.AddDays(3).ToShortDateString(),
						Date = date.AddDays(3),
						Positions = { { parentProduct, 100 } },
					},
				});
			return database;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Accounting.Core.BusinessLogic;
using Accounting.Core.Reports.Existing;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.UnitTests.Reports
{
	public class BalanceReportTest
	{
		[Test]
		public void HappyPath()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product product;
			var database = createTestBase(beginDate, out product);

			// act and assert
			var report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(-1), false));
			Assert.AreEqual(0, report.Items.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(-1), true));
			var item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(0, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate, false));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(10, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate, true));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(10, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(1), false));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(5, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(1), true));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(5, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(2), false));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(10, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(2), true));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(10, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(3), false));
			Assert.AreEqual(0, report.Items.OfType<Position>().Count());

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(3), true));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(0, item.Count);

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(4), false));
			Assert.AreEqual(0, report.Items.OfType<Position>().Count());

			report = new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(4), true));
			item = report.Items.OfType<Position>().Single();
			Assert.AreSame(product, item.BoundProduct);
			Assert.AreEqual(0, item.Count);
		}

		[Test]
		public void ReportDoesNotRuinBalance()
		{
			// arrange
			var beginDate = DateTime.Now;
			Product product;
			var database = createTestBase(beginDate, out product);

			// act and assert
			Assert.AreEqual(0, database.Balance.ToList().Count());

			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(-1), false));
			Assert.AreEqual(0, database.Balance.ToList().Count());
			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(-1), true));
			Assert.AreEqual(0, database.Balance.ToList().Count());

			new BalanceReport(database, new BalanceReportParams(beginDate, false));
			Assert.AreEqual(0, database.Balance.ToList().Count());
			new BalanceReport(database, new BalanceReportParams(beginDate, true));
			Assert.AreEqual(0, database.Balance.ToList().Count());

			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(1), false));
			Assert.AreEqual(0, database.Balance.ToList().Count());
			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(1), true));
			Assert.AreEqual(0, database.Balance.ToList().Count());

			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(2), false));
			Assert.AreEqual(0, database.Balance.ToList().Count());
			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(2), true));
			Assert.AreEqual(0, database.Balance.ToList().Count());

			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(3), false));
			Assert.AreEqual(0, database.Balance.ToList().Count());
			new BalanceReport(database, new BalanceReportParams(beginDate.AddDays(3), true));
			Assert.AreEqual(0, database.Balance.ToList().Count());
		}

		private static Database createTestBase(DateTime date, out Product product)
		{
			var unit = new Unit
			{
				ID = 1,
				Name = "Full Name",
				ShortName = "short",
			};
			product = new Product
			{
				ID = 1,
				Name = "1",
				Unit = unit,
			};
			var database = new Database
			(
				new[] { unit },
				new[] { product },
				new Dictionary<long, decimal>(),
				new[]
				{
					new Document(DocumentType.Income)
					{
						ID = 1,
						Number = date.ToShortDateString(),
						Date = date,
						Positions = { { product, 10 } },
					},
					new Document(DocumentType.Outcome)
					{
						ID = 2,
						Number = date.AddDays(1).ToShortDateString(),
						Date = date.AddDays(1),
						Positions = { { product, 5 } },
					},
					new Document(DocumentType.Income)
					{
						ID = 3,
						Number = date.AddDays(2).ToShortDateString(),
						Date = date.AddDays(2),
						Positions = { { product, 5 } },
					},
					new Document(DocumentType.Outcome)
					{
						ID = 4,
						Number = date.AddDays(3).ToShortDateString(),
						Date = date.AddDays(3),
						Positions = { { product, 10 } },
					},
				});
			return database;
		}
	}
}

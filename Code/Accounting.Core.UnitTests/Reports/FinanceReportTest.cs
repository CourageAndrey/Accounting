using System;
using System.Collections.Generic;

using NUnit.Framework;

using Accounting.Core.BusinessLogic;
using Accounting.Core.Reports.Existing;
using Accounting.Core.Reports.Params;

namespace Accounting.Core.UnitTests.Reports
{
	public class FinanceReportTest
	{
		[Test]
		public void NoFinancesBeforeDocuments()
		{
			// arrange
			var beginDate = DateTime.Now;
			var database = createTestBase(beginDate);

			// act
			var report = new FinanceReport(database, new PeriodParams(beginDate.AddDays(-1), beginDate.AddDays(-1)));

			// assert
			Assert.AreEqual(2, report.Items.Count);
			var income = (FinanceItem) report.Items[0];
			var outcome = (FinanceItem) report.Items[1];
			Assert.AreEqual(0, income.Value);
			Assert.AreEqual(0, outcome.Value);
		}

		[Test]
		public void NoFinancesAfterDocuments()
		{
			// arrange
			var beginDate = DateTime.Now;
			var database = createTestBase(beginDate);

			// act
			var report = new FinanceReport(database, new PeriodParams(beginDate.AddDays(8), beginDate.AddDays(8)));

			// assert
			Assert.AreEqual(2, report.Items.Count);
			var income = (FinanceItem) report.Items[0];
			var outcome = (FinanceItem) report.Items[1];
			Assert.AreEqual(0, income.Value);
			Assert.AreEqual(0, outcome.Value);
		}

		[Test]
		public void SummAllFinances()
		{
			// arrange
			var beginDate = DateTime.Now;
			var database = createTestBase(beginDate);

			// act
			var report = new FinanceReport(database, new PeriodParams(beginDate, beginDate.AddDays(7)));

			// assert
			Assert.AreEqual(2, report.Items.Count);
			var income = (FinanceItem) report.Items[0];
			var outcome = (FinanceItem) report.Items[1];
			Assert.AreEqual(6, income.Value);
			Assert.AreEqual(8, outcome.Value);
		}

		private static Database createTestBase(DateTime date)
		{
			var database = new Database
			(
				new Unit[0],
				new Product[0],
				new Dictionary<long, decimal>(),
				new[]
				{
					new Document(DocumentType.Income)
					{
						ID = 1,
						Number = date.ToShortDateString(),
						Date = date,
						Summ = 1,
					},
					new Document(DocumentType.Outcome)
					{
						ID = 2,
						Number = date.AddDays(1).ToShortDateString(),
						Date = date.AddDays(1),
						Summ = 2,
					},
					new Document(DocumentType.Produce)
					{
						ID = 3,
						Number = date.AddDays(2).ToShortDateString(),
						Date = date.AddDays(2),
						Summ = 3,
					},
					new Document(DocumentType.ToWarehouse)
					{
						ID = 4,
						Number = date.AddDays(3).ToShortDateString(),
						Date = date.AddDays(3),
						Summ = 4,
					},
					new Document(DocumentType.Income)
					{
						ID = 5,
						Number = date.AddDays(4).ToShortDateString(),
						Date = date.AddDays(4),
						Summ = 5,
					},
					new Document(DocumentType.Outcome)
					{
						ID = 6,
						Number = date.AddDays(5).ToShortDateString(),
						Date = date.AddDays(5),
						Summ = 6,
					},
				});
			return database;
		}
	}
}

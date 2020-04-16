using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using NUnit.Framework;

using Accounting.Core.BusinessLogic;
using Accounting.Core.Helpers;
using Accounting.Core.Reports;
using Accounting.Core.Reports.Existing;
using Accounting.Core.Reports.Params;

namespace Accounting.Reports.OpenXml.UnitTests
{
	public class ReportExportTest
	{
		[Test]
		public void ExportTradeReportSmoke()
		{
			var database = createTestBase();
			var now = DateTime.Now;
			var report = new TradeReport(database, new PeriodParams(now.AddDays(-2), now.AddDays(2)));

			smokeTestReportExport(report);
		}

		[Test]
		public void ExportBalanceReportSmoke()
		{
			var database = createTestBase();
			var report = new BalanceReport(database, new BalanceReportParams(DateTime.Now, true));

			smokeTestReportExport(report);
		}

		private static IDatabase createTestBase()
		{
			var date = DateTime.Now;
			var unit = new Unit
			{
				ID = 1,
				Name = "Full Name",
				ShortName = "short",
			};
			var product = new Product
			{
				ID = 1,
				Name = "1",
				Unit = unit,
			};
			var database = new InMemoryDatabase
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

		private static void smokeTestReportExport(IReport report)
		{
			// arrange
			var exporter = new ExcelOpenXmlReportExporter();
			string reportFileName = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
			string excelHeaderSearchString = Path.GetFileNameWithoutExtension(reportFileName);

			// act and assert
			try
			{
				Assert.DoesNotThrow(() => exporter.ExportReport(report, reportFileName));

				reportFileName.ShellOpen();
			}
			finally
			{
				Process excelProcess;
				const int maxSecondsToAwait = 5;
				int secondsAwaited = 0;
				do
				{
					Thread.Sleep(1000);
					excelProcess = Process.GetProcesses().FirstOrDefault(p => p.MainWindowTitle.Contains(excelHeaderSearchString));
					secondsAwaited++;
				} while (excelProcess == null && secondsAwaited < maxSecondsToAwait);

				if (excelProcess != null)
				{
					excelProcess.Kill();
					excelProcess.WaitForExit();
				}
				else
				{
					Assert.Fail("Excel is not started!");
				}

				if (File.Exists(reportFileName))
				{
					File.Delete(reportFileName);
				}
			}
		}
	}
}

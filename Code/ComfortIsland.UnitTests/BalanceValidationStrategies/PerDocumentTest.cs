using System;
using System.Text;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.UnitTests.BalanceValidationStrategies
{
	public class PerDocumentTest
	{
		[Test]
		public void OutcomeBeforeLargeIncomeIsNotAllowedAdd()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDocument;
			Document document, income;
			var errors = new StringBuilder();

			// act and assert
			bool result = BalanceCheckWorkflowHelper.TryToAddAfterSecond(database, 10, validationStrategy, errors, out income);
			Assert.IsTrue(result);
			income.Apply(database);
			database.Documents.Add(income);

			result = BalanceCheckWorkflowHelper.TryToAddAfterFirst(database, -10, validationStrategy, errors, out document);
			Assert.IsFalse(result);
		}

		[Test]
		public void OutcomeBeforeLargeIncomeIsNotAllowedEdit()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDocument;
			Document document, income;
			var errors = new StringBuilder();

			// act and assert
			bool result = BalanceCheckWorkflowHelper.TryToAddAfterSecond(database, 10, validationStrategy, errors, out income);
			Assert.IsTrue(result);
			income.Apply(database);
			database.Documents.Add(income);

			result = BalanceCheckWorkflowHelper.TryToEditSecond(database, doc => new Tuple<double, int>(5, 0), validationStrategy, errors, out document);
			Assert.IsFalse(result);
		}

		[Test]
		public void DeleteBeforeLargeIncomeIsNotAllowed()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDocument;
			Document income;
			var errors = new StringBuilder();

			// act and assert
			bool result = BalanceCheckWorkflowHelper.TryToAddAfterAll(database, 10, validationStrategy, errors, out income);
			Assert.IsTrue(result);
			income.Apply(database);
			database.Documents.Add(income);

			result = BalanceCheckWorkflowHelper.TryToDelete(database, new[] { database.Documents[3] }, validationStrategy, errors);
			Assert.IsFalse(result);
		}
	}
}

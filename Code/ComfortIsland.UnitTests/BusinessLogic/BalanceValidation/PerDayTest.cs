using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.UnitTests.BusinessLogic.BalanceValidation
{
	public class PerDayTest
	{
		[Test]
		public void FinalDayBalanceHasToBePositiveAdd()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDay;
			var errors = new StringBuilder();
			Document document;

			// act
			bool result = BalanceCheckWorkflowHelper.TryToAddAfterFirst(database, -5, validationStrategy, errors, out document);
			document.Apply(database);

			// assert
			Assert.IsFalse(result);
			Assert.AreEqual(-4, database.Balance.Single().Value);
		}

		[Test]
		public void FinalDayBalanceHasToBePositiveEdit()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDay;
			var errors = new StringBuilder();
			Document original = null, edited, income;

			// act
			bool result = BalanceCheckWorkflowHelper.TryToEditSecond(
				database,
				doc =>
				{
					original = doc;
					return new Tuple<decimal, int>(4, 1);
				},
				validationStrategy,
				errors,
				out edited);
			Assert.IsFalse(result);

			result = BalanceCheckWorkflowHelper.TryToAddAfterSecond(database, 1, validationStrategy, errors, out income);
			Assert.IsTrue(result);
			income.Apply(database);

			result = BalanceCheckWorkflowHelper.TryToEditSecond(
				database,
				doc =>
				{
					original = doc;
					return new Tuple<decimal, int>(4, 0);
				},
				validationStrategy,
				errors,
				out edited);
			Assert.IsFalse(result);

			result = BalanceCheckWorkflowHelper.TryToEditFirst(
				database,
				doc =>
				{
					original = doc;
					return new Tuple<decimal, int>(-2, 0);
				},
				validationStrategy,
				errors,
				out edited);
			Assert.IsTrue(result);
			original.Rollback(database);
			edited.Apply(database);

			Assert.AreEqual(0, database.Balance.Single().Value);
		}

		[Test]
		public void DuringDayBalanceCanBeNegativeAdd()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDay;
			var errors = new StringBuilder();
			Document income, document;

			// act and assert
			bool result = BalanceCheckWorkflowHelper.TryToAddAfterFirst(database, -16, validationStrategy, errors, out document);
			Assert.IsFalse(result);

			result = BalanceCheckWorkflowHelper.TryToAddAfterSecond(database, 10, validationStrategy, errors, out income);
			Assert.IsTrue(result);
			income.Apply(database);

			result = BalanceCheckWorkflowHelper.TryToAddAfterFirst(database, -10, validationStrategy, errors, out document);
			Assert.IsTrue(result);
			document.Apply(database);

			// assert
			Assert.AreEqual(1, database.Balance.Single().Value);
		}

		[Test]
		public void DuringDayBalanceCanBeNegativeEdit()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDay;
			var errors = new StringBuilder();
			Document original = null, edited;

			// act and assert
			bool result = BalanceCheckWorkflowHelper.TryToEditThird(
				database,
				doc =>
				{
					original = doc;
					return new Tuple<decimal, int>(5, 0);
				},
				validationStrategy,
				errors,
				out edited);
			Assert.IsTrue(result);
			edited.Apply(database);
			original.Rollback(database);
			database.Documents.Add(edited);

			result = BalanceCheckWorkflowHelper.TryToEditSecond(
				database,
				doc =>
				{
					original = doc;
					return new Tuple<decimal, int>(5, 0);
				},
				validationStrategy,
				errors,
				out edited);
			Assert.IsFalse(result);
		}

		[Test]
		public void DuringDayBalanceCanBeNegativeDelete()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.PerDay;
			var errors = new StringBuilder();
			Document original = null, edited;

			// act and assert
			bool result = BalanceCheckWorkflowHelper.TryToEditThird(
				database,
				doc =>
				{
					original = doc;
					return new Tuple<decimal, int>(4, 0);
				},
				validationStrategy,
				errors,
				out edited);
			Assert.IsTrue(result);
			edited.Apply(database);
			original.Rollback(database);
			database.Documents.Add(edited);

			result = BalanceCheckWorkflowHelper.TryToDelete(database, new[] { database.Documents[1] }, validationStrategy, errors);
			Assert.IsFalse(result);
		}
	}
}

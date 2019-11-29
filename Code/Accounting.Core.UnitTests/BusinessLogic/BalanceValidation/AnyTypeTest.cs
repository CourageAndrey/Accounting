using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.UnitTests.BusinessLogic.BalanceValidation
{
	public class AnyTypeTest
	{
		[Test]
		public void ImpossibleToOutcomeTwoMoreAnywhereExceptNoVerify()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();
			Document document;

			// act and assert
			foreach (var validationStrategy in new[] { BalanceValidationStrategy.FinalOnly, BalanceValidationStrategy.PerDay, BalanceValidationStrategy.PerDocument })
			{
				foreach (var checkMethod in BalanceCheckWorkflowHelper.AddCheckers)
				{
					bool result = checkMethod(database, -2, validationStrategy, errors, out document);
					Assert.IsFalse(result);
				}
			}
		}

		[Test]
		public void PossibleToIncreaseAnyOutcomeOrDecreaseAnyIncomeBy1()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();
			Document original = null, edited;

			// act and assert
			foreach (var validationStrategy in new[] { BalanceValidationStrategy.FinalOnly, BalanceValidationStrategy.PerDay, BalanceValidationStrategy.PerDocument })
			{
				foreach (var checkMethod in BalanceCheckWorkflowHelper.EditCheckers)
				{
					bool result = checkMethod(
						database,
						doc =>
						{
							original = doc;
							return new Tuple<decimal, int>(
								doc.Type == DocumentType.Income ? -1 : 1,
								0);
						},
						validationStrategy,
						errors,
						out edited);
					Assert.IsTrue(result);

					original.RollbackBalanceChanges(database.Balance);
					edited.ApplyBalanceChanges(database.Balance);
					Assert.AreEqual(0, database.Balance.First().Value);
					edited.RollbackBalanceChanges(database.Balance);
					original.ApplyBalanceChanges(database.Balance);
				}
			}
		}

		[Test]
		public void ForbiddenToIncreaseAnyOutcomeOrDecreaseAnyIncomeBy2()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();
			Document original = null, edited;

			// act and assert
			foreach (var validationStrategy in new[] { BalanceValidationStrategy.FinalOnly, BalanceValidationStrategy.PerDay, BalanceValidationStrategy.PerDocument })
			{
				foreach (var checkMethod in BalanceCheckWorkflowHelper.EditCheckers)
				{
					bool result = checkMethod(
						database,
						doc =>
						{
							original = doc;
							return new Tuple<decimal, int>(
								doc.Type == DocumentType.Income ? -2 : 2,
								0);
						},
						validationStrategy,
						errors,
						out edited);
					Assert.IsFalse(result);
				}
			}
		}

		[Test]
		public void PossibleToDeleteAnyOutcome()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();

			// act and assert
			foreach (var validationStrategy in BalanceValidationStrategy.All.Values)
			{
				foreach (var document in database.Documents.Where(doc => doc.Type == DocumentType.Outcome))
				{
					Assert.IsTrue(validationStrategy.VerifyDelete(database, new[] { document }, errors));
				}
			}
		}

		[Test]
		public void PossibleToDecreaseAnyOutcomeOrIncreaseAnyIncome()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();
			Document original = null, edited;

			// act and assert
			foreach (var validationStrategy in BalanceValidationStrategy.All.Values)
			{
				foreach (var checkMethod in BalanceCheckWorkflowHelper.EditCheckers)
				{
					bool result = checkMethod(
						database,
						doc =>
						{
							original = doc;
							return new Tuple<decimal, int>(
								doc.Type == DocumentType.Income
									? doc.Positions.Values.First() * 10
									: -(doc.Positions.Values.First() - 1),
								0);
						},
						validationStrategy,
						errors,
						out edited);
					Assert.IsTrue(result);

					original.RollbackBalanceChanges(database.Balance);
					edited.ApplyBalanceChanges(database.Balance);
					Assert.Greater(database.Balance.First().Value, 0);
					edited.RollbackBalanceChanges(database.Balance);
					original.ApplyBalanceChanges(database.Balance);
				}
			}
		}

		[Test]
		public void PossibleToAddIncomeAnywhere()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();
			Document document;

			// act and assert
			foreach (var validationStrategy in BalanceValidationStrategy.All.Values)
			{
				foreach (var checkMethod in BalanceCheckWorkflowHelper.AddCheckers)
				{
					bool result = checkMethod(database, 10, validationStrategy, errors, out document);
					Assert.IsTrue(result);
				}
			}
		}

		[Test]
		public void AnyIncomeCanMoveDateToBeginning()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();
			Document edited;

			// act and assert
			foreach (var validationStrategy in BalanceValidationStrategy.All.Values)
			{
				bool result = BalanceCheckWorkflowHelper.TryToEditFirst(
					database,
					doc => new Tuple<decimal, int>(0, -72),
					validationStrategy,
					errors,
					out edited);
				Assert.IsTrue(result);

				result = BalanceCheckWorkflowHelper.TryToEditThird(
					database,
					doc => new Tuple<decimal, int>(0, -72),
					validationStrategy,
					errors,
					out edited);
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void AnyOutcomeCanMoveDateToEnding()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var errors = new StringBuilder();
			Document edited;

			// act and assert
			foreach (var validationStrategy in BalanceValidationStrategy.All.Values)
			{
				bool result = BalanceCheckWorkflowHelper.TryToEditSecond(
					database,
					doc => new Tuple<decimal, int>(0, 72),
					validationStrategy,
					errors,
					out edited);
				Assert.IsTrue(result);

				result = BalanceCheckWorkflowHelper.TryToEditFourth(
					database,
					doc => new Tuple<decimal, int>(0, 72),
					validationStrategy,
					errors,
					out edited);
				Assert.IsTrue(result);
			}
		}
	}
}

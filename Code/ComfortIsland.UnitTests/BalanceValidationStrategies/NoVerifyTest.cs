﻿using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.UnitTests.BalanceValidationStrategies
{
	public class NoVerifyTest
	{
		[Test]
		public void PossibleToOutcomeToMinus()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.NoVerify;
			var errors = new StringBuilder();
			Document document;

			// act and assert
			foreach (var checkMethod in BalanceCheckWorkflowHelper.AddCheckers)
			{
				bool result = checkMethod(database, -100, validationStrategy, errors, out document);
				Assert.IsTrue(result);

				document.Apply(database);
				Assert.AreEqual(-99, database.Balance.First().Value);
				document.Rollback(database);
			}
		}

		[Test]
		public void PossibleToDeleteFirstIncome()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.NoVerify;
			var errors = new StringBuilder();

			// act
			var documentsToDelete = new[] { database.Documents.First(document => document.Type == DocumentType.Income) };
			bool result = BalanceCheckWorkflowHelper.TryToDelete(
				database,
				documentsToDelete,
				validationStrategy,
				errors);
			foreach (var document in documentsToDelete)
			{
				document.Rollback(database);
			}

			// assert
			Assert.IsTrue(result);
			Assert.AreEqual(-4, database.Balance.First().Value);
		}

		[Test]
		public void PossibleToDeleteLastIncome()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.NoVerify;
			var errors = new StringBuilder();

			// act
			var documentsToDelete = new[] { database.Documents.Last(document => document.Type == DocumentType.Income) };
			bool result = BalanceCheckWorkflowHelper.TryToDelete(
				database,
				documentsToDelete,
				validationStrategy,
				errors);
			foreach (var document in documentsToDelete)
			{
				document.Rollback(database);
			}

			// assert
			Assert.IsTrue(result);
			Assert.AreEqual(-7, database.Balance.First().Value);
		}

		[Test]
		public void PossibleToIncreaseAnyOutcomeOrDecreaseAnyIncome()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.NoVerify;
			var errors = new StringBuilder();
			Document original = null, edited;

			// act and assert
			foreach (var checkMethod in BalanceCheckWorkflowHelper.EditCheckers)
			{
				bool result = checkMethod(
					database,
					doc =>
					{
						original = doc;
						return new Tuple<decimal, int>(
							doc.Type == DocumentType.Income
								? -(doc.Positions.Values.First() - 1)
								: doc.Positions.Values.First()*10,
							0);
					},
					validationStrategy,
					errors,
					out edited);
				Assert.IsTrue(result);

				original.Rollback(database);
				edited.Apply(database);
				Assert.Greater(0, database.Balance.First().Value);
				edited.Rollback(database);
				original.Apply(database);
			}
		}
	}
}
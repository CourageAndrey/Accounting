using System.Linq;
using System.Text;

using NUnit.Framework;

using Accounting.Core.BusinessLogic;

namespace Accounting.Core.UnitTests.BusinessLogic.BalanceValidation
{
	public class FinalOnlyTest
	{
		[Test]
		public void ForbiddenToDeleteAnyIncome()
		{
			// arrange
			var database = BalanceCheckWorkflowHelper.CreateComplexDatabase();
			var validationStrategy = BalanceValidationStrategy.FinalOnly;
			var errors = new StringBuilder();

			// act and assert
			foreach (var document in database.Documents.Where(doc => doc.Type == DocumentType.Income))
			{
				Assert.IsFalse(validationStrategy.VerifyDelete(database, new[] { document }, errors));
			}
		}
	}
}

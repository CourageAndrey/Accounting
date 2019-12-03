using System.Collections.Generic;
using System.Text;

namespace Accounting.Core.BusinessLogic.BalanceValidationStrategies
{
	public class NoVerifyBalanceValidationStrategy : BalanceValidationStrategy
	{
		#region Overrides

		public override bool VerifyCreate(Database database, Document document, StringBuilder errors)
		{
			return true;
		}

		public override bool VerifyEdit(Database database, Document document, StringBuilder errors)
		{
			return true;
		}

		public override bool VerifyDelete(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors)
		{
			return true;
		}

		#endregion
	}
}

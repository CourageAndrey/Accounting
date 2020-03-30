using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Accounting.Core.BusinessLogic.BalanceValidationStrategies;

namespace Accounting.Core.BusinessLogic
{
	public abstract class BalanceValidationStrategy
	{
		#region List

		public static readonly BalanceValidationStrategy PerDocument = new PerDocumentBalanceValidationStrategy();

		public static readonly BalanceValidationStrategy PerDay = new PerDayBalanceValidationStrategy();

		public static readonly BalanceValidationStrategy FinalOnly = new FinalOnlyBalanceValidationStrategy();

		public static readonly BalanceValidationStrategy NoVerify = new NoVerifyBalanceValidationStrategy();

		internal static readonly IReadOnlyDictionary<string, BalanceValidationStrategy> All = new ReadOnlyDictionary<string, BalanceValidationStrategy>(
			new Dictionary<string, BalanceValidationStrategy>
			{
				{ "PerDocument", PerDocument },
				{ "PerDay", PerDay },
				{ "FinalOnly", FinalOnly },
				{ "NoVerify", NoVerify },
			});

		#endregion

		#region Public API

		public abstract bool VerifyCreate(IDatabase database, Document document, StringBuilder errors);

		public abstract bool VerifyEdit(IDatabase database, Document document, StringBuilder errors);

		public abstract bool VerifyDelete(IDatabase database, IReadOnlyCollection<Document> documents, StringBuilder errors);

		#endregion
	}
}

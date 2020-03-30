using System.Collections.Generic;
using System.Text;

namespace Accounting.Core.BusinessLogic.BalanceValidationStrategies
{
	public class FinalOnlyBalanceValidationStrategy : BalanceValidationStrategy
	{
		#region Overrides

		public override bool VerifyCreate(IDatabase database, Document document, StringBuilder errors)
		{
			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// применение нового документа
			var productsToCheck = document.ApplyBalanceChanges(balance).Keys;

			// проверка итогового баланса
			return balance.Check(database.Products, errors, productsToCheck);
		}

		public override bool VerifyEdit(IDatabase database, Document document, StringBuilder errors)
		{
			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// откат старой версии документа
			var productsToCheck = new HashSet<long>(database.Documents[document.PreviousVersionId.Value].RollbackBalanceChanges(balance).Keys);

			// применение новой версии документа
			foreach (long productId in document.ApplyBalanceChanges(balance).Keys)
			{
				productsToCheck.Add(productId);
			}

			// проверка итогового баланса
			return balance.Check(database.Products, errors, productsToCheck);
		}

		public override bool VerifyDelete(IDatabase database, IReadOnlyCollection<Document> documents, StringBuilder errors)
		{
			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// последовательный откат документов
			var productsToCheck = new HashSet<long>();
			foreach (var document in documents)
			{
				foreach (long productId in document.RollbackBalanceChanges(balance).Keys)
				{
					productsToCheck.Add(productId);
				}
			}

			// проверка итогового баланса
			return balance.Check(database.Products, errors, productsToCheck);
		}

		#endregion
	}
}

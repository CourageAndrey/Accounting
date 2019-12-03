using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Core.BusinessLogic.BalanceValidationStrategies
{
	public class PerDocumentBalanceValidationStrategy : BalanceValidationStrategy
	{
		#region Overrides

		public override bool VerifyCreate(Database database, Document document, StringBuilder errors)
		{
			ICollection<long> productsToCheck;

			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// откат предыдущих документов
			var documentsRolledBack = new Stack<Document>();
			foreach (var doc in database.GetActiveDocuments().Where(d => d.Date > document.Date))
			{
				doc.RollbackBalanceChanges(balance);
				documentsRolledBack.Push(doc);
			}

			// применение нового документа
			productsToCheck = document.ApplyBalanceChanges(balance).Keys;
			if (!balance.Check(database.Products, errors, productsToCheck))
			{
				return false;
			}

			// обратный накат документов
			while (documentsRolledBack.Any())
			{
				var doc = documentsRolledBack.Pop();
				productsToCheck = doc.ApplyBalanceChanges(balance).Keys;
				if (!balance.Check(database.Products, errors, productsToCheck))
				{
					return false;
				}
			}

			return true;
		}

		public override bool VerifyEdit(Database database, Document document, StringBuilder errors)
		{
			ICollection<long> productsToCheck;
			var original = database.Documents[document.PreviousVersionId.Value];

			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// откат документов до самой маленькой даты
			var firstDate = document.Date > original.Date ? original.Date : document.Date;
			var documentsRolledBack = new Stack<Document>();
			bool needToRevertOriginal = true;
			foreach (var doc in database.GetActiveDocuments().Where(d => d.Date > firstDate))
			{
				doc.RollbackBalanceChanges(balance);
				if (doc != original)
				{
					documentsRolledBack.Push(doc);
				}
				else
				{
					needToRevertOriginal = false;
				}
			}

			// откат оригинального
			if (needToRevertOriginal)
			{
				original.RollbackBalanceChanges(balance);
			}

			// обратный накат документов
			bool editVersionApplied = false;
			while (documentsRolledBack.Any())
			{
				var doc = documentsRolledBack.Pop();
				if (!editVersionApplied && doc.Date > document.Date)
				{
					productsToCheck = document.ApplyBalanceChanges(balance).Keys;
					if (!balance.Check(database.Products, errors, productsToCheck))
					{
						return false;
					}
					editVersionApplied = true;
				}
				productsToCheck = doc.ApplyBalanceChanges(balance).Keys;
				if (!balance.Check(database.Products, errors, productsToCheck))
				{
					return false;
				}
			}
			if (!editVersionApplied)
			{
				productsToCheck = document.ApplyBalanceChanges(balance).Keys;
				if (!balance.Check(database.Products, errors, productsToCheck))
				{
					return false;
				}
			}

			return true;
		}

		public override bool VerifyDelete(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors)
		{
			ICollection<long> productsToCheck;

			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// откат предыдущих документов
			var documentsToDelete = new HashSet<Document>(documents);
			var documentsRolledBack = new Stack<Document>();
			foreach (var doc in database.GetActiveDocuments())
			{
				doc.RollbackBalanceChanges(balance);
				if (documentsToDelete.Contains(doc))
				{
					documentsToDelete.Remove(doc);
				}
				else
				{
					documentsRolledBack.Push(doc);
				}
				if (!documentsToDelete.Any())
				{
					break;
				}
			}

			// обратный накат документов
			while (documentsRolledBack.Any())
			{
				var doc = documentsRolledBack.Pop();
				productsToCheck = doc.ApplyBalanceChanges(balance).Keys;
				if (!balance.Check(database.Products, errors, productsToCheck))
				{
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}

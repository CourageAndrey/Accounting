using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Core.BusinessLogic.BalanceValidationStrategies
{
	public class PerDayBalanceValidationStrategy : BalanceValidationStrategy
	{
		#region Overrides

		public override bool VerifyCreate(IDatabase database, Document document, StringBuilder errors)
		{
			List<long> productsToCheck;

			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// откат предыдущих документов
			var documentsRolledBack = new Stack<ICollection<Document>>();
			foreach (var date in database.GetActiveDocuments().Where(d => d.Date.Date <= document.Date.Date).GroupBy(d => d.Date.Date))
			{
				foreach (var doc in date)
				{
					doc.RollbackBalanceChanges(balance);
				}
				documentsRolledBack.Push(date.ToList());
			}

			// занесение нового документа в список на применение
			ICollection<Document> lastDate;
			if (documentsRolledBack.Count == 0 || (lastDate = documentsRolledBack.Peek()).First().Date.Date > document.Date.Date)
			{
				documentsRolledBack.Push(lastDate = new List<Document>());
			}
			lastDate.Add(document);

			// обратный накат документов
			while (documentsRolledBack.Any())
			{
				var date = documentsRolledBack.Pop();
				productsToCheck = new List<long>();
				foreach (var doc in date)
				{
					productsToCheck.AddRange(doc.ApplyBalanceChanges(balance).Keys);
				}
				if (!balance.Check(database.Products, errors, productsToCheck.Distinct().ToList()))
				{
					return false;
				}
			}

			return true;
		}

		public override bool VerifyEdit(IDatabase database, Document document, StringBuilder errors)
		{
			List<long> productsToCheck;
			var original = database.Documents[document.PreviousVersionId.Value];

			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// откат документов до самой маленькой даты
			var firstDate = document.Date.Date > original.Date.Date ? original.Date.Date : document.Date.Date;
			var documentsRolledBack = new Stack<ICollection<Document>>();
			foreach (var date in database.GetActiveDocuments().Where(doc => doc.Date.Date >= firstDate).GroupBy(doc => doc.Date.Date))
			{
				foreach (var doc in date)
				{
					doc.RollbackBalanceChanges(balance);
				}
				var dateList = date.Where(doc => doc != original).ToList();
				if (date.Key == document.Date.Date)
				{
					dateList.Add(document);
				}
				documentsRolledBack.Push(dateList);
			}

			// обратный накат документов
			bool newVersionApplied = false;
			if (documentsRolledBack.Peek().First().Date.Date > document.Date)
			{
				productsToCheck = document.ApplyBalanceChanges(balance).Keys.ToList();
				if (!balance.Check(database.Products, errors, productsToCheck))
				{
					return false;
				}
				else
				{
					newVersionApplied = true;
				}
			}

			while (documentsRolledBack.Any())
			{
				var date = documentsRolledBack.Pop();
				productsToCheck = new List<long>();
				foreach (var doc in date)
				{
					productsToCheck.AddRange(doc.ApplyBalanceChanges(balance).Keys);
					if (doc == document)
					{
						newVersionApplied = true;
					}
				}
				if (!balance.Check(database.Products, errors, productsToCheck.Distinct().ToList()))
				{
					return false;
				}
			}

			if (!newVersionApplied)
			{
				productsToCheck = document.ApplyBalanceChanges(balance).Keys.ToList();
				if (!balance.Check(database.Products, errors, productsToCheck))
				{
					return false;
				}
			}

			return true;
		}

		public override bool VerifyDelete(IDatabase database, IReadOnlyCollection<Document> documents, StringBuilder errors)
		{
			List<long> productsToCheck;

			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// откат предыдущих документов
			var minDate = documents.Min(doc => doc.Date.Date);
			var documentsToDelete = new HashSet<Document>(documents);
			var documentsRolledBack = new Stack<ICollection<Document>>();
			foreach (var date in database.GetActiveDocuments().Where(d => d.Date.Date >= minDate).GroupBy(d => d.Date.Date))
			{
				foreach (var doc in date)
				{
					doc.RollbackBalanceChanges(balance);
				}
				documentsRolledBack.Push(date.Where(doc => !documentsToDelete.Contains(doc)).ToList());
			}

			// обратный накат документов
			while (documentsRolledBack.Any())
			{
				var date = documentsRolledBack.Pop();
				productsToCheck = new List<long>();
				foreach (var doc in date)
				{
					productsToCheck.AddRange(doc.ApplyBalanceChanges(balance).Keys);
				}
				if (!balance.Check(database.Products, errors, productsToCheck.Distinct().ToList()))
				{
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}

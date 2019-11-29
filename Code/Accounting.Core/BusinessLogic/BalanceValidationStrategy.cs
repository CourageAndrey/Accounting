using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class BalanceValidationStrategy
	{
		public delegate bool BalanceTryChangeDelegate(Database database, Document document, StringBuilder errors);
		private delegate bool BalanceTryChangeManyDelegate(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors);

		private readonly BalanceTryChangeDelegate _createHandler;
		private readonly BalanceTryChangeDelegate _editHandler;
		private readonly BalanceTryChangeManyDelegate _deleteHandler;

		private BalanceValidationStrategy(BalanceTryChangeDelegate createHandler, BalanceTryChangeDelegate editHandler, BalanceTryChangeManyDelegate deleteHandler)
		{
			_createHandler = createHandler;
			_editHandler = editHandler;
			_deleteHandler = deleteHandler;
		}

		#region List

		public static readonly BalanceValidationStrategy PerDocument = new BalanceValidationStrategy(
			createPerDocument,
			editPerDocument,
			deletePerDocument);

		public static readonly BalanceValidationStrategy PerDay = new BalanceValidationStrategy(
			createPerDay,
			editPerDay,
			deletePerDay);

		public static readonly BalanceValidationStrategy FinalOnly = new BalanceValidationStrategy(
			createFinalOnly,
			editFinalOnly,
			deleteFinalOnly);

		public static readonly BalanceValidationStrategy NoVerify = new BalanceValidationStrategy(
			createNoVerify,
			editNoVerify,
			deleteNoVerify);

		internal static readonly IReadOnlyDictionary<string, BalanceValidationStrategy> All = new ReadOnlyDictionary<string, BalanceValidationStrategy>(
			new Dictionary<string, BalanceValidationStrategy>
			{
				{ "PerDocument", PerDocument },
				{ "PerDay", PerDay },
				{ "FinalOnly", FinalOnly },
				{ "NoVerify", NoVerify },
			});

		#endregion

		#region Handlers

		private static bool createPerDocument(Database database, Document document, StringBuilder errors)
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

		private static bool editPerDocument(Database database, Document document, StringBuilder errors)
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

		private static bool deletePerDocument(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors)
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

		private static bool createPerDay(Database database, Document document, StringBuilder errors)
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

		private static bool editPerDay(Database database, Document document, StringBuilder errors)
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

		private static bool deletePerDay(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors)
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

		private static bool createFinalOnly(Database database, Document document, StringBuilder errors)
		{
			// создание временной копии таблицы баланса
			var balance = database.Balance.Clone();

			// применение нового документа
			var productsToCheck = document.ApplyBalanceChanges(balance).Keys;

			// проверка итогового баланса
			return balance.Check(database.Products, errors, productsToCheck);
		}

		private static bool editFinalOnly(Database database, Document document, StringBuilder errors)
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

		private static bool deleteFinalOnly(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors)
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

		private static bool createNoVerify(Database database, Document document, StringBuilder errors)
		{
			return true;
		}

		private static bool editNoVerify(Database database, Document document, StringBuilder errors)
		{
			return true;
		}

		private static bool deleteNoVerify(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors)
		{
			return true;
		}

		#endregion

		public bool VerifyCreate(Database database, Document document, StringBuilder errors)
		{
			return _createHandler(database, document, errors);
		}

		public bool VerifyEdit(Database database, Document document, StringBuilder errors)
		{
			return _editHandler(database, document, errors);
		}

		public bool VerifyDelete(Database database, IReadOnlyCollection<Document> documents, StringBuilder errors)
		{
			return _deleteHandler(database, documents, errors);
		}
	}
}

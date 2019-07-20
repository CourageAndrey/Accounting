using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.UnitTests.BalanceValidationStrategies
{
	internal static class BalanceCheckWorkflowHelper
	{
		public static Database CreateComplexDatabase()
		{
			var unit = new Unit
			{
				ID = 1,
				Name = "Full Name",
				ShortName = "short",
			};
			var product = new Product
			{
				ID = 1,
				Name = "1",
				Unit = unit,
			};
			Document document;

			var database = new Database(
				new[] { unit },
				new[] { product },
				new Dictionary<long, decimal> { { product.ID, 10 } },
				new Document[0]);
			var firstDate = DateTime.Now.Date.AddDays(-2);

			database.Documents.Add(document = new Document(DocumentType.Income)
			{
				Date = firstDate.AddHours(6),
				Number = "income +5",
				Positions = new Dictionary<Product, decimal> { { product, 5 } },
			});
			document.Apply(database);

			database.Documents.Add(document = new Document(DocumentType.Outcome)
			{
				Date = firstDate.AddHours(12),
				Number = "outcome -12",
				Positions = new Dictionary<Product, decimal> { { product, 12 } },
			});
			document.Apply(database);

			database.Documents.Add(document = new Document(DocumentType.Income)
			{
				Date = firstDate.AddDays(1).AddHours(6),
				Number = "income +8",
				Positions = new Dictionary<Product, decimal> { { product, 8 } },
			});
			document.Apply(database);

			database.Documents.Add(document = new Document(DocumentType.Outcome)
			{
				Date = firstDate.AddDays(1).AddHours(12),
				Number = "outcome -10",
				Positions = new Dictionary<Product, decimal> { { product, 10 } },
			});
			document.Apply(database);

			Assert.AreEqual(1, database.Balance.Single().Value);

			return database;
		}

		#region Test cases

		public delegate bool AddChecker(Database database, decimal delta, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document document);
		public delegate bool EditChecker(Database database, Func<Document, Tuple<decimal, int>> deltaGetter, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document edited);

		public static readonly IReadOnlyCollection<AddChecker> AddCheckers = new AddChecker[]
		{
			TryToAddBeforeAll,
			TryToAddAfterFirst,
			TryToAddAfterSecond,
			TryToAddAfterThird,
			TryToAddAfterAll,
		};

		public static readonly IReadOnlyCollection<EditChecker> EditCheckers = new EditChecker[]
		{
			TryToEditFirst,
			TryToEditSecond,
			TryToEditThird,
			TryToEditFourth,
		};

		public static bool TryToAddBeforeAll(Database database, decimal delta, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document document)
		{
			document = new Document(delta > 0 ? DocumentType.Income : DocumentType.Outcome)
			{
				Number = "TEST",
				Date = database.Documents.Skip(0).First().Date.AddDays(-1),
				Positions = new Dictionary<Product, decimal> { { database.Products.First(), Math.Abs(delta) } },
			};
			return validationStrategy.VerifyCreate(database, document, errors);
		}

		public static bool TryToAddAfterFirst(Database database, decimal delta, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document document)
		{
			document = new Document(delta > 0 ? DocumentType.Income : DocumentType.Outcome)
			{
				Number = "TEST",
				Date = database.Documents.Skip(0).First().Date.AddHours(3),
				Positions = new Dictionary<Product, decimal> { { database.Products.First(), Math.Abs(delta) } },
			};
			return validationStrategy.VerifyCreate(database, document, errors);
		}

		public static bool TryToAddAfterSecond(Database database, decimal delta, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document document)
		{
			document = new Document(delta > 0 ? DocumentType.Income : DocumentType.Outcome)
			{
				Number = "TEST",
				Date = database.Documents.Skip(1).First().Date.AddHours(3),
				Positions = new Dictionary<Product, decimal> { { database.Products.First(), Math.Abs(delta) } },
			};
			return validationStrategy.VerifyCreate(database, document, errors);
		}

		public static bool TryToAddAfterThird(Database database, decimal delta, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document document)
		{
			document = new Document(delta > 0 ? DocumentType.Income : DocumentType.Outcome)
			{
				Number = "TEST",
				Date = database.Documents.Skip(2).First().Date.AddHours(3),
				Positions = new Dictionary<Product, decimal> { { database.Products.First(), Math.Abs(delta) } },
			};
			return validationStrategy.VerifyCreate(database, document, errors);
		}

		public static bool TryToAddAfterAll(Database database, decimal delta, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document document)
		{
			document = new Document(delta > 0 ? DocumentType.Income : DocumentType.Outcome)
			{
				Number = "TEST",
				Date = database.Documents.Last().Date.AddDays(1),
				Positions = new Dictionary<Product, decimal> { { database.Products.First(), Math.Abs(delta) } },
			};
			return validationStrategy.VerifyCreate(database, document, errors);
		}

		public static bool TryToEditFirst(Database database, Func<Document, Tuple<decimal, int>> deltaGetter, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document edited)
		{
			var original = database.Documents.Skip(0).First();
			var deltas = deltaGetter(original);
			decimal delta = deltas.Item1;
			int hoursDelta = deltas.Item2;
			edited = new Document(original.ID, original.Type, DocumentState.Active)
			{
				Number = "EDIT " + original.Number,
				Date = original.Date.AddHours(hoursDelta),
				Positions = new Dictionary<Product, decimal>
				{
					{
						database.Products.First(),
						original.Positions.First().Value + delta
					}
				},
			};
			return validationStrategy.VerifyEdit(database, edited, errors);
		}

		public static bool TryToEditSecond(Database database, Func<Document, Tuple<decimal, int>> deltaGetter, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document edited)
		{
			var original = database.Documents.Skip(1).First();
			var deltas = deltaGetter(original);
			decimal delta = deltas.Item1;
			int hoursDelta = deltas.Item2;
			edited = new Document(original.ID, original.Type, DocumentState.Active)
			{
				Number = "EDIT " + original.Number,
				Date = original.Date.AddHours(hoursDelta),
				Positions = new Dictionary<Product, decimal>
				{
					{
						database.Products.First(),
						Math.Abs(original.Positions.First().Value + delta)
					}
				},
			};
			return validationStrategy.VerifyEdit(database, edited, errors);
		}

		public static bool TryToEditThird(Database database, Func<Document, Tuple<decimal, int>> deltaGetter, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document edited)
		{
			var original = database.Documents.Skip(2).First();
			var deltas = deltaGetter(original);
			decimal delta = deltas.Item1;
			int hoursDelta = deltas.Item2;
			edited = new Document(original.ID, original.Type, DocumentState.Active)
			{
				Number = "EDIT " + original.Number,
				Date = original.Date.AddHours(hoursDelta),
				Positions = new Dictionary<Product, decimal>
				{
					{
						database.Products.First(),
						Math.Abs(original.Positions.First().Value + delta)
					}
				},
			};
			return validationStrategy.VerifyEdit(database, edited, errors);
		}

		public static bool TryToEditFourth(Database database, Func<Document, Tuple<decimal, int>> deltaGetter, BalanceValidationStrategy validationStrategy, StringBuilder errors, out Document edited)
		{
			var original = database.Documents.Skip(3).First();
			var deltas = deltaGetter(original);
			decimal delta = deltas.Item1;
			int hoursDelta = deltas.Item2;
			edited = new Document(original.ID, original.Type, DocumentState.Active)
			{
				Number = "EDIT " + original.Number,
				Date = original.Date.AddHours(hoursDelta),
				Positions = new Dictionary<Product, decimal>
				{
					{
						database.Products.First(),
						Math.Abs(original.Positions.First().Value + delta)
					}
				},
			};
			return validationStrategy.VerifyEdit(database, edited, errors);
		}

		public static bool TryToDelete(Database database, IReadOnlyCollection<Document> documents, BalanceValidationStrategy validationStrategy, StringBuilder errors)
		{
			return validationStrategy.VerifyDelete(database, documents, errors);
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

using Accounting.Core.Application;
using Accounting.Core.BusinessLogic;
using Accounting.DAL.EntityFramework.Entities;

namespace Accounting.DAL.EntityFramework
{
	public class DatabaseDriver : IDatabaseDriver
	{
		private readonly string _connectionString;

		internal DatabaseDriver(string connectionString)
		{
			_connectionString = connectionString;
		}

		public bool CanLoad
		{
			get
			{
				try
				{
					using (var database = new AccountingEntities())
					{
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		public IDatabase Load()
		{
			using (var database = new AccountingEntities())
			{
				var units = database.Units.ToDictionary(
					u => u.ID,
					u => new Core.BusinessLogic.Unit
					{
						ID = u.ID,
						Name = u.Name,
						ShortName = u.ShortName,
					});

				var products = database.Products.ToDictionary(
					p => p.ID,
					p => new Core.BusinessLogic.Product
					{
						ID = p.ID,
						Name = p.Name,
						Unit = units[p.UnitID],
					});
				foreach (var part in database.IsPartOfs)
				{
					products[part.ParentID].Children.Add(products[part.ChildID], part.Count);
				}

				var balance = database.Balances.ToDictionary(
					b => b.ProductID,
					b => b.Count);

				var documentTypes = new Dictionary<short, DocumentType>
				{
					{ 0, DocumentType.Income },
					{ 1, DocumentType.Outcome },
					{ 2, DocumentType.Produce },
					{ 3, DocumentType.ToWarehouse },
				};
				var documentStates = new Dictionary<short, DocumentState>
				{
					{ 0, DocumentState.Active },
					{ 1, DocumentState.Edited },
					{ 2, DocumentState.Deleted },
				};
				var documents = database.Documents.ToDictionary(
					d => d.ID,
					d =>
					{
						var document = new Core.BusinessLogic.Document(
							d.PreviousVersionID,
							documentTypes[d.TypeID],
							documentStates[d.StateID])
						{
							ID = d.ID,
							Number = d.Number,
							Date = d.Date,
						};
						foreach (var position in d.Positions)
						{
							document.Positions[products[position.ProductID]] = position.Count;
						}
						return document;
					});


				return new InMemoryDatabase(
					units.Values,
					products.Values,
					balance,
					documents.Values);
			}
		}

		public void Save(IDatabase database)
		{
			throw new NotImplementedException();
		}
	}
}

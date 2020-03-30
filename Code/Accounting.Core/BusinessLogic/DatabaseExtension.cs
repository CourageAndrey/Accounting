using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.Core.BusinessLogic
{
	public static class DatabaseExtension
	{
		public static IEnumerable<Document> GetActiveDocuments(this IDatabase database)
		{
			return database.Documents.Where(d => d.State == DocumentState.Active).OrderByDescending(d => d.Date);
		}

		public static IRegistry<EntityT> GetRegistry<EntityT>(this IDatabase database)
			where EntityT : IEntity
		{
			return (IRegistry<EntityT>) GetRegistry(database, typeof(EntityT));
		}

		public static IRegistry GetRegistry(this IDatabase database, Type entityType)
		{
			if (typeof(Unit).IsAssignableFrom(entityType))
			{
				return database.Units;
			}
			else if (typeof(Product).IsAssignableFrom(entityType))
			{
				return database.Products;
			}
			else if (typeof(Document).IsAssignableFrom(entityType))
			{
				return database.Documents;
			}
			else
			{
				throw new NotSupportedException("Unsupported entity type " + entityType.FullName);
			}
		}
	}
}

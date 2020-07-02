using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Accounting.Core.BusinessLogic;
using Accounting.DAL.EntityFramework.Entities;

namespace Accounting.DAL.EntityFramework
{
	internal class EntitiesRegistry<T, EntityT> : IRegistry<T>
		where T : IEntity
		where EntityT : class, IEntity
	{
		private readonly Func<AccountingEntities> _createContext;
		private readonly Func<AccountingEntities, DbSet<EntityT>> _getTable;
		private readonly Func<AccountingEntities, EntityT, T> _convertFromEntity;
		private readonly Func<AccountingEntities, T, EntityT> _convertToEntity;
		private readonly IDictionary<long, T> _cache;

		#region Public interface

		public int Count
		{
			get
			{
				using (var database = _createContext())
				{
					var table = _getTable(database);
					return table.Count();
				}
			}
		}

		public long Add(T item)
		{
			_cache[item.ID] = item;
			using (var database = _createContext())
			{
				var table = _getTable(database);
				var entity = _convertToEntity(database, item);
				table.Add(entity);
				return WHAT;
			}
		}

		public bool Remove(long id)
		{
			_cache.Remove(id);
			using (var database = _createContext())
			{
				var table = _getTable(database);
				return table.Remove(WHAT);
			}
		}

		public T this[long id]
		{
			get
			{
				T instance;
				if (!_cache.TryGetValue(id, out instance))
				{
					using (var database = _createContext())
					{
						var table = _getTable(database);
						_cache[id] = instance = _convertFromEntity(database, table.First(entity => entity.ID == id));
					}
				}
				return instance;
			}
		}

		#endregion

		#region Implementation of IRegistry

		long IRegistry.Add(object item)
		{
			return Add((T)item);
		}

		object IRegistry.this[long id]
		{ get { return this[id]; } }

		#endregion

		#region Implementation of IEnumerable<T>

		public IEnumerator<T> GetEnumerator()
		{
			using (var database = _createContext())
			{
				foreach (var entity in _getTable(database))
				{
					T instance;
					if (!_cache.TryGetValue(entity.ID, out instance))
					{
						_cache[entity.ID] = instance = _convertFromEntity(database, entity);
					}
					yield return instance;
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public EntitiesRegistry(
			Func<AccountingEntities> createContext,
			Func<AccountingEntities, DbSet<EntityT>> getTable,
			Func<AccountingEntities, EntityT, T> convertFromEntity,
			Func<AccountingEntities, T, EntityT> convertToEntity)
		{
			_createContext = createContext;
			_getTable = getTable;
			_convertFromEntity = convertFromEntity;
			_convertToEntity = convertToEntity;

			_cache = new Dictionary<long, T>();
		}
	}
}

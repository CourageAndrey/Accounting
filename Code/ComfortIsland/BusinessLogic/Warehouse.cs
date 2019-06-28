using System.Collections.Generic;
using System.Linq;

using ComfortIsland.Helpers;

namespace ComfortIsland.BusinessLogic
{
	public class Warehouse<T> : IEnumerable<T>
		where T : IEntity
	{
		private readonly IDictionary<long, T> storage;

		#region Public interface

		public int Count
		{ get { return storage.Count; } }

		public long Add(T item)
		{
			storage[item.ID = IdHelper.GenerateNewId(storage.Values)] = item;
			return item.ID;
		}

		public bool Remove(long id)
		{
			return storage.Remove(id);
		}

		public T this[long id]
		{ get { return storage[id]; } }

		#endregion

		#region Implementation of IEnumerable<T>

		public IEnumerator<T> GetEnumerator()
		{
			return storage.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Constructors

		private Warehouse(IDictionary<long, T> items)
		{
			storage = items;
		}

		public Warehouse()
			: this(new Dictionary<long, T>())
		{ }

		public Warehouse(IEnumerable<T> items)
			: this(items.ToDictionary(item => item.ID, item => item))
		{ }

		#endregion
	}
}

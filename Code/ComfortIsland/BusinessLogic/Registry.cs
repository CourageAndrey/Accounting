using System.Collections.Generic;
using System.Linq;

using ComfortIsland.Helpers;

namespace ComfortIsland.BusinessLogic
{
	public class Registry<T> : IEnumerable<T>
		where T : IEntity
	{
		private readonly IDictionary<long, T> _storage;

		#region Public interface

		public int Count
		{ get { return _storage.Count; } }

		public long Add(T item)
		{
			_storage[item.ID = _storage.Values.NewId()] = item;
			return item.ID;
		}

		public bool Remove(long id)
		{
			return _storage.Remove(id);
		}

		public T this[long id]
		{ get { return _storage[id]; } }

		#endregion

		#region Implementation of IEnumerable<T>

		public IEnumerator<T> GetEnumerator()
		{
			return _storage.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Constructors

		private Registry(IDictionary<long, T> storage)
		{
			_storage = storage;
		}

		public Registry()
			: this(new Dictionary<long, T>())
		{ }

		public Registry(IEnumerable<T> items)
			: this(items.ToDictionary(item => item.ID, item => item))
		{ }

		#endregion
	}
}

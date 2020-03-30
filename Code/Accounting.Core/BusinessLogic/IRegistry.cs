using System.Collections.Generic;

namespace Accounting.Core.BusinessLogic
{
	public interface IRegistry : System.Collections.IEnumerable
	{
		int Count
		{ get; }

		long Add(object item);

		bool Remove(long id);

		object this[long id]
		{ get; }
	}

	public interface IRegistry<T> : IRegistry, IEnumerable<T>
	{
		long Add(T item);

		new T this[long id]
		{ get; }
	}
}
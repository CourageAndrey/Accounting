using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.Core.Helpers
{
	public static class DictionaryComparer
	{
		public static bool IsEqualTo<TKey, TValue>(this IDictionary<TKey, TValue> a, IDictionary<TKey, TValue> b)
		{
			if (a == null) throw new ArgumentNullException(nameof(a));
			if (b == null) throw new ArgumentNullException(nameof(b));

			return a.Count == b.Count && !a.Except(b).Any();
		}
	}
}

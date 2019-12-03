using System.Collections.Generic;
using System.Linq;

namespace Accounting.Core.Helpers
{
	public static class IdGenerator
	{
		public static long NewId<T>(this ICollection<T> list)
			where T: ComfortIsland.IEntity
		{
			return list.Count > 0
				? list.Max(entity => entity.ID) + 1
				: 1;
		}
	}
}

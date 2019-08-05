using System.Collections.Generic;
using System.Linq;

namespace ComfortIsland.Helpers
{
	public static class IdGenerator
	{
		public static long NewId<T>(this ICollection<T> list)
			where T: IEntity
		{
			return list.Count > 0
				? list.Max(entity => entity.ID) + 1
				: 1;
		}
	}
}

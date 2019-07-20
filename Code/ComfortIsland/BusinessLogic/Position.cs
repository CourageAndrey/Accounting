using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class Position
	{
		#region Properties

		public long ID
		{ get; set; }

		public decimal Count
		{ get; set; }

		public Product BoundProduct
		{ get; private set; }

		#endregion

		#region Constructors

		public Position()
		{ }

		public Position(long id, decimal count)
		{
			ID = id;
			Count = count;
		}

		public Position(Position other)
		{
			ID = other.ID;
			Count = other.Count;
		}

		#endregion

		internal void SetProduct(Database database)
		{
			BoundProduct = database.Products[ID];
		}

		#region Валидация

		public static bool ProductIsSet(long id, int lineNumber, StringBuilder errors)
		{
			if (id < 1)
			{
				errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Не задан продукт в {0} строке.", lineNumber));
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool CountIsPositive(decimal count, int lineNumber, StringBuilder errors)
		{
			if (count <= 0)
			{
				errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Заданное в {0} строке количество меньше ноля.", lineNumber));
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool PositionsDoNotDuplicate(IEnumerable<Position> positions, string whatToCheck, StringBuilder errors)
		{
			var ids = positions.Select(c => c.ID).ToList();
			if (ids.Count > ids.Distinct().Count())
			{
				errors.AppendLine(string.Format(CultureInfo.InvariantCulture, "Некоторые {0} включены несколько раз.", whatToCheck));
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion
	}
}

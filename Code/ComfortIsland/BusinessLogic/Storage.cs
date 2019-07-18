using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ComfortIsland.Helpers;

namespace ComfortIsland.BusinessLogic
{
	public class Storage : IEnumerable<KeyValuePair<long, double>>
	{
		private readonly IDictionary<long, double> data;

		#region Public interface

		public void Increase(long id, double value)
		{
			double count;
			if (!data.TryGetValue(id, out count))
			{
				count = 0;
			}
			count += value;
			data[id] = count;
		}

		public void Decrease(long id, double value)
		{
			double count;
			if (!data.TryGetValue(id, out count))
			{
				count = 0;
			}
			count -= value;
			data[id] = count;
		}

		public bool TryGetValue(long id, out double value)
		{
			return data.TryGetValue(id, out value);
		}

		public double this[long id]
		{
			get
			{
				double count;
				return data.TryGetValue(id, out count)
					? count
					: 0;
			}
		}

		public List<Position> ToPositions()
		{
			return data.Select(b => new Position(b.Key, b.Value)).ToList();
		}

		public Storage Clone()
		{
			return new Storage(new Dictionary<long, double>(data));
		}

		public bool Check(Warehouse<Product> products, StringBuilder errors, ICollection<long> productsFilter = null)
		{
			IEnumerable<KeyValuePair<long, double>> wrongPositions = data.Where(position => position.Value < 0);
			if (productsFilter != null)
			{
				wrongPositions = wrongPositions.Where(position => productsFilter.Contains(position.Key));
			}
			var wrongPositionsList = wrongPositions.ToList();

			if (wrongPositionsList.Count > 0)
			{
				errors.AppendLine("При выполнении данной операции остатки следующих товаров принимают отрицательные значения:");
				foreach (var position in wrongPositionsList)
				{
					var product = products[position.Key];
					errors.AppendLine(string.Format(" * {0} = {1}", product.DisplayMember, DigitRoundingConverter.Simplify(position.Value)));
				}
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		#region Constructors

		public Storage()
			: this(new Dictionary<long, double>())
		{ }

		public Storage(IDictionary<long, double> data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			this.data = data;
		}

		#endregion

		#region Enumeration

		public IEnumerator<KeyValuePair<long, double>> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ComfortIsland.Helpers;

namespace ComfortIsland.BusinessLogic
{
	public class Storage : IEnumerable<KeyValuePair<long, decimal>>
	{
		private readonly IDictionary<long, decimal> _data;

		#region Public interface

		public void Increase(long id, decimal value)
		{
			decimal count;
			if (!_data.TryGetValue(id, out count))
			{
				count = 0;
			}
			count += value;
			_data[id] = count;
		}

		public void Decrease(long id, decimal value)
		{
			decimal count;
			if (!_data.TryGetValue(id, out count))
			{
				count = 0;
			}
			count -= value;
			_data[id] = count;
		}

		public bool TryGetValue(long id, out decimal value)
		{
			return _data.TryGetValue(id, out value);
		}

		public decimal this[long id]
		{
			get
			{
				decimal count;
				return _data.TryGetValue(id, out count)
					? count
					: 0;
			}
		}

		public List<Position> ToPositions()
		{
			return _data.Select(b => new Position(b.Key, b.Value)).ToList();
		}

		public Storage Clone()
		{
			return new Storage(new Dictionary<long, decimal>(_data));
		}

		public bool Check(Warehouse<Product> products, StringBuilder errors, ICollection<long> productsFilter = null)
		{
			IEnumerable<KeyValuePair<long, decimal>> wrongPositions = _data.Where(position => position.Value < 0);
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
			: this(new Dictionary<long, decimal>())
		{ }

		public Storage(IDictionary<long, decimal> data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			_data = data;
		}

		#endregion

		#region Enumeration

		public IEnumerator<KeyValuePair<long, decimal>> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

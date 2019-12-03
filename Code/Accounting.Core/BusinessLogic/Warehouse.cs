using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Accounting.Core.Helpers;

namespace ComfortIsland.BusinessLogic
{
	public class Warehouse : IEnumerable<KeyValuePair<long, decimal>>
	{
		private readonly IDictionary<long, decimal> _data;

		#region Public interface

		public void Increase(long id, decimal value)
		{
			_data[id] = this[id] + value;
		}

		public void Decrease(long id, decimal value)
		{
			_data[id] = this[id] - value;
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

		public bool Check(Registry<Product> products, StringBuilder errors, ICollection<long> productsFilter = null)
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
					errors.AppendLine(string.Format(" * {0} = {1}", product.DisplayMember, position.Value.Simplify()));
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

		public Warehouse(IDictionary<long, decimal> data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			_data = data;
		}

		public Warehouse Clone()
		{
			return new Warehouse(new Dictionary<long, decimal>(_data));
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

using System.Collections.Generic;
using System.Text;

namespace Accounting.Core.BusinessLogic
{
	public interface IWarehouse : IEnumerable<KeyValuePair<long, decimal>>
	{
		void Increase(long id, decimal value);

		void Decrease(long id, decimal value);

		decimal this[long id] { get; }

		List<Position> ToPositions();

		bool Check(Registry<Product> products, StringBuilder errors, ICollection<long> productsFilter = null);

		IWarehouse Clone();
	}
}
using System.Collections.Generic;
using System.Linq;

namespace ComfortIsland.BusinessLogic
{
	public class DocumentType
	{
		private delegate IDictionary<long, decimal> GetBalanceDeltaDelegate(IDictionary<Product, decimal> positions);

		#region Properties

		public Xml.DocumentType Enum
		{ get; }

		public string Name
		{ get; }

		private readonly GetBalanceDeltaDelegate _getBalanceDelta;

		#endregion

		private DocumentType(Xml.DocumentType enumValue, string name, GetBalanceDeltaDelegate getBalanceDelta)
		{
			Enum = enumValue;
			Name = name;
			_getBalanceDelta = getBalanceDelta;
		}

		public override string ToString()
		{
			return Name;
		}

		#region List

		public static readonly DocumentType Income = new DocumentType(Xml.DocumentType.Income, "приход", getBalanceDeltaIncome);
		public static readonly DocumentType Outcome = new DocumentType(Xml.DocumentType.Outcome, "продажа", getBalanceDeltaOutcome);
		public static readonly DocumentType Produce = new DocumentType(Xml.DocumentType.Produce, "производство", getBalanceDeltaProduce);
		public static readonly DocumentType ToWarehouse = new DocumentType(Xml.DocumentType.ToWarehouse, "перемещение на склад", getBalanceDeltaOutcome);

		public static readonly IDictionary<Xml.DocumentType, DocumentType> AllTypes = new[] { Income, Outcome, Produce, ToWarehouse }.ToDictionary(
			type => type.Enum,
			type => type);

		#endregion

		#region GetDelta-methods

		private static IDictionary<long, decimal> getBalanceDeltaIncome(IDictionary<Product, decimal> positions)
		{
			return positions.ToDictionary(p => p.Key.ID, p => p.Value);
		}

		private static IDictionary<long, decimal> getBalanceDeltaOutcome(IDictionary<Product, decimal> positions)
		{
			return positions.ToDictionary(p => p.Key.ID, p => -p.Value);
		}

		private static IDictionary<long, decimal> getBalanceDeltaProduce(IDictionary<Product, decimal> positions)
		{
			var result = new Dictionary<long, decimal>();
			foreach (var position in positions)
			{
				result[position.Key.ID] = position.Value;
				foreach (var child in position.Key.Children)
				{
					decimal count;
					if (result.TryGetValue(child.Key.ID, out count))
					{
						count -= (position.Value * child.Value);
					}
					else
					{
						count = -(position.Value * child.Value);
					}
					result[child.Key.ID] = count;
				}
			}
			return result;
		}

		#endregion

		public IDictionary<long, decimal> GetBalanceDelta(IDictionary<Product, decimal> positions)
		{
			return _getBalanceDelta(positions);
		}
	}
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ComfortIsland.Database
{
	public enum DocumentType
	{
		Income,
		Outcome,
		Produce,
		ToWarehouse,
	}

	public delegate IDictionary<long, double> GetBalanceDeltaDelegate(Database database, Document document);

	internal class DocumentTypeImplementation
	{
		#region Properties

		public DocumentType Type
		{ get; private set; }

		public string Name
		{ get; private set; }

		public GetBalanceDeltaDelegate GetBalanceDelta
		{ get; private set; }

		#endregion

		private DocumentTypeImplementation(DocumentType type, string name, GetBalanceDeltaDelegate getBalanceDelta)
		{
			Type = type;
			Name = name;
			GetBalanceDelta = getBalanceDelta;
		}

		#region List

		public static readonly DocumentTypeImplementation Income;

		public static readonly DocumentTypeImplementation Outcome;

		public static readonly DocumentTypeImplementation Produce;

		public static readonly DocumentTypeImplementation ToWarehouse;

		public static readonly IDictionary<DocumentType, DocumentTypeImplementation> AllTypes;

		static DocumentTypeImplementation()
		{
			Income = new DocumentTypeImplementation(DocumentType.Income, "приход", getBalanceDeltaIncome);
			Outcome = new DocumentTypeImplementation(DocumentType.Outcome, "продажа", getBalanceDeltaOutcome);
			Produce = new DocumentTypeImplementation(DocumentType.Produce, "производство", getBalanceDeltaProduce);
			ToWarehouse = new DocumentTypeImplementation(DocumentType.ToWarehouse, "перемещение на склад", getBalanceDeltaOutcome);
			AllTypes = new ReadOnlyDictionary<DocumentType, DocumentTypeImplementation>(new Dictionary<DocumentType, DocumentTypeImplementation>
			{
				{ DocumentType.Income, Income },
				{ DocumentType.Outcome, Outcome },
				{ DocumentType.Produce, Produce },
				{ DocumentType.ToWarehouse, ToWarehouse },
			});
		}

		#endregion

		#region GetDelta-methods

		private static IDictionary<long, double> getBalanceDeltaIncome(Database database, Document document)
		{
			return document.PositionsToSerialize.ToDictionary(p => p.ID, p => p.Count);
		}

		private static IDictionary<long, double> getBalanceDeltaOutcome(Database database, Document document)
		{
			return document.PositionsToSerialize.ToDictionary(p => p.ID, p => -p.Count);
		}

		private static IDictionary<long, double> getBalanceDeltaProduce(Database database, Document document)
		{
			var result = new Dictionary<long, double>();
			foreach (var position in document.PositionsToSerialize)
			{
				result[position.ID] = position.Count;
				foreach (var child in database.Products.First(p => p.ID == position.ID).Children)
				{
					double count;
					if (result.TryGetValue(child.Key.ID, out count))
					{
						count -= (position.Count * child.Value);
					}
					else
					{
						count = -(position.Count * child.Value);
					}
					result[child.Key.ID] = count;
				}
			}
			return result;
		}

		#endregion
	}
}

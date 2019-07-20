using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class Document : IEntity
	{
		#region Properties

		public long ID
		{ get; set; }

		public long? PreviousVersionId
		{ get; }

		public string Number
		{ get; set; }

		public DateTime Date
		{ get; set; }

		public DocumentType Type
		{ get; }

		public DocumentState State
		{ get; private set; }

		public Dictionary<Product, decimal> Positions
		{
			get { return positions; }
			set
			{
				var errors = new StringBuilder();
				if (PositionsCountHasToBePositive(value, errors) &
					PositionCountsArePositive(value, errors) &
					PositionDoNotDuplicate(value, errors))
				{
					positions = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		private Dictionary<Product, decimal> positions;

		#endregion

		#region Constructors

		public Document(long? previousVersionId, DocumentType type, DocumentState state)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (state == null) throw new ArgumentNullException(nameof(state));

			PreviousVersionId = previousVersionId;
			Type = type;
			State = state;

			positions = new Dictionary<Product, decimal>();
		}

		public Document(DocumentType type)
			: this(null, type, DocumentState.Active)
		{ }

		#endregion

		#region Validation

		public static bool PositionsCountHasToBePositive(Dictionary<Product, decimal> children, StringBuilder errors)
		{
			if (children.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool PositionCountsArePositive(Dictionary<Product, decimal> children, StringBuilder errors)
		{
			if (children.Any(c => c.Value <= 0))
			{
				errors.AppendLine("Количество товара во всех позициях должно быть строго больше ноля.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool PositionDoNotDuplicate(Dictionary<Product, decimal> children, StringBuilder errors)
		{
			var ids = children.Select(c => c.Key.ID).ToList();
			if (ids.Count > ids.Distinct().Count())
			{
				errors.AppendLine("Некоторые товары включены несколько раз.");
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		#region Workflow

		public IDictionary<long, decimal> Apply(Database database)
		{
			var delta = Type.GetBalanceDelta(Positions);
			foreach (var position in delta)
			{
				database.Balance.Increase(position.Key, position.Value);
			}
			return delta;
		}

		public IDictionary<long, decimal> Rollback(Database database)
		{
			var delta = Type.GetBalanceDelta(Positions);
			foreach (var position in delta)
			{
				database.Balance.Decrease(position.Key, position.Value);
			}
			return delta;
		}

		public void MakeEdited(Database database)
		{
			if (State == DocumentState.Active)
			{
				State = DocumentState.Edited;
				Rollback(database);
			}
		}

		#endregion

		public StringBuilder FindUsages(Database database)
		{
			return new StringBuilder();
		}
	}
}

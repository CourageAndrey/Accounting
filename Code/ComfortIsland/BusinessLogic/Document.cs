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
			get { return _positions; }
			set
			{
				var errors = new StringBuilder();
				var positionsToCheck = value.Select(kvp => new Position(kvp.Key.ID, kvp.Value)).ToList();
				bool isValid = Position.PositionsDoNotDuplicate(positionsToCheck, "товарные позиции", errors);
				for (int line = 0; line < positionsToCheck.Count; line++)
				{
					isValid &= Position.ProductIsSet(positionsToCheck[line].ID, line + 1, errors);
					isValid &= Position.CountIsPositive(positionsToCheck[line].Count, line + 1, errors);
				}
				if (isValid & PositionsCountHasToBePositive(value, errors))
				{
					_positions = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		private Dictionary<Product, decimal> _positions;

		#endregion

		#region Constructors

		public Document(long? previousVersionId, DocumentType type, DocumentState state)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (state == null) throw new ArgumentNullException(nameof(state));

			PreviousVersionId = previousVersionId;
			Type = type;
			State = state;

			_positions = new Dictionary<Product, decimal>();
		}

		public Document(DocumentType type)
			: this(null, type, DocumentState.Active)
		{ }

		#endregion

		#region Validation

		public static bool PositionsCountHasToBePositive(System.Collections.ICollection positions, StringBuilder errors)
		{
			if (positions.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
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

		public void Edit(Database database)
		{
			if (State == DocumentState.Active)
			{
				State = DocumentState.Edited;
				Rollback(database);
			}
		}

		public void Delete(Database database)
		{
			if (State == DocumentState.Active)
			{
				State = DocumentState.Deleted;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComfortIsland.ViewModels
{
	public class Document : NotifyDataErrorInfo, IViewModel<BusinessLogic.Document>
	{
		#region Properties

		public BusinessLogic.DocumentType Type
		{ get; }

		public string Number
		{ get; set; }

		public DateTime Date
		{ get; set; }

		public List<BusinessLogic.Position> Positions
		{
			get { return positions; }
			set
			{
				positions = value;
				var errors = new StringBuilder();
#warning Dictionary
				/*var valueDictionary = value.ToDictionary(, );
				if (BusinessLogic.Document.PositionsCountHasToBePositive(value, errors) &
					BusinessLogic.Document.PositionCountsArePositive(value, errors) &
					BusinessLogic.Document.PositionDoNotDuplicate(value, errors))
				{
					ClearErrors();
				}
				else
				{
					AddError(errors.ToString());
				}*/
			}
		}

		private readonly long? id;
		private List<BusinessLogic.Position> positions;

		#endregion

		#region Constructors

		private Document(long? id, BusinessLogic.DocumentType type, string number, DateTime date, List<BusinessLogic.Position> positions)
		{
			this.id = id;
			Type = type;
			Number = number;
			Date = date;
			Positions = positions;
		}

		public Document(BusinessLogic.DocumentType type)
			: this(null, type, string.Empty, DateTime.Now, new List<BusinessLogic.Position>())
		{ }

		public Document(BusinessLogic.Document instance)
			: this(
				instance.ID,
				instance.Type,
				instance.Number,
				instance.Date,
				instance.Positions.Select(child => new BusinessLogic.Position(child.Key.ID, child.Value)).ToList())
		{ }

		#endregion

		public BusinessLogic.Document ConvertToBusinessLogic(BusinessLogic.Database database)
		{
			BusinessLogic.Document instance;
			database.Documents.Add(instance = new BusinessLogic.Document
			{
				Type = Type,
				State = BusinessLogic.DocumentState.Active,
				PreviousVersionId = id,
			});
			instance.Number = Number;
			instance.Date = Date;
			instance.Positions = Positions.ToDictionary(
				position => database.Products[position.ID],
				position => position.Count);
			if (id.HasValue)
			{
				var previousVersion = database.Documents[id.Value];
				previousVersion.Rollback(database);
				if (previousVersion.State == BusinessLogic.DocumentState.Active)
				{
					previousVersion.State = BusinessLogic.DocumentState.Edited;
				}
			}
			instance.Apply(database);
			return instance;
		}
	}
}

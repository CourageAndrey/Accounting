using System;
using System.Collections.Generic;
using System.Linq;

namespace ComfortIsland.ViewModels
{
	public class Document : NotifyDataErrorInfo, IViewModel<BusinessLogic.Document>
	{
		#region Properties

		public long? ID
		{ get; }

		public BusinessLogic.DocumentType Type
		{ get; }

		public string Number
		{ get; set; }

		public DateTime Date
		{ get; set; }

		public List<BusinessLogic.Position> Positions
		{ get; }

		#endregion

		#region Constructors

		private Document(long? id, BusinessLogic.DocumentType type, string number, DateTime date, List<BusinessLogic.Position> positions)
		{
			ID = id;
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
			if (ID.HasValue)
			{
				var previousVersion = database.Documents[ID.Value];
				instance = new BusinessLogic.Document(previousVersion.ID, previousVersion.Type, BusinessLogic.DocumentState.Active);
				previousVersion.Edit(database);
			}
			else
			{
				instance = new BusinessLogic.Document(Type);
			}
			database.Documents.Add(instance);
			ApplyChanges(instance, database.Products);
			instance.Apply(database);
			return instance;
		}

		internal void ApplyChanges(BusinessLogic.Document document, BusinessLogic.Registry<BusinessLogic.Product> products)
		{
			document.Number = Number;
			document.Date = Date;
			document.Positions = Positions.ToDictionary(
				position => products[position.ID],
				position => position.Count);
		}
	}
}

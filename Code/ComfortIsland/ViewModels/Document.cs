using System;
using System.Collections.Generic;
using System.Linq;

using ComfortIsland.Helpers;

namespace ComfortIsland.ViewModels
{
	public class Document : IViewModel<BusinessLogic.Document>
	{
		#region Properties

		private readonly long? id;
		private readonly BusinessLogic.DocumentType type;

		public string Number
		{ get; set; }

		public DateTime Date
		{ get; set; }

		public string TypeName
		{ get { return type.Name; } }

		public List<BusinessLogic.Position> Positions
		{ get; set; }

		#endregion

		public Document(BusinessLogic.DocumentType type)
		{
			this.type = type;
			Positions = new List<BusinessLogic.Position>();
		}

		public Document(BusinessLogic.Document instance)
		{
			id = instance.ID;
			Number = instance.Number;
			Date = instance.Date;
			type = instance.Type;
			Positions = instance.Positions.Select(child => new BusinessLogic.Position(child.Key.ID, child.Value)).ToList();
		}

		public BusinessLogic.Document ConvertToBusinessLogic(BusinessLogic.Database database)
		{
			BusinessLogic.Document instance;
			database.Documents.Add(instance = new BusinessLogic.Document
			{
				Type = type,
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

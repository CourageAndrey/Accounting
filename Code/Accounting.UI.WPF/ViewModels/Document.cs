using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.UI.WPF.ViewModels
{
	public class Document : ViewModelBase<Accounting.Core.BusinessLogic.Document>
	{
		#region Properties

		public Accounting.Core.BusinessLogic.DocumentType Type
		{ get; }

		public string Number
		{ get; set; }

		public DateTime Date
		{ get; set; }

		public decimal Summ
		{ get; set; }

		public List<Accounting.Core.BusinessLogic.Position> Positions
		{ get; }

		#endregion

		#region Constructors

		private Document(long? id, Accounting.Core.BusinessLogic.DocumentType type, string number, DateTime date, decimal summ, List<Accounting.Core.BusinessLogic.Position> positions)
		{
			ID = id;
			Type = type;
			Number = number;
			Date = date;
			Summ = summ;
			Positions = positions;
		}

		public Document(Accounting.Core.BusinessLogic.DocumentType type)
			: this(null, type, string.Empty, DateTime.Now, 0, new List<Accounting.Core.BusinessLogic.Position>())
		{ }

		public Document(Accounting.Core.BusinessLogic.Document instance)
			: this(
				instance.ID,
				instance.Type,
				instance.Number,
				instance.Date,
				instance.Summ,
				instance.Positions.Select(child => new Accounting.Core.BusinessLogic.Position(child.Key.ID, child.Value)).ToList())
		{ }

		#endregion

		public override Accounting.Core.BusinessLogic.Document ConvertToBusinessLogic(Accounting.Core.BusinessLogic.IDatabase database)
		{
			Accounting.Core.BusinessLogic.Document entity;
			if (ID.HasValue)
			{
				var previousVersion = database.Documents[ID.Value];
				entity = new Accounting.Core.BusinessLogic.Document(previousVersion.ID, previousVersion.Type, Accounting.Core.BusinessLogic.DocumentState.Active);
				previousVersion.MakeObsolete(database.Balance, Accounting.Core.BusinessLogic.DocumentState.Edited);
			}
			else
			{
				entity = CreateNewEntity();
			}
			database.Documents.Add(entity);
			UpdateProperties(entity, database);
			entity.ApplyBalanceChanges(database.Balance);
			return entity;
		}

		public override Accounting.Core.BusinessLogic.Document CreateNewEntity()
		{
			return new Accounting.Core.BusinessLogic.Document(Type);
		}

		public override void UpdateProperties(Accounting.Core.BusinessLogic.Document entity, Accounting.Core.BusinessLogic.IDatabase database)
		{
			entity.Number = Number;
			entity.Date = Date;
			entity.Summ = Summ;
			entity.Positions = Positions.ToDictionary(
				position => database.Products[position.ID],
				position => position.Count);
		}
	}
}

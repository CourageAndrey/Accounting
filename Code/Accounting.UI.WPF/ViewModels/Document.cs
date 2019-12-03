using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.UI.WPF.ViewModels
{
	public class Document : NotifyDataErrorInfo, IViewModel<Accounting.Core.BusinessLogic.Document>
	{
		#region Properties

		public long? ID
		{ get; }

		public Accounting.Core.BusinessLogic.DocumentType Type
		{ get; }

		public string Number
		{ get; set; }

		public DateTime Date
		{ get; set; }

		public List<Accounting.Core.BusinessLogic.Position> Positions
		{ get; }

		#endregion

		#region Constructors

		private Document(long? id, Accounting.Core.BusinessLogic.DocumentType type, string number, DateTime date, List<Accounting.Core.BusinessLogic.Position> positions)
		{
			ID = id;
			Type = type;
			Number = number;
			Date = date;
			Positions = positions;
		}

		public Document(Accounting.Core.BusinessLogic.DocumentType type)
			: this(null, type, string.Empty, DateTime.Now, new List<Accounting.Core.BusinessLogic.Position>())
		{ }

		public Document(Accounting.Core.BusinessLogic.Document instance)
			: this(
				instance.ID,
				instance.Type,
				instance.Number,
				instance.Date,
				instance.Positions.Select(child => new Accounting.Core.BusinessLogic.Position(child.Key.ID, child.Value)).ToList())
		{ }

		#endregion

		public Accounting.Core.BusinessLogic.Document ConvertToBusinessLogic(Accounting.Core.BusinessLogic.Database database)
		{
			Accounting.Core.BusinessLogic.Document instance;
			if (ID.HasValue)
			{
				var previousVersion = database.Documents[ID.Value];
				instance = new Accounting.Core.BusinessLogic.Document(previousVersion.ID, previousVersion.Type, Accounting.Core.BusinessLogic.DocumentState.Active);
				previousVersion.MakeObsolete(database.Balance, Accounting.Core.BusinessLogic.DocumentState.Edited);
			}
			else
			{
				instance = new Accounting.Core.BusinessLogic.Document(Type);
			}
			database.Documents.Add(instance);
			ApplyChanges(instance, database.Products);
			instance.ApplyBalanceChanges(database.Balance);
			return instance;
		}

		internal void ApplyChanges(Accounting.Core.BusinessLogic.Document document, Accounting.Core.BusinessLogic.Registry<Accounting.Core.BusinessLogic.Product> products)
		{
			document.Number = Number;
			document.Date = Date;
			document.Positions = Positions.ToDictionary(
				position => products[position.ID],
				position => position.Count);
		}
	}
}

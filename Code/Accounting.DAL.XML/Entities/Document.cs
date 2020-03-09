using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using Accounting.DAL.XML.Enumerations;

namespace Accounting.DAL.XML.Entities
{
	[XmlType]
	public class Document
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlAttribute]
		private string PreviousVersionId
		{ get; set; }

		[XmlAttribute]
		public string Number
		{ get; set; }

		[XmlAttribute]
		public DateTime Date
		{ get; set; }

		[XmlAttribute]
		public decimal Summ
		{ get; set; }

		[XmlAttribute]
		public DocumentType Type
		{ get; set; }

		[XmlAttribute]
		public DocumentState State
		{ get; set; }

		[XmlArray("Positions"), XmlArrayItem("Product")]
		public List<Position> Positions
		{ get; set; }

		#endregion

		#region Constructors

		public Document()
		{
			Positions = new List<Position>();
		}

		public Document(Accounting.Core.BusinessLogic.Document document)
		{
			ID = document.ID;
			PreviousVersionId = document.PreviousVersionId.ToString();
			Number = document.Number;
			Date = document.Date;
			Summ = document.Summ;
			Type = document.Type.ToEnum();
			State = document.State.ToEnum();
			Positions = document.Positions.Select(position => new Position(new Accounting.Core.BusinessLogic.Position(position.Key.ID, position.Value))).ToList();
		}

		#endregion

		public Accounting.Core.BusinessLogic.Document ConvertToBusinessLogic()
		{
			return new Accounting.Core.BusinessLogic.Document(
				!string.IsNullOrEmpty(PreviousVersionId) ? (long?) long.Parse(PreviousVersionId) : null,
				Type.ToClass(),
				State.ToClass())
			{
				ID = ID,
				Number = Number,
				Date = Date,
				Summ = Summ,
			};
		}
	}
}

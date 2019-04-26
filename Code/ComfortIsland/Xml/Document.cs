using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ComfortIsland.Xml
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

		public Document(BusinessLogic.Document document)
		{
			ID = document.ID;
			PreviousVersionId = document.PreviousVersionId.ToString();
			Number = document.Number;
			Date = document.Date;
			Type = document.Type.Enum;
			State = document.State.Enum;
			Positions = document.PositionsToSerialize.Select(position => new Position(position)).ToList();
		}

		#endregion

		public BusinessLogic.Document ConvertToBusinessLogic()
		{
			return new BusinessLogic.Document
			{
				ID = ID,
				PreviousVersionId = !string.IsNullOrEmpty(PreviousVersionId) ? (long?) long.Parse(PreviousVersionId) : null,
				Number = Number,
				Date = Date,
				Type = BusinessLogic.DocumentType.AllTypes[Type],
				State = BusinessLogic.DocumentState.AllStates[State],
				PositionsToSerialize = Positions.Select(position => position.ConvertToBusinessLogic()).ToList(),
			};
		}
	}
}

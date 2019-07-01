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
			Positions = document.Positions.Select(position => new Position(new BusinessLogic.Position(position.Key.ID, position.Value))).ToList();
		}

		#endregion

		public BusinessLogic.Document ConvertToBusinessLogic()
		{
			return new BusinessLogic.Document(
				!string.IsNullOrEmpty(PreviousVersionId) ? (long?) long.Parse(PreviousVersionId) : null,
				BusinessLogic.DocumentType.AllTypes[Type],
				BusinessLogic.DocumentState.AllStates[State])
			{
				ID = ID,
				Number = Number,
				Date = Date,
			};
		}
	}
}

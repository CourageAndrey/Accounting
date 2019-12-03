﻿using System.Xml.Serialization;

namespace ComfortIsland.DataAccessLayer.Xml
{
	[XmlType]
	public class Position
	{
		#region Properties

		[XmlAttribute]
		public long ID
		{ get; set; }

		[XmlAttribute]
		public decimal Count
		{ get; set; }

		#endregion

		#region Constructors

		public Position()
		{ }

		public Position(Accounting.Core.BusinessLogic.Position position)
		{
			ID = position.ID;
			Count = position.Count;
		}

		#endregion
	}
}

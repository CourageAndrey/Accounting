﻿using System.Xml.Serialization;

namespace ComfortIsland.Xml
{
	[XmlType]
	public class Balance
	{
		#region Properties

		[XmlAttribute]
		public long Product
		{ get; set; }

		[XmlAttribute]
		public double Count
		{ get; set; }

		#endregion

		#region Constructors

		public Balance()
		{ }

		public Balance(BusinessLogic.Position balance)
		{
			Product = balance.ID;
			Count = balance.Count;
		}

		#endregion

		public BusinessLogic.Position ConvertToBusinessLogic()
		{
			return new BusinessLogic.Position
			{
				ID = Product,
				Count = Count,
			};
		}
	}
}
﻿using System;

namespace Accounting.Core.Helpers
{
	public class Period
	{
		public DateTime From
		{ get; }

		public DateTime To
		{ get; }

		public Period(DateTime from, DateTime to)
		{
			From = from;
			To = to;
		}
	}
}

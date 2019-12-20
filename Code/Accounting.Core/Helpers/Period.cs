using System;
using System.Globalization;

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

		public override string ToString()
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				"({0} - {1})",
				From,
				To);
		}
	}
}

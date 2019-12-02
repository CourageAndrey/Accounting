namespace ComfortIsland.Reports
{
	public class ReportColumn
	{
		#region Properties

		public string Header
		{ get; }

		public string Binding
		{ get; }

		public bool NeedsDigitRounding
		{ get; }

		public int MinWidth
		{ get; }

		#endregion

		public ReportColumn(string header, string binding, bool needsDigitRounding, int minWidth)
		{
			Header = header;
			Binding = binding;
			NeedsDigitRounding = needsDigitRounding;
			MinWidth = minWidth;
		}
	}
}

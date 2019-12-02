namespace ComfortIsland
{
	public interface IReportExporter
	{
		string SaveFileDialogFilter
		{ get; }

		void ExportReport(IReport report, string fileName);
	}
}

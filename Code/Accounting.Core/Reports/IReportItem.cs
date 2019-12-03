namespace Accounting.Core.Reports
{
	public interface IReportItem
	{
		string GetValue(string columnBinding);
	}
}

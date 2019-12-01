namespace ComfortIsland
{
	public interface IEditDialog<T> : IAccountingApplicationClient
	{
		T EditValue { get; set; }
	}
}
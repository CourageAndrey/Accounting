using Accounting.Core.Application;

namespace Accounting.UI.WPF
{
	public interface IEditDialog<T> : IAccountingApplicationClient
	{
		T EditValue { get; set; }
	}
}
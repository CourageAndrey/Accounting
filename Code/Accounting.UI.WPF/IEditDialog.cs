using Accounting.Core.Application;

namespace Accounting.UI.WPF
{
	public interface IEditDialog<T> : IAccountingApplicationClient
	{
		T EditValue { get; set; }
	}

	public interface IViewModelEditDialog : IAccountingApplicationClient
	{
		IViewModel EditValueObject { get; set; }
	}

	public interface IViewModelEditDialog<ViewModelT> : IViewModelEditDialog
		where ViewModelT : IViewModel
	{
		ViewModelT EditValue { get; set; }
	}
}
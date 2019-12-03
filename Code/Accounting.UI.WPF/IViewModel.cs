using Accounting.Core.BusinessLogic;

namespace Accounting.UI.WPF
{
	public interface IViewModel<out T>
	{
		long? ID
		{ get; }

		T ConvertToBusinessLogic(Database database);
	}
}
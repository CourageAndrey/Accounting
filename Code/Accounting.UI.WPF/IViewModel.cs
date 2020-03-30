using Accounting.Core.BusinessLogic;

namespace Accounting.UI.WPF
{
	public interface IViewModel
	{
		long? ID
		{ get; }

		IEntity ConvertToEntity(IDatabase database);
	}

	public interface IViewModel<out TEntity> : IViewModel
		where TEntity : IEntity
	{
		TEntity ConvertToBusinessLogic(IDatabase database);
	}
}
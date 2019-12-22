using Accounting.Core.BusinessLogic;

namespace Accounting.UI.WPF.ViewModels
{
	public abstract class ViewModelBase<TEntity>: NotifyDataErrorInfo, IViewModel<TEntity>
		where TEntity : IEntity
	{
		public long? ID
		{ get; protected set; }

		public IEntity ConvertToEntity(Database database)
		{
			return ConvertToBusinessLogic(database);
		}

		public abstract TEntity ConvertToBusinessLogic(Database database);
	}
}

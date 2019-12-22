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

		public virtual TEntity ConvertToBusinessLogic(Database database)
		{
			TEntity entity;
			var registry = database.GetRegistry<TEntity>();
			if (ID.HasValue)
			{
				entity = registry[ID.Value];
			}
			else
			{
				registry.Add(entity = CreateNewEntity());
			}

			UpdateProperties(entity, database);

			return entity;
		}

		public abstract TEntity CreateNewEntity();

		public abstract void UpdateProperties(TEntity entity, Database database);
	}
}

using System;
using System.Windows;

using Accounting.Core.BusinessLogic;
using Accounting.UI.WPF.Dialogs;

namespace Accounting.UI.WPF
{
	public interface IUiFactory
	{
		IViewModel CreateViewModel(Type entityType);

		IViewModel CreateViewModel(IEntity entity);

		IViewModelEditDialog CreateEditDialog(IViewModel viewModel, WpfAccountingApplication application);
	}

	public class UiFactory : IUiFactory
	{
		public IViewModel CreateViewModel(Type entityType)
		{
			if (typeof(Unit).IsAssignableFrom(entityType))
			{
				return new ViewModels.Unit();
			}
			else if (typeof(Product).IsAssignableFrom(entityType))
			{
				return new ViewModels.Product();
			}
			else
			{
				throw new NotSupportedException("Unsupported entity type " + entityType.FullName);
			}
		}

		public IViewModel CreateViewModel(IEntity entity)
		{
			var entityType = entity.GetType();

			if (typeof(Unit).IsAssignableFrom(entityType))
			{
				return new ViewModels.Unit(entity as Unit);
			}
			else if (typeof(Product).IsAssignableFrom(entityType))
			{
				return new ViewModels.Product(entity as Product);
			}
			else if (typeof(Document).IsAssignableFrom(entityType))
			{
				return new ViewModels.Document(entity as Document);
			}
			else
			{
				throw new NotSupportedException("Unsupported entity type " + entityType.FullName);
			}
		}

		public IViewModelEditDialog CreateEditDialog(IViewModel viewModel, WpfAccountingApplication application)
		{
			var viewModelType = viewModel.GetType();

			IViewModelEditDialog dialog;
			if (typeof(ViewModels.Unit).IsAssignableFrom(viewModelType))
			{
				dialog = new UnitDialog();
			}
			else if (typeof(ViewModels.Product).IsAssignableFrom(viewModelType))
			{
				dialog = new ProductDialog();
			}
			else if (typeof(ViewModels.Document).IsAssignableFrom(viewModelType))
			{
				dialog = new DocumentDialog();
			}
			else
			{
				throw new NotSupportedException("Unsupported view model type " + viewModelType.FullName);
			}

			((Window) dialog).Owner = application.MainWindow;
			dialog.ConnectTo(application);
			dialog.EditValueObject = viewModel;

			return dialog;
		}
	}
}

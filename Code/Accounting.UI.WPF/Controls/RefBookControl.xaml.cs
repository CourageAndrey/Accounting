using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using Accounting.Core.Application;
using Accounting.Core.BusinessLogic;
using Accounting.UI.WPF.Dialogs;
using Accounting.UI.WPF.Helpers;

namespace Accounting.UI.WPF.Controls
{
	public partial class RefBookControl : IAccountingApplicationClient
	{
		public RefBookControl()
		{
			InitializeComponent();
		}

		public void ConnectTo(IAccountingApplication application)
		{
			_application = (WpfAccountingApplication) application;
		}

		private WpfAccountingApplication _application;

		#region Public interface

		public void Initialize(Type entityType, IEnumerable<DataGridColumn> columns)
		{
			_entityType = entityType;
			_registry = _application.Database.GetRegistry(entityType);

			foreach (var column in columns)
			{
				grid.Columns.Add(column);
			}

			grid.Tag = _registry;
			grid.RefreshGrid();
			grid.UpdateButtonsAvailability(buttonEdit, buttonDelete);
		}

		private Type _entityType;
		private IRegistry _registry;

		public event EventHandler Changed;

		private void raiseChanged()
		{
			var handler = Volatile.Read(ref Changed);
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Buttons

		private void buttonAddClick(object sender, RoutedEventArgs e)
		{
			var viewModel = _application.UiFactory.CreateViewModel(_entityType);
			if (grid.AddNewEntity(viewModel, _application, _entityType))
			{
				raiseChanged();
			}
		}

		private void buttonEditClick(object sender, RoutedEventArgs e)
		{
			if (grid.EditEntity(_application, _entityType))
			{
				raiseChanged();
			}
		}

		private void buttonDeleteClick(object sender, RoutedEventArgs e)
		{
			var selectedItems = grid.SelectedItems.OfType<IEntity>().ToList();
			if (selectedItems.Count < 1) return;
			var message = new StringBuilder();
			foreach (var item in selectedItems)
			{
				message.Append(item.FindUsages(_application.Database));
			}
			if (message.Length > 0)
			{
				LongTextDialog.Warn(
					message.ToString(),
					"Невозможно удалить выбранные сущности, так как они используются");
				return;
			}

			try
			{
				foreach (var item in selectedItems)
				{
					_registry.Remove(item.ID);
				}

				_application.DatabaseDriver.Save(_application.Database);

				raiseChanged();
			}
			catch (Exception error)
			{
				MessageBox.Show(error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#endregion

		private void gridSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			grid.UpdateButtonsAvailability(buttonEdit, buttonDelete);
		}
	}
}

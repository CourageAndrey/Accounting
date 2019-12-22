using System.Windows;

using Accounting.Core.Application;

namespace Accounting.UI.WPF.Dialogs
{
	public partial class UnitDialog : IViewModelEditDialog<ViewModels.Unit>
	{
		public UnitDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IAccountingApplication application)
		{
			_application = application;
			FontSize = application.Settings.UserInterface.FontSize;
		}

		private IAccountingApplication _application;

		public IViewModel EditValueObject
		{
			get { return EditValue; }
			set { EditValue = (ViewModels.Unit) value; }
		}

		public ViewModels.Unit EditValue
		{
			get { return (ViewModels.Unit) contextControl.DataContext; }
			set
			{
				buttonOk.IsEnabled = !value.HasErrors;
				value.ErrorsChanged += (sender, args) => { buttonOk.IsEnabled = !value.HasErrors; };
				contextControl.DataContext = value;
			}
		}

		private void okClick(object sender, RoutedEventArgs e)
		{
			if (!EditValue.HasErrors)
			{
				DialogResult = true;
			}
		}

		private void cancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}

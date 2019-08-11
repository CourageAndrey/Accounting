using System.Windows;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland.Dialogs
{
	public partial class UnitDialog : IEditDialog<ViewModels.Unit>, IApplicationClient
	{
		public UnitDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IApplication application)
		{
			_application = application;
			FontSize = application.Settings.FontSize;
		}

		private IApplication _application;

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

		public void Initialize(Database database)
		{ }

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

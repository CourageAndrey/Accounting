using System.Windows;

namespace Accounting.UI.WPF.Dialogs
{
	public partial class LongTextDialog
	{
		public LongTextDialog()
		{
			InitializeComponent();
		}

		#region Buttons

		private void okClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void yesClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void noClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		#endregion

		#region API

		public static bool Ask(string text, string title)
		{
			var dialog = new LongTextDialog();
			dialog.Title = title;
			dialog.text.Text = text;
			dialog.buttonOk.Visibility = Visibility.Collapsed;
			dialog.imageWarning.Visibility = dialog.imageInformation.Visibility = Visibility.Collapsed;

			return dialog.ShowDialog() == true;
		}

		public static void Warn(string text, string title)
		{
			var dialog = new LongTextDialog();
			dialog.Title = title;
			dialog.text.Text = text;
			dialog.buttonYes.Visibility = dialog.buttonNo.Visibility = Visibility.Collapsed;
			dialog.imageQuestion.Visibility = dialog.imageInformation.Visibility = Visibility.Collapsed;

			dialog.ShowDialog();
		}

		public static void Info(string text, string title)
		{
			var dialog = new LongTextDialog();
			dialog.Title = title;
			dialog.text.Text = text;
			dialog.buttonYes.Visibility = dialog.buttonNo.Visibility = Visibility.Collapsed;
			dialog.imageQuestion.Visibility = dialog.imageWarning.Visibility = Visibility.Collapsed;

			dialog.ShowDialog();
		}

		#endregion
	}
}

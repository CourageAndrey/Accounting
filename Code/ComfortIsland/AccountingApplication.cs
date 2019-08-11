using System;
using System.IO;
using System.Text;
using System.Windows;

namespace ComfortIsland
{
	internal class AccountingApplication : Application
	{
		public AccountingApplication()
		{
			DispatcherUnhandledException += (sender, exceptionArgs) =>
			{
				LogError("Application.DispatcherUnhandledException", exceptionArgs.Exception);
			};
			AppDomain.CurrentDomain.UnhandledException += (sender, exceptionArgs) =>
			{
				var error = exceptionArgs.ExceptionObject as Exception;
				LogError(
					"AppDomain.CurrentDomain.UnhandledException",
					error ?? new Exception("" + exceptionArgs.ExceptionObject));
			};

			MainWindow = new MainWindow();
			ShutdownMode = ShutdownMode.OnMainWindowClose;
		}

		private static void LogError(string source, Exception error)
		{
			var text = new StringBuilder();
			text.AppendLine("================================");
			text.AppendLine(source);
			text.AppendLine(DateTime.Now.ToString("F"));

			do
			{
				text.AppendLine();
				text.AppendLine(error.GetType().FullName);
				text.AppendLine(error.Message);
				text.AppendLine(error.StackTrace);
				error = error.InnerException;
			} while (error != null);

			File.AppendAllText("Exception.txt", text.ToString());
		}

		[STAThread]
		static void Main()
		{
			var application = new AccountingApplication();
			application.MainWindow.Show();
			application.Run();
		}
	}
}

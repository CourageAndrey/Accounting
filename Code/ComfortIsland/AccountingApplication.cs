using System;
using System.IO;
using System.Text;
using System.Windows;

using ComfortIsland.Configuration;

namespace ComfortIsland
{
	internal class AccountingApplication : Application, IApplication
	{
		#region Свойства

		public string StartupPath
		{ get; }

		public Settings Settings
		{ get; }

		public BusinessLogic.Database Database
		{ get; }

		#endregion

		public AccountingApplication()
		{
			var appDomain = AppDomain.CurrentDomain;
			setupExceptionHandling(appDomain);

			StartupPath = AppDomain.CurrentDomain.BaseDirectory;
			Settings = new Settings(Configuration.Xml.Settings.Load(StartupPath));
			Database = Settings.DataAccessLayer.DatabaseDriver.TryLoad();

			var mainWindow = new MainWindow();
			mainWindow.ConnectTo(this);
			MainWindow = mainWindow;
			ShutdownMode = ShutdownMode.OnMainWindowClose;
		}

		#region Обработка ошибок

		private void setupExceptionHandling(AppDomain appDomain)
		{
			DispatcherUnhandledException += (sender, exceptionArgs) =>
			{
				LogError("Application.DispatcherUnhandledException", exceptionArgs.Exception);
			};

			appDomain.UnhandledException += (sender, exceptionArgs) =>
			{
				var error = exceptionArgs.ExceptionObject as Exception;
				LogError(
					"AppDomain.CurrentDomain.UnhandledException",
					error ?? new Exception("" + exceptionArgs.ExceptionObject));
			};
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

		#endregion

		[STAThread]
		static void Main()
		{
			var application = new AccountingApplication();
			application.MainWindow.Show();
			application.Run();
		}
	}
}

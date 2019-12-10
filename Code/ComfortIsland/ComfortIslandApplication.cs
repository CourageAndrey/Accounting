using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

using Accounting.Core.Application;
using Accounting.Core.BusinessLogic;
using Accounting.Core.Configuration;

namespace ComfortIsland
{
	internal class ComfortIslandApplication : Application, IAccountingApplication
	{
		#region Свойства

		public string StartupPath
		{ get; }

		public Settings Settings
		{ get; }

		public IDatabaseDriver DatabaseDriver
		{ get { return Settings.DataAccessLayer.DatabaseDriver; } }

		public IUserInterface UserInterface
		{ get { return Settings.UserInterface.Engine; } }

		public IReportExporter ReportExporter
		{ get { return Settings.Reporting.Exporter; } }

		public Database Database
		{ get; }

		#endregion

		public ComfortIslandApplication()
		{
			var appDomain = AppDomain.CurrentDomain;
			setupExceptionHandling(appDomain);

			StartupPath = appDomain.BaseDirectory;
			this.LoadPlugins(new DirectoryInfo(Path.Combine(StartupPath, "Plugins")));

			Settings = new Settings(Accounting.Core.Configuration.Xml.Settings.Load(StartupPath));

			Database = DatabaseDriver.TryLoad();

			var wpfAssembly = appDomain.GetAssemblies().First(assembly => assembly.FullName.StartsWith("Accounting.UI.WPF") && !assembly.FullName.Contains("Test"));
			var mainWindowType = wpfAssembly.GetType("Accounting.UI.WPF.MainWindow");
			var mainWindow = Activator.CreateInstance(mainWindowType);
			((IAccountingApplicationClient) mainWindow).ConnectTo(this);
			MainWindow = (Window) mainWindow;
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
			var application = new ComfortIslandApplication();
			application.MainWindow.Show();
			application.Run();
		}
	}
}

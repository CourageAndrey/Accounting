﻿using System;
using System.IO;
using System.Text;
using System.Windows;

using Accounting.Core.Application;
using Accounting.Core.BusinessLogic;
using Accounting.Core.Configuration;

namespace Accounting.UI.WPF
{
	public class WpfAccountingApplication : Application, IAccountingApplication
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

		public IDatabase Database
		{ get; }

		public virtual IUiFactory UiFactory
		{ get; }

		#endregion

		public WpfAccountingApplication()
		{
			var appDomain = AppDomain.CurrentDomain;
			setupExceptionHandling(appDomain);

			UiFactory = new UiFactory();

			StartupPath = appDomain.BaseDirectory;
			this.LoadPlugins(new DirectoryInfo(Path.Combine(StartupPath, "Plugins")));
			new WpfUiPlugin().Setup(this);

			Settings = new Settings(Accounting.Core.Configuration.Xml.Settings.Load(StartupPath));

			Database = DatabaseDriver.TryLoad();

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
			LogSingleError(error, text);

			File.AppendAllText("Exception.txt", text.ToString());
		}

		private static void LogSingleError(Exception error, StringBuilder text)
		{
			text.AppendLine();
			text.AppendLine(error.GetType().FullName);
			text.AppendLine(error.Message);
			text.AppendLine(error.StackTrace);
			var typeLoadException = error as System.Reflection.ReflectionTypeLoadException;
			if (typeLoadException != null)
			{
				foreach (var type in typeLoadException.Types)
				{
					text.AppendLine("Involved type: " + type);
				}
				text.AppendLine("Loader exceptions:");
				foreach (var loaderException in typeLoadException.LoaderExceptions)
				{
					LogSingleError(loaderException, text);
				}
			}
			if (error.InnerException != null)
			{
				LogSingleError(error.InnerException, text);
			}
		}

		#endregion

		[STAThread]
		static void Main()
		{
			var application = new WpfAccountingApplication();
			application.MainWindow.Show();
			application.Run();
		}
	}
}

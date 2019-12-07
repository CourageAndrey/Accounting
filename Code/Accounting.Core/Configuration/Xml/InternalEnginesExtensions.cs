using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Accounting.Core.Configuration.Xml
{
	public static class InternalEnginesExtensions
	{
		private static readonly IDictionary<string, Type> _databaseEngines = new Dictionary<string, Type>();
		private static readonly IDictionary<string, Type> _reportExportEngines = new Dictionary<string, Type>();
		private static readonly IDictionary<string, Type> _userInterfaceEngines = new Dictionary<string, Type>();

		#region Register-methods

		public static void RegisterDatabaseEngine(Type databaseEngineType, string nodeName)
		{
			_databaseEngines.Add(nodeName, databaseEngineType);
		}

		public static void RegisterDatabaseEngine<DatabaseEngineT>(string nodeName)
			where DatabaseEngineT : DatabaseDriver
		{
			RegisterDatabaseEngine(typeof(DatabaseEngineT), nodeName);
		}

		public static void RegisterReportExportEngine(Type reportExportEngineType, string nodeName)
		{
			_reportExportEngines.Add(nodeName, reportExportEngineType);
		}

		public static void RegisterReportExportEngine<ReportExportEngineT>(string nodeName)
			where ReportExportEngineT : ReportExportDriver
		{
			RegisterReportExportEngine(typeof(ReportExportEngineT), nodeName);
		}

		public static void RegisterUserInterfaceEngine(Type userInterfaceEngineType, string nodeName)
		{
			_userInterfaceEngines.Add(nodeName, userInterfaceEngineType);
		}

		public static void RegisterUserInterfaceEngine<UserInterfaceEngineT>(string nodeName)
			where UserInterfaceEngineT : UserInterfaceDriver
		{
			RegisterUserInterfaceEngine(typeof(UserInterfaceEngineT), nodeName);
		}

		#endregion Register-methods

#warning Раскомментировать и корректно реализовать!
		/*public static void RegisterExtensions(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract))
			{
				if (typeof(DatabaseDriver).IsAssignableFrom(type))
				{
					RegisterDatabaseEngine(type, );
				}
				if (typeof(ReportExportDriver).IsAssignableFrom(type))
				{
					RegisterReportExportEngine(type, );
				}
				if (typeof(UserInterfaceDriver).IsAssignableFrom(type))
				{
					RegisterUserInterfaceEngine(type, );
				}
			}
		}

		public static void RegisterExtensions(DirectoryInfo pluginsFolder)
		{
			foreach (var file in pluginsFolder.GetFiles("*.dll"))
			{
				RegisterExtensions(Assembly.LoadFile(file.FullName));
			}
		}*/

		#region GetImplementation-methods

		internal static IReadOnlyDictionary<string, Type> GetRegisteredDatabaseEngines()
		{
			return new ReadOnlyDictionary<string, Type>(_databaseEngines);
		}

		internal static IReadOnlyDictionary<string, Type> GetRegisteredReportExportEngines()
		{
			return new ReadOnlyDictionary<string, Type>(_reportExportEngines);
		}

		internal static IReadOnlyDictionary<string, Type> GetRegisteredUserInterfaceEngines()
		{
			return new ReadOnlyDictionary<string, Type>(_userInterfaceEngines);
		}

		#endregion GetImplementation-methods
	}
}

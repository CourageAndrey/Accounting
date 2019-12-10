using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Accounting.Core.Application
{
	public interface IAccountingPlugin
	{
		void Setup(IAccountingApplication application);
	}

	public static class PluginLoader
	{
		public static void LoadPlugins(this IAccountingApplication application, DirectoryInfo pluginsFolder)
		{
			foreach (var file in pluginsFolder.GetFiles("*.dll"))
			{
				application.LoadPlugins(Assembly.LoadFile(file.FullName));
			}
			foreach (var file in pluginsFolder.GetFiles("*.exe"))
			{
				application.LoadPlugins(Assembly.LoadFile(file.FullName));
			}
		}

		public static void LoadPlugins(this IAccountingApplication application, Assembly assembly)
		{
			var pluginType = typeof(IAccountingPlugin);
			foreach (var type in assembly.GetTypes().Where(t => pluginType.IsAssignableFrom(t) && !t.IsAbstract))
			{
				var plugin = (IAccountingPlugin) Activator.CreateInstance(type);
				plugin.Setup(application);
			}
		}
	}
}

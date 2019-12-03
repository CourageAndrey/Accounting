using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Accounting.Core.Configuration.Xml;

namespace Accounting.Core.Configuration.Extensions
{
	public static class DatabaseDriverExtensions
	{
		private static readonly IDictionary<string, Type> _implementations = new Dictionary<string, Type>();

		public static void RegisterImplementation<DriverT>(string nodeName)
			where DriverT : DatabaseDriver
		{
			_implementations.Add(nodeName, typeof(DriverT));
		}

		internal static IReadOnlyDictionary<string, Type> GetRegisteredImplementations()
		{
			return new ReadOnlyDictionary<string, Type>(_implementations);
		}
	}
}

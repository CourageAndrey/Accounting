using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Accounting.Core.Configuration.Xml
{
	[XmlType]
	public abstract class DatabaseDriver
	{
		public abstract Accounting.Core.DataAccessLayer.IDatabaseDriver CreateDataAccessLayer();

		#region Customization

		[XmlIgnore]
		private static readonly IDictionary<string, Type> _implementations = new Dictionary<string, Type>();

		public static void RegisterImplementation<DriverT>(string nodeName)
			where DriverT : DatabaseDriver
		{
			_implementations.Add(nodeName, typeof(DriverT));
		}

		public static IReadOnlyDictionary<string, Type> GetRegisteredImplementations()
		{
			return new ReadOnlyDictionary<string, Type>(_implementations);
		}

		#endregion
	}
}

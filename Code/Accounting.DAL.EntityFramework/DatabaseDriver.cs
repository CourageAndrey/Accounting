using System;

using Accounting.Core.Application;

namespace Accounting.DAL.EntityFramework
{
	public class DatabaseDriver : IDatabaseDriver
	{
		private readonly string _connectionString;

		internal DatabaseDriver(string connectionString)
		{
			_connectionString = connectionString;
		}

		public bool CanLoad
		{ get { throw new NotImplementedException(); } }

		public Core.BusinessLogic.Database Load()
		{
			throw new NotImplementedException();
		}

		public void Save(Core.BusinessLogic.Database database)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.IO;

using ComfortIsland.BusinessLogic;

namespace ComfortIsland
{
	public class Settings
	{
		public double FontSize
		{ get { return 16; } }

		public BalanceValidationStrategy BalanceValidationStrategy
		{ get { return BalanceValidationStrategy.FinalOnly; } }

		public IDatabaseDriver DatabaseDriver
		{ get { return _databaseDriver; } }
		private readonly IDatabaseDriver _databaseDriver = new Xml.DatabaseDriver(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.xml"));
	}
}

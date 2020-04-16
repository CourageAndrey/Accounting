using System;
using System.Collections.Generic;

using NUnit.Framework;

using Accounting.Core.BusinessLogic;

namespace Accounting.Core.UnitTests.BusinessLogic
{
	public class DatabaseRegistriesTest
	{
		[Test]
		public void RegistriesOfAllKnownTypesWorksCorrect()
		{
			// arrange
			var database = new InMemoryDatabase(
				new Unit[0],
				new Product[0],
				new Dictionary<long, decimal>(),
				new Document[0]);

			// act and assert
			Assert.AreSame(database.Units, database.GetRegistry<Unit>());
			Assert.AreSame(database.Products, database.GetRegistry<Product>());
			Assert.AreSame(database.Documents, database.GetRegistry<Document>());
		}

		[Test]
		public void ImpossibleToGetRegistryOfUnkownType()
		{
			// arrange
			var database = new InMemoryDatabase(
				new Unit[0],
				new Product[0],
				new Dictionary<long, decimal>(),
				new Document[0]);

			// act and assert
			Assert.Throws<NotSupportedException>(() => database.GetRegistry<UnknownEntity>());
		}

		private class UnknownEntity : IEntity
		{
			public long ID
			{ get; set; }
		}
	}
}

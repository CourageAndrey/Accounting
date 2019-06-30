using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ComfortIsland;
using ComfortIsland.Helpers;

namespace UnitTests
{
	[TestClass]
	public class IdGeneratorTest
	{
		[TestMethod]
		public void FirstIdIsOne()
		{
			// arrange
			var entities = new List<TestEntity>();

			// act
			long id = IdGenerator.NewId(entities);

			// assert
			Assert.AreEqual(1, id);
		}

		[TestMethod]
		public void GeneratedIdIsMaxPlusOne()
		{
			// arrange
			var entities = new List<TestEntity>
			{
				new TestEntity { ID = 2 },
				new TestEntity { ID = 4 },
				new TestEntity { ID = 6 },
				new TestEntity { ID = 5 },
				new TestEntity { ID = 3 },
				new TestEntity { ID = 1 },
			};

			// act
			long id = IdGenerator.NewId(entities);

			// assert
			Assert.AreEqual(7, id);
		}

		private class TestEntity : IEntity
		{
			public long ID { get; set; }

			public StringBuilder FindUsages(ComfortIsland.BusinessLogic.Database database)
			{
				throw new NotSupportedException();
			}

			public bool Validate(ComfortIsland.BusinessLogic.Database database, out StringBuilder errors)
			{
				throw new NotSupportedException();
			}
		}
	}
}

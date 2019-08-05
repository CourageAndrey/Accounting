using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using ComfortIsland.Helpers;

namespace ComfortIsland.UnitTests.Helpers
{
	public class IdGeneratorTest
	{
		[Test]
		public void FirstIdIsOne()
		{
			// arrange
			var entities = new List<TestEntity>();

			// act
			long id = entities.NewId();

			// assert
			Assert.AreEqual(1, id);
		}

		[Test]
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
			long id = entities.NewId();

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
		}
	}
}

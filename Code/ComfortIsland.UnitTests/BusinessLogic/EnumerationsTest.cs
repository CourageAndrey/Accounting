using System;
using System.Linq;

using NUnit.Framework;

namespace ComfortIsland.UnitTests.BusinessLogic
{
	public class EnumerationsTest
	{
		[Test]
		public void DocumentType()
		{
			var values = Enum.GetValues(typeof(ComfortIsland.DataAccessLayer.Xml.DocumentType)).OfType<ComfortIsland.DataAccessLayer.Xml.DocumentType>().ToList();
			Assert.IsTrue(values.SequenceEqual(ComfortIsland.BusinessLogic.DocumentType.AllTypes.Keys));
		}

		[Test]
		public void DocumentState()
		{
			var values = Enum.GetValues(typeof(ComfortIsland.DataAccessLayer.Xml.DocumentState)).OfType<ComfortIsland.DataAccessLayer.Xml.DocumentState>().ToList();
			Assert.IsTrue(values.SequenceEqual(ComfortIsland.BusinessLogic.DocumentState.AllStates.Keys));
		}
	}
}

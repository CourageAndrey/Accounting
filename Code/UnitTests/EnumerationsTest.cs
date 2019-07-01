using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class EnumerationsTest
	{
		[TestMethod]
		public void DocumentType()
		{
			var values = Enum.GetValues(typeof(ComfortIsland.Xml.DocumentType)).OfType<ComfortIsland.Xml.DocumentType>().ToList();
			Assert.IsTrue(values.SequenceEqual(ComfortIsland.BusinessLogic.DocumentType.AllTypes.Keys));
		}

		[TestMethod]
		public void DocumentState()
		{
			var values = Enum.GetValues(typeof(ComfortIsland.Xml.DocumentState)).OfType<ComfortIsland.Xml.DocumentState>().ToList();
			Assert.IsTrue(values.SequenceEqual(ComfortIsland.BusinessLogic.DocumentState.AllStates.Keys));
		}
	}
}

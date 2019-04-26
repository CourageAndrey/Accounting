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
	}
}

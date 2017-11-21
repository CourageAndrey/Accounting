using System;
using System.Linq;

using ComfortIsland.Database;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class EnumerationsTest
	{
		[TestMethod]
		public void DocumentType()
		{
			var values = Enum.GetValues(typeof(DocumentType)).OfType<DocumentType>().ToList();
			Assert.IsTrue(values.SequenceEqual(DocumentTypeImplementation.AllTypes.Keys));
		}
	}
}

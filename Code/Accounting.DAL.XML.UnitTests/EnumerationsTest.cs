using System;
using System.Linq;

using ComfortIsland.DataAccessLayer.Xml;

using NUnit.Framework;

namespace Accounting.DAL.XML.UnitTests
{
	public class EnumerationsTest
	{
		[Test]
		public void DocumentType()
		{
			var enums = Enum.GetValues(typeof(DocumentType)).OfType<DocumentType>().ToList();
			var objects = Accounting.Core.BusinessLogic.DocumentType.All.Select(item => item.ToEnum()).ToList();
			Assert.IsTrue(enums.SequenceEqual(objects));
		}

		[Test]
		public void DocumentState()
		{
			var enums = Enum.GetValues(typeof(DocumentState)).OfType<DocumentState>().ToList();
			var objects = Accounting.Core.BusinessLogic.DocumentState.All.Select(item => item.ToEnum()).ToList();
			Assert.IsTrue(enums.SequenceEqual(objects));
		}
	}
}

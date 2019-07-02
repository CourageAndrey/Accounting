using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ComfortIsland.Helpers;

namespace UnitTests
{
	[TestClass]
	public class DictionaryComparerTest
	{
		[TestMethod]
		public void ComparisonFailsIfOneOfDictionariesIsNull()
		{
			TestHelper.Throws<ArgumentNullException>(() => DictionaryComparer.IsEqualTo(null, new Dictionary<string, string>()));
			TestHelper.Throws<ArgumentNullException>(() => DictionaryComparer.IsEqualTo(new Dictionary<string, string>(), null));
		}

		[TestMethod]
		public void TwoEmptyDictionariesAreEqual()
		{
			Assert.IsTrue(new Dictionary<string, string>().IsEqualTo(new Dictionary<string, string>()));
		}

		[TestMethod]
		public void DictionaryIsEqualToItself()
		{
			var dictionary = new Dictionary<string, string>
			{
				{ "a", "A" },
				{ "b", "B" },
				{ "c", "C" },
			};
			Assert.IsTrue(dictionary.IsEqualTo(dictionary));
		}

		[TestMethod]
		public void EqualDictionariesAreEqual()
		{
			var a = new Dictionary<string, string>
			{
				{ "a", "A" },
				{ "b", "B" },
				{ "c", "C" },
			};
			var b = new Dictionary<string, string>
			{
				{ "c", "C" },
				{ "b", "B" },
				{ "a", "A" },
			};
			Assert.IsTrue(a.IsEqualTo(b));
		}

		[TestMethod]
		public void DifferentDictionariesAreNotEqual()
		{
			var a = new Dictionary<string, string>
			{
				{ "a", "A" },
				{ "b", "B" },
				{ "c", "C" },
			};
			var b = new Dictionary<string, string>
			{
				{ "x", "X" },
				{ "y", "Y" },
				{ "z", "Z" },
			};
			var c = new Dictionary<string, string>
			{
				{ "a", "A" },
				{ "b", "B" },
			};
			var d = new Dictionary<string, string>
			{
				{ "a", "A" },
				{ "b", "B" },
				{ "c", "C" },
				{ "d", "D" },
			};
			Assert.IsFalse(a.IsEqualTo(b));
			Assert.IsFalse(a.IsEqualTo(c));
			Assert.IsFalse(a.IsEqualTo(d));
		}
	}
}

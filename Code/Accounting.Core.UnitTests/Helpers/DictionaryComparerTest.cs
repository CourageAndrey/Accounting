using System;
using System.Collections.Generic;

using NUnit.Framework;

using ComfortIsland.Helpers;

namespace Accounting.Core.UnitTests.Helpers
{
	public class DictionaryComparerTest
	{
		[Test]
		public void ComparisonFailsIfOneOfDictionariesIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => DictionaryComparer.IsEqualTo(null, new Dictionary<string, string>()));
			Assert.Throws<ArgumentNullException>(() => DictionaryComparer.IsEqualTo(new Dictionary<string, string>(), null));
		}

		[Test]
		public void TwoEmptyDictionariesAreEqual()
		{
			Assert.IsTrue(new Dictionary<string, string>().IsEqualTo(new Dictionary<string, string>()));
		}

		[Test]
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

		[Test]
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

		[Test]
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

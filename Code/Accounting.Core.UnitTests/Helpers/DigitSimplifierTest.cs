using ComfortIsland.Helpers;

using NUnit.Framework;

namespace Accounting.Core.UnitTests.Helpers
{
	public class DigitSimplifierTest
	{
		[Test]
		public void IntegerNumbersReturnAsIs()
		{
			for (int i = 0; i < 10; i++)
			{
				Assert.AreEqual(i.ToString(), ((decimal) i).Simplify());
			}
		}

		[Test]
		public void DoubleNumbersAreRounded()
		{
			Assert.AreEqual("0.1", 0.1m.Simplify());
			Assert.AreEqual("0.1", 0.11m.Simplify());
			Assert.AreEqual("0.1", 0.111m.Simplify());
			Assert.AreEqual("0.1", 0.11111m.Simplify());
		}
	}
}

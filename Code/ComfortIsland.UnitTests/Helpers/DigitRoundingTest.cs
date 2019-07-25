using ComfortIsland.Helpers;

using NUnit.Framework;

namespace ComfortIsland.UnitTests.Helpers
{
	public class DigitRoundingTest
	{
		[Test]
		public void IntegerNumbersReturnAsIs()
		{
			for (int i = 0; i < 10; i++)
			{
				Assert.AreEqual(i.ToString(), DigitRoundingConverter.Simplify(i));
			}
		}

		[Test]
		public void DoubleNumbersAreRounded()
		{
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.1m));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.11m));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.111m));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.11111m));
		}
	}
}

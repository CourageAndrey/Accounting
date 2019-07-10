using ComfortIsland.Helpers;

using NUnit.Framework;

namespace ComfortIsland.UnitTests
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
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.1));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.11));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.111));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.11111));
		}
	}
}

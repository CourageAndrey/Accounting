using ComfortIsland.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class DigitRoundingTest
	{
		[TestMethod]
		public void IntegerNumbersReturnAsIs()
		{
			for (int i = 0; i < 10; i++)
			{
				Assert.AreEqual(i.ToString(), DigitRoundingConverter.Simplify(i));
			}
		}

		[TestMethod]
		public void DoubleNumbersAreRounded()
		{
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.1));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.11));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.111));
			Assert.AreEqual("0.1", DigitRoundingConverter.Simplify(0.11111));
		}
	}
}

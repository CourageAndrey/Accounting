using System;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	internal static class TestHelper
	{
		public static ExceptionT Throws<ExceptionT>(Action code)
			where ExceptionT : Exception
		{
			try
			{
				code();
			}
			catch (ExceptionT e)
			{
				return e;
			}
			Assert.Fail(string.Format(CultureInfo.InvariantCulture, "Code executed successfully, but exception of type {0} expected!", typeof(ExceptionT)));
			return null;
		}
	}
}

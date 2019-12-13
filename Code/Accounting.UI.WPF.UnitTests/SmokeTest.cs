using NUnit.Framework;

using Accounting.UI.WPF.UnitTests.Framework;

namespace Accounting.UI.WPF.UnitTests
{
	public class SmokeTest
	{
		[Test]
		public void ApplicationLaunchIsSuccessfull()
		{
			// arrange
			var application = Framework.UI.Run(() => new WpfAccountingApplication());
			var mainWindow = application.Get(x => x.MainWindow);

			// act
			application.Invoke(x => x.Shutdown());

			// assert
			Assert.IsNotNull(mainWindow);
		}
	}
}

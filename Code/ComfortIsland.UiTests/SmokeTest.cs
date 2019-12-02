using NUnit.Framework;

using ComfortIsland.UiTests.Framework;

namespace ComfortIsland.UiTests
{
	public class SmokeTest
	{
		[Test]
		public void ApplicationLaunchIsSuccessfull()
		{
			// arrange
			var application = UI.Run(() => new ComfortIslandApplication());
			var mainWindow = application.Get(x => x.MainWindow);

			// act
			application.Invoke(x => x.Shutdown());

			// assert
			Assert.IsNotNull(mainWindow);
		}
	}
}

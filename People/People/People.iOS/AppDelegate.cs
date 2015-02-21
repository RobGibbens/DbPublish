using Foundation;
using SQLite.Net.Platform.XamarinIOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace People.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();

			string dbPath = FileAccessHelper.GetLocalFilePath("people.db3");

			LoadApplication(new App(dbPath, new SQLitePlatformIOS()));

			return base.FinishedLaunching(app, options);
		}
	}
}
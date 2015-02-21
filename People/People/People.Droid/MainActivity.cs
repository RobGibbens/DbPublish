using Android.App;
using Android.Content.PM;
using Android.OS;
using SQLite.Net.Platform.XamarinAndroid;

namespace People.Droid
{
[Activity (Label = "People", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
{
	protected override void OnCreate (Bundle bundle)
	{
		base.OnCreate (bundle);

		global::Xamarin.Forms.Forms.Init (this, bundle);

		string dbPath = FileAccessHelper.GetLocalFilePath ("people.db3");

		LoadApplication (new People.App (dbPath, new SQLitePlatformAndroid ()));
	}
}
}
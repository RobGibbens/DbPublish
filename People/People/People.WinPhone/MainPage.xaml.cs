using Microsoft.Phone.Controls;
using SQLite.Net.Platform.WindowsPhone8;

namespace People.WinPhone
{
public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
{
	public MainPage ()
	{
		InitializeComponent ();

		SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

		global::Xamarin.Forms.Forms.Init ();

		string dbPath = FileAccessHelper.GetLocalFilePath ("people.db3");

		LoadApplication (new People.App (dbPath, new SQLitePlatformWP8 ()));
	}
}
}

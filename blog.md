> *Sample code is available at [my Github repo](https://github.com/RobGibbens/DbPublish)*

It's very common for a mobile application to utilize a local sqlite database. The combination of [Sqlite.net](http://developer.xamarin.com/guides/cross-platform/application_fundamentals/data/part_3_using_sqlite_orm/) and Xamarin.Forms makes this [very easy](http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/databases/). One of the reasons that we choose Sqlite as our mobile database is that it is single file and easily works cross platform.  The same file can be created and opened on iOS, Android, Windows, Mac, Windows Phone, and WinRT.

Sometimes we may want to create our database from scratch the first time our application runs.  Sqlite.net will automatically create the file if it doesn't exist the first time we attempt to connect to it.  We can then use the **CreateTable&lt;TModel&gt;()** method on the **SQLiteConnection** class to create our tables from our C# models.

Other times, though, we will want to ship a prepopulated database with the application. We may have some lookup tables that need have records in them, or default data that the application needs in order to function. Fortunately, this is easy to accomplish as well.

##The steps##

- Create the database file
- Link the database file to each platform specific project
- Copy the database file from the application bundle to a writable location on the file system
- Open the database file for reading and writing


##Create SQLite database##

![Create SQL Schema](http://arteksoftware.com/content/images/2015/02/SqlSchema.png)

We can create a Sqlite database file using a variety of tools on both Mac and Windows. I use [DB Browser for SQLite](http://sqlitebrowser.org/), a cross platform tool which will allow us to create the database, define the schema, and add records to the database. For this example, we'll be creating a file named "**people.db3**".

##Create the Model##

When using Sqlite.net, it is important that the C# models match the table definitions. If there is a discrepancy between the model and the table, we will have unexpected results. We may accidently create new tables, new columns in existing tables, or no results from queries.

```language-csharp
[Table ("people")]
public class Person
{
	[PrimaryKey, AutoIncrement, Column("Id")]
	public int Id { get; set; }

	[MaxLength (250), Unique, Column("Name")]
	public string Name { get; set; }
}
```

##Link database file##

First we need to include the database file in each platform specific project (iOS/Android/WP8). We can keep the file anywhere on the desktop's file system. We're going to use [File Linking](http://blogs.msdn.com/b/jjameson/archive/2009/04/02/linked-files-in-visual-studio-solutions.aspx) to include the exact same file in each project.

####iOS####

For iOS, we will link the db3 file into the Resources folder. Be sure to set the Build Action to **BundleResource**.

{<4>}![Link database file to iOS](http://arteksoftware.com/content/images/2015/02/IncludeIOSDb-1.png)

####Android####

On Android, we will link the db3 file into the Assets folder, and set the Build Action to **AndroidAsset**.

{<6>}![Link database file to Android](http://arteksoftware.com/content/images/2015/02/IncludeAndroidDb.png)

####Windows Phone 8####

Windows Phone will link the database file into the root of the project, and set the Build Action as **Content**.

{<7>}![Link database file to Windows Phone 8](http://arteksoftware.com/content/images/2015/02/IncludeWP8Db.png)

##Copy the database file##

Although we've now included the database file with our application, and it will be part of the bundle that gets installed on the user's device, the app bundle and its included files are read-only. In order to actually change the database schema or add new records we'll need to copy the file to a writable location. Each device contains a special folder for each app, known as the app sandbox, to store files. We'll use each platform's file APIs to copy the file from the app bundle to the app sandbox.

####iOS####

```language-csharp
[Register ("AppDelegate")]
public partial class AppDelegate : FormsApplicationDelegate
{
	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		Forms.Init ();

		string dbPath = FileAccessHelper.GetLocalFilePath ("people.db3");

		LoadApplication (new App (dbPath, new SQLitePlatformIOS ()));

		return base.FinishedLaunching (app, options);
	}
}
```

```language-csharp
public class FileAccessHelper
{
	public static string GetLocalFilePath (string filename)
	{
		string docFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		string libFolder = Path.Combine (docFolder, "..", "Library", "Databases");

		if (!Directory.Exists (libFolder)) {
			Directory.CreateDirectory (libFolder);
		}

		string dbPath = Path.Combine (libFolder, filename);

		CopyDatabaseIfNotExists (dbPath);

		return dbPath;
	}

	private static void CopyDatabaseIfNotExists (string dbPath)
	{
		if (!File.Exists (dbPath)) {
			var existingDb = NSBundle.MainBundle.PathForResource ("people", "db3");
			File.Copy (existingDb, dbPath);
		}
	}
}
```

####Android####

```language-csharp
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
```

```language-csharp
public class FileAccessHelper
{
	public static string GetLocalFilePath (string filename)
	{
		string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		string dbPath = Path.Combine (path, filename);

		CopyDatabaseIfNotExists (dbPath);

		return dbPath;
	}

	private static void CopyDatabaseIfNotExists (string dbPath)
	{
		if (!File.Exists (dbPath)) {
			using (var br = new BinaryReader (Application.Context.Assets.Open ("people.db3"))) {
				using (var bw = new BinaryWriter (new FileStream (dbPath, FileMode.Create))) {
					byte[] buffer = new byte[2048];
					int length = 0;
					while ((length = br.Read (buffer, 0, buffer.Length)) > 0) {
						bw.Write (buffer, 0, length);
					}
				}
			}
		}
	}
}
```

####Windows Phone 8####

```language-csharp
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
```

```language-csharp
public class FileAccessHelper
{
	public static string GetLocalFilePath (string filename)
	{
		string path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
		string dbPath = Path.Combine (path, filename);

		CopyDatabaseIfNotExists (dbPath);

		return dbPath;
	}

	public static void CopyDatabaseIfNotExists (string dbPath)
	{
		var storageFile = IsolatedStorageFile.GetUserStoreForApplication ();

		if (!storageFile.FileExists (dbPath)) {
			using (var resourceStream = Application.GetResourceStream (new Uri ("people.db3", UriKind.Relative)).Stream) {
				using (var fileStream = storageFile.CreateFile (dbPath)) {
					byte[] readBuffer = new byte[4096];
					int bytes = -1;

					while ((bytes = resourceStream.Read (readBuffer, 0, readBuffer.Length)) > 0) {
						fileStream.Write (readBuffer, 0, bytes);
					}
				}
			}
		}
	}
}
```


###Source Code###

> You can find a complete sample on [my Github repo](https://github.com/RobGibbens/DbPublish)

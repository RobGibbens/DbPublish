using SQLite.Net.Interop;
using Xamarin.Forms;

namespace People
{
	public class App : Application
	{
		public static PersonRepository PersonRepo { get; private set; }

		public App(string dbPath, ISQLitePlatform sqlitePlatform)
		{
			//set database path first, then retrieve main page
			PersonRepo = new PersonRepository(sqlitePlatform, dbPath);

			this.MainPage = new MainPage();
		}
	}
}
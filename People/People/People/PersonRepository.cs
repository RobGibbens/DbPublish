using System;
using System.Collections.Generic;
using System.Linq;
using People.Models;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Async;
using System.Threading.Tasks;

namespace People
{
	public class PersonRepository
	{
		private SQLiteAsyncConnection dbConn;

		public string StatusMessage { get; set; }

		public PersonRepository(ISQLitePlatform sqlitePlatform, string dbPath)
		{
			//initialize a new SQLiteConnection 
			if (dbConn == null)
			{
				var connectionFunc = new Func<SQLiteConnectionWithLock>(() =>
					new SQLiteConnectionWithLock
					(
						sqlitePlatform,
						new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: false)
					));

				dbConn = new SQLiteAsyncConnection(connectionFunc);
				dbConn.CreateTableAsync<Person>();
			}
		}

		public async Task AddNewPersonAsync(string name)
		{
			int result = 0;
			try
			{
				//basic validation to ensure a name was entered
				if (string.IsNullOrEmpty(name))
					throw new Exception("Valid name required");

				//insert a new person into the Person table
				result = await dbConn.InsertAsync(new Person { Name = name });
				StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, name);
			}
			catch (Exception ex)
			{
				StatusMessage = string.Format("Failed to add {0}. Error: {1}", name, ex.Message);
			}

		}

		public async Task<List<Person>> GetAllPeopleAsync()
		{
			//return a list of people saved to the Person table in the database
			List<Person> people = await dbConn.Table<Person>().ToListAsync();
			return people;
		}
	}
}
using People.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace People
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();            
        }

		public async void OnNewButtonClicked(object sender, EventArgs args)
		{
			statusMessage.Text = "";

			await App.PersonRepo.AddNewPersonAsync(newPerson.Text);
			statusMessage.Text = App.PersonRepo.StatusMessage;
		}

		public async void OnGetButtonClicked(object sender, EventArgs args)
		{
			statusMessage.Text = "";

			List<Person> pplList = await App.PersonRepo.GetAllPeopleAsync();

			ObservableCollection<Person> pplCollection = new ObservableCollection<Person>(pplList);
			peopleList.ItemsSource = pplCollection;
		}
    }
}
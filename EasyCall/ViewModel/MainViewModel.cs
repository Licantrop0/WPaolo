using GalaSoft.MvvmLight;
using EasyCall1.Model;
using Microsoft.Phone.UserData;
using System;
using System.Collections.ObjectModel;

namespace EasyCall1.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDataService dataService)
        {
            cons = new Contacts();

            //Identify the method that runs after the asynchronous search completes.
            cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(cons_SearchCompleted);
        }
        Contacts cons;

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                //Start the asynchronous search.
                cons.SearchAsync(value, FilterKind.DisplayName, string.Empty);
            }
        }


        void cons_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            SearchedContacts = new ObservableCollection<Contact>();
            foreach (var contact in e.Results)
            {
                SearchedContacts.Add(contact);
            }
            RaisePropertyChanged("SearchedContacts");
        }

        public ObservableCollection<Contact> SearchedContacts { get; set; }

    }
}
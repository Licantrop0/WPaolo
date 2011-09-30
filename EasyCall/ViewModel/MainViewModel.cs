using GalaSoft.MvvmLight;
using System.Linq;
using Microsoft.Phone.UserData;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EasyCall.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEnumerable<ContactViewModel> ContactsVM { get; set; }
        public ObservableCollection<ContactViewModel> SearchedContacts { get; set; }

        public MainViewModel()
        {
            if (!IsInDesignMode)
                LoadContacts();
        }

        private void LoadContacts()
        {
            Contacts cons = new Contacts();
            cons.SearchCompleted += (sender, e) =>
            {
              ContactsVM = e.Results.Select(c => new ContactViewModel(c.DisplayName,
                    c.PhoneNumbers.Select(n => n.PhoneNumber)));
            };
            cons.SearchAsync(string.Empty, FilterKind.None, string.Empty);
        }


        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                Filter(value);
            }
        }


        private void Filter(string searchedText)
        {
            SearchedContacts = new ObservableCollection<ContactViewModel>();

            var contacts = ContactsVM.Where(c => c.NumberRepresentation.StartsWith(searchedText));
                
                
                //(from c in Contacts
                //           from n in c.Numbers
                //           where n.StartsWith(searchedText)
                //           select c).Union(Contacts.Where(c=> c.NumberRepresentation.StartsWith(searchedText)));

            foreach (var contact in contacts)
            {
                SearchedContacts.Add(contact);
            }
            RaisePropertyChanged("SearchedContacts");
        }

    }
}
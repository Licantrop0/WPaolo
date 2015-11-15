using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using WPCommon.Helpers;

namespace EasyCall.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private List<ContactViewModel> ContactsVM { get; set; }
        public List<ContactViewModel> SearchedContacts { get; set; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                Filter(value);
            }
        }

        public MainViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
                SearchedContacts = new List<ContactViewModel>
                {
                    new ContactViewModel("Luca Spolidoro", new[] { new NumberViewModel("393 3714189", ""), new NumberViewModel("010 311540", "") }, null),
                    new ContactViewModel("Pino Quercia", new[] { new NumberViewModel("+39 384 30572194", ""), new NumberViewModel("+39 02 365688", ""), new NumberViewModel("02 074 234456", "") }, null),
                    new ContactViewModel("Sapporo Piloro", new[] { new NumberViewModel("347 5840382", "") }, null)
                };
            else
            {
                LoadContacts();
            }
        }

        private async void LoadContacts()
        {
            var cs = await ContactManager.RequestStoreAsync();
            var allContacts = await cs.FindContactsAsync();

            ContactsVM = allContacts
                .Where(c => c.Phones.Any())
                .Select(c => new ContactViewModel(
                    c.DisplayName,
                    c.Phones.Select(p => new NumberViewModel(p.Number, c.DisplayName)),
                    c.Thumbnail))
                .ToList();

            Filter(SearchText);
        }


        private void Filter(string searchedText)
        {
            if (ContactsVM == null)
                return;

            SearchedContacts = string.IsNullOrEmpty(searchedText)
                ? ContactsVM
                : ContactsVM.Where(contact => contact.NumberRepresentation.Any(nr => nr.StartsWith(searchedText)) ||
                                              contact.Any(n => n.Number.Contains(searchedText))).ToList();

            RaisePropertyChanged("SearchedContacts");
        }
    }
}
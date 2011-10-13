using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Phone.UserData;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System;
using System.Windows.Threading;
using System.Windows;

namespace EasyCall.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private IList<Model.Contact> Contacts { get; set; }
        public List<ContactViewModel> SearchedContacts { get; set; }

        public MainViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
                SearchedContacts = new List<ContactViewModel>()
                {
                    new ContactViewModel(new Model.Contact("Luca Spolidoro", new[] {"393 3714189", "010 311540"}, null), ""),
                    new ContactViewModel(new Model.Contact("Pino Quercia", new[] {"+39 384 30572194", "+39 02 365688", "02 074 234456"}, null), ""),
                    new ContactViewModel(new Model.Contact("Sapporo Piloro", new[] {"347 5840382"}, null), "")
                };
            else
            {
                LoadContacts();
            }
        }

        private void LoadContacts()
        {
            SearchedContacts = new List<ContactViewModel>();
            var cons = new Contacts();
            cons.SearchCompleted += (sender, e) =>
            {
                //Da rendere Async
                Contacts = (from c in e.Results
                              where c.PhoneNumbers.Any()
                              select new Model.Contact(c.DisplayName,
                                  c.PhoneNumbers.Select(n => n.PhoneNumber),
                                  c.GetPicture())
                             ).ToList();

                if (!string.IsNullOrEmpty(SearchText) &&
                    !SearchedContacts.Any())
                    Filter(SearchText);
            };

            cons.SearchAsync(string.Empty, FilterKind.None, null);
        }

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

        private void Filter(string searchedText)
        {
            if (Contacts == null)
                return;

            if (string.IsNullOrEmpty(searchedText))
            {
                SearchedContacts = new List<ContactViewModel>();
                RaisePropertyChanged("SearchedContacts");
                return;
            }

            new Thread(() =>
            {
                SearchedContacts = (from contact in Contacts
                                    where contact.ContainsName(searchedText) ||
                                          contact.ContainsNumber(searchedText)
                                    select new ContactViewModel(contact, searchedText)
                                    ).ToList();

               Deployment.Current.Dispatcher.BeginInvoke(() => RaisePropertyChanged("SearchedContacts"));
            }).Start();
        }
    }
}
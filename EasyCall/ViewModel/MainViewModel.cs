using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Phone.UserData;
using System.Collections.Generic;

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

        private ContactViewModel[] ContactsVM { get; set; }
        public IEnumerable<ContactViewModel> SearchedContacts { get; set; }

        public MainViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
                SearchedContacts = new[]
                {
                    new ContactViewModel("Luca Spolidoro", new[] {"393 3714189", "010 311540"}, null),
                    new ContactViewModel("Pino Quercia", new[] {"+39 384 30572194", "+39 02 365688", "02 074 234456"}, null),
                    new ContactViewModel("Sapporo Piloro", new[] {"347 5840382"}, null)
                };
            else
            {
                LoadContacts();
            }
        }

        private void LoadContacts()
        {
            SearchedContacts = new ObservableCollection<ContactViewModel>();
            var cons = new Contacts();
            cons.SearchCompleted += (sender, e) =>
            {
                //Da rendere Async
                ContactsVM = (from c in e.Results
                              where c.PhoneNumbers.Any()
                              select new ContactViewModel(
                                  c.DisplayName,
                                  c.PhoneNumbers.Select(n => n.PhoneNumber),
                                  c.GetPicture())
                             ).ToArray();

                if (!string.IsNullOrEmpty(SearchText) &&
                    !SearchedContacts.Any())
                    Filter(SearchText);
            };
            cons.SearchAsync(string.Empty, FilterKind.None, null);
        }

        private string _searchText;
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
            if (ContactsVM == null)
                return;

            if (string.IsNullOrEmpty(searchedText))
            {
                SearchedContacts = null;
                RaisePropertyChanged("SearchedContacts");
                return;
            }

            //Da rendere Async
            SearchedContacts = from contact in ContactsVM
                               where contact.NumberRepresentation.Any(nr => nr.StartsWith(searchedText)) ||
                                     contact.Numbers.Any(n => n.Contains(searchedText))
                               select contact;
 

            RaisePropertyChanged("SearchedContacts");
        }
    }
}
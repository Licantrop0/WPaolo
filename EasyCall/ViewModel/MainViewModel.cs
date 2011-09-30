using GalaSoft.MvvmLight;
using System.Linq;
using Microsoft.Phone.UserData;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;

namespace EasyCall.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEnumerable<ContactViewModel> ContactsVM { get; set; }
        public ObservableCollection<ContactViewModel> SearchedContacts { get; set; }
        PhoneCallTask CallTask;        

        public MainViewModel()
        {
            if (IsInDesignMode)
                SearchedContacts = new ObservableCollection<ContactViewModel>(new[]
                {
                    new ContactViewModel("Luca Spolidoro", new[] {"393 3714189", "010 311540"}),
                    new ContactViewModel("Pino Quercia", new[] {"+39 384 30572194", "+39 02 365688"}),
                    new ContactViewModel("Sapporo Piloro", new[] {"347 5840382", "02 7039 2650"})
                });
            else
            {
                LoadContacts();
                CallTask = new PhoneCallTask();
            }
        }

        private void LoadContacts()
        {
            SearchedContacts = new ObservableCollection<ContactViewModel>();
            var cons = new Contacts();
            cons.SearchCompleted += (sender, e) =>
            {
                ContactsVM = from c in e.Results
                             where c.PhoneNumbers.Any()
                             select new ContactViewModel(c.DisplayName,
                                 c.PhoneNumbers.Select(n => n.PhoneNumber));
            };
            cons.SearchAsync(string.Empty, FilterKind.None, null);
        }

        public void Call(string name, string number)
        {
            CallTask.DisplayName = name;
            CallTask.PhoneNumber = number;
            CallTask.Show();
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
            if (string.IsNullOrEmpty(searchedText))
            {
                SearchedContacts.Clear();
                RaisePropertyChanged("SearchedContacts");
                return;
            }

            if (ContactsVM == null)
                return;

            SearchedContacts = new ObservableCollection<ContactViewModel>(
                from c in ContactsVM
                where c.NumberRepresentation.Any(nr => nr.StartsWith(searchedText)) ||
                c.Numbers.Any(n => n.Contains(searchedText))
                select c);

            RaisePropertyChanged("SearchedContacts");
        }

    }
}
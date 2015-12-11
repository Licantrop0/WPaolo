using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Storage;
using WPCommon.Helpers;

namespace EasyCall.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private const string FileName = "contacts.db";

        private IList<ContactViewModel> _contacts;
        public IList<ContactViewModel> SearchedContacts { get; private set; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                FilterAsync();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
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
            IsBusy = true;
            _contacts = await ReadAsync();
            FilterAsync();
            await UpdateContactsAsync();
            IsBusy = false;
        }

        private async Task UpdateContactsAsync()
        {
            var cs = await ContactManager.RequestStoreAsync();
            IReadOnlyList<Contact> contactsFromCs =
                (await cs.FindContactsAsync())
                .Where(c => c.Phones.Any())
                .ToList();

            //Update only if sequence is different
            if (_contacts == null || !AreEquals(_contacts, contactsFromCs))
            {
                _contacts = contactsFromCs
                    .Select(c => new ContactViewModel(
                        c.DisplayName,
                        c.Phones.Select(p => new NumberViewModel(p.Number, c.DisplayName)),
                        c.Thumbnail)).ToList();
                FilterAsync();
                WriteAsync(_contacts);
            }
        }

        private static bool AreEquals(IList<ContactViewModel> contacts1, IReadOnlyList<Contact> contacts2)
        {
            if (contacts1.Count != contacts2.Count)
                return false;

            for (var i = 0; i < contacts1.Count; i++)
            {
                var c1 = contacts1[i];
                var c2 = contacts2[i];

                if (c1.Name != c2.DisplayName)
                    return false;

                for (var j = 0; j < c1.Numbers.Count; j++)
                {
                    if (c1.Numbers[j].Number != c2.Phones[j].Number)
                        return false;
                }
            }

            return true;
        }


        private static async Task<List<ContactViewModel>> ReadAsync()
        {
            try { await ApplicationData.Current.LocalFolder.GetFileAsync(FileName); }
            catch (FileNotFoundException) { return null; }

            var serializer = new DataContractJsonSerializer(typeof(List<ContactViewModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(FileName))
            {
                return (List<ContactViewModel>)serializer.ReadObject(stream);
            }
        }

        private static async void WriteAsync(IEnumerable<ContactViewModel> data)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<ContactViewModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                FileName, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, data);
            }
        }

        private async void FilterAsync()
        {
            if (_contacts == null)
                return;

            await Task.Run(() =>
            {
                SearchedContacts = string.IsNullOrEmpty(SearchText)
                    ? _contacts
                    : _contacts.Where(contact =>
                        contact.NumberRepresentation.Any(nr => nr.StartsWith(SearchText)) ||
                        contact.Any(n => n.Number.Contains(SearchText)))
                        .ToList();
            });

            RaisePropertyChanged("SearchedContacts");
        }
    }
}
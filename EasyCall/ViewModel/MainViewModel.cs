using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        private List<ContactViewModel> _contacts;
        private List<ContactViewModel> _searchedContacts;
        public List<ContactViewModel> SearchedContacts
        {
            get { return _searchedContacts; }
            set
            {
                _searchedContacts = value;
                RaisePropertyChanged("SearchedContacts");
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                Filter();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
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
            Filter();

            await UpdateContacts();
            IsBusy = false;
        }

        private async Task UpdateContacts()
        {
            var cs = await ContactManager.RequestStoreAsync();
            var contactsFromCs = await cs.FindContactsAsync();

            //Update only if sequence is different
            if (_contacts == null || !AreEquals(_contacts, contactsFromCs))
            {
                _contacts = contactsFromCs
                .Where(c => c.Phones.Any())
                .Select(c => new ContactViewModel(
                    c.DisplayName,
                    c.Phones.Select(p => new NumberViewModel(p.Number, c.DisplayName)),
                    c.Thumbnail)).ToList();
                Filter();
                WriteAsync(_contacts);
            }
        }

        private static bool AreEquals(IEnumerable<ContactViewModel> contacts1, IReadOnlyList<Contact> contacts2)
        {
            return contacts1.Where((t, i) =>
                t.Name != contacts2[i].DisplayName ||
                !t.Numbers.Select(n => n.Number).SequenceEqual(contacts2[i].Phones.Select(n => n.Number))
                ).Any();
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

        private void Filter()
        {
            if (_contacts == null)
                return;

            if (string.IsNullOrEmpty(SearchText))
            {
                SearchedContacts = _contacts;
            }
            else
            {
                SearchedContacts = _contacts.Where(contact =>
                    contact.NumberRepresentation.Any(nr => nr.StartsWith(SearchText)) ||
                    contact.Any(n => n.Number.Contains(SearchText)))
                    .ToList();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        private const string FileName = "contacts.db";

        public double ScrollPosition
        {
            get { return _scrollPosition; }
            set
            {
                _scrollPosition = value;
                RaisePropertyChanged("ScrollPosition");
            }
        }

        private List<ContactViewModel> _contacts;

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
        private List<ContactViewModel> _searchedContacts;
        private bool _isBusy;
        private double _scrollPosition;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

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

            var cs = await ContactManager.RequestStoreAsync();
            var allContacts = await cs.FindContactsAsync();

            _contacts = allContacts
                .Where(c => c.Phones.Any())
                .Select(c => new ContactViewModel(
                    c.DisplayName,
                    c.Phones.Select(p => new NumberViewModel(p.Number, c.DisplayName)),
                    c.Thumbnail))
                .ToList();

            WriteAsync(_contacts);

            Filter();

            IsBusy = false;
        }

        private static async Task<List<ContactViewModel>> ReadAsync()
        {
            try { await LocalFolder.GetFileAsync(FileName); }
            catch (FileNotFoundException) { return null; }

            var serializer = new DataContractJsonSerializer(typeof(List<ContactViewModel>));
            using (var stream = await LocalFolder.OpenStreamForReadAsync(FileName))
            {
                return (List<ContactViewModel>)serializer.ReadObject(stream);
            }
        }

        private static async void WriteAsync(List<ContactViewModel> data)
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
                SearchedContacts =
                    _contacts.Where(contact => contact.NumberRepresentation.Any(nr => nr.StartsWith(SearchText)) ||
                                               contact.Any(n => n.Number.Contains(SearchText))).ToList();
            }
        }
    }
}
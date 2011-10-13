using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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

        public MainViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                SearchedContacts = AppContext.GetFakeContacts();
            }
        }

        private IList<ContactViewModel> _searchedContacts;
        public IList<ContactViewModel> SearchedContacts
        {
            get { return _searchedContacts; }
            set
            {
                if (_searchedContacts == value) return;
                _searchedContacts = value;

                Deployment.Current.Dispatcher.BeginInvoke(() => RaisePropertyChanged("SearchedContacts"));           
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

                new Thread(() => { SearchedContacts =  AppContext.Filter(value); }).Start();
            }
        }

    }
}
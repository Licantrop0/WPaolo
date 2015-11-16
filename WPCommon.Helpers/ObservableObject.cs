using System.ComponentModel;
using System.Runtime.Serialization;

namespace WPCommon.Helpers
{
    [DataContract]
    public class ObservableObject : INotifyPropertyChanged
    {
        private INavigationService _navigationService;
        protected INavigationService Ns => _navigationService ?? (_navigationService = new NavigationService());

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
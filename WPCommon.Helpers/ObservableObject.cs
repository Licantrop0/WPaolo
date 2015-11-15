using System.ComponentModel;

namespace WPCommon.Helpers
{
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
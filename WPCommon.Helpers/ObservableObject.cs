using System.ComponentModel;

namespace WPCommon.Helpers
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private INavigationService _ns;
        protected INavigationService NS
        {
            get
            {
                if (_ns == null)
                    _ns = new NavigationService();
                return _ns;
            }
        }
    }
}

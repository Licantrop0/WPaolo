using Scudetti.Model;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using NascondiChiappe.Helpers;
using System.Windows;

namespace Scudetti.ViewModel
{
    public class ShieldViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

        private Shield _currentShield;
        public Shield CurrentShield
        {
            get { return _currentShield; }
            set
            {
                if (CurrentShield == value)
                    return;
                _currentShield = value;
                RaisePropertyChanged("CurrentShield");
            }
        }

        private string _shieldName = string.Empty;
        public string ShieldName
        {
            get { return _shieldName; }
            set
            {
                _shieldName = value;
            }
        }

        public ShieldViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            Messenger.Default.Register<Shield>(this, (m) =>
                CurrentShield = m);
        }

        public void Validate()
        {
            switch (string.Compare(CurrentShield.Name, ShieldName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                case 0:
                    CurrentShield.IsValidated = true;
                    NavigationService.GoBack();
                    break;
                case 1:
                    MessageBox.Show("quasi!");
                    break;
                default:
                    MessageBox.Show("suca!");
                    break;
            }
        }
    }
}


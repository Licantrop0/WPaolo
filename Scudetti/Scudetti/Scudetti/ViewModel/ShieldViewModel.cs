using Scudetti.Model;
using System.Linq;
using GalaSoft.MvvmLight;
using NascondiChiappe.Helpers;
using System.Windows;

namespace Scudetti.ViewModel
{
    public class ShieldViewModel : ViewModelBase
    {
        //public INavigationService NavigationService { get; set; }

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

        public string ShieldName { get; set; }

        public ShieldViewModel()//INavigationService navigationService)
        {
            //NavigationService = navigationService;
            ShieldName = string.Empty;
        }

        public void Validate()
        {
            switch (string.Compare(CurrentShield.Name, ShieldName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                case 0:
                    CurrentShield.IsValidated = true;
                    //NavigationService.GoBack();
                    MessengerInstance.Send<string>("goback", "navigation");

                    MessengerInstance.Send<string>("UpdateProgress");
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


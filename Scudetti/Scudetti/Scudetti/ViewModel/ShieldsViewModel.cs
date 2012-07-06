using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using System.Linq;
using NascondiChiappe.Helpers;
using System;

namespace Scudetti.ViewModel
{
    public class ShieldsViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

        public string Livello { get { return "Livello " + Shields.First().Level; } }
        public string Progress
        {
            get
            {
                return "Scudetti " + Shields.Count(s => s.IsValidated) + " / " + Shields.Count();
            }
        }

        public IEnumerable<Shield> Shields { get { return AppContext.Shields; } }

        public Shield SelectedShield
        {
            get { return null; }
            set
            {
                NavigationService.Navigate(new Uri("/View/ShieldPage.xaml", UriKind.Relative));
                MessengerInstance.Send<Shield>(value);
                RaisePropertyChanged("SelectedShield");
            }
        }

        public ShieldsViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            MessengerInstance.Register<String>(this, (m) =>
            {
                if (m == "UpdateScudetti")
                    RaisePropertyChanged("Progress");
            });
        }
    }
}

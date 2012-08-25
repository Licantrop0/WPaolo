using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Messaging;

namespace SocceramaWin8.ViewModel
{
    public class LevelsViewModel : ViewModelBase
    {
        public IEnumerable<IGrouping<int, Shield>> Levels { get; set; }
        public IGrouping<int, Shield> SelectedLevel
        {
            get { return null; }
            set
            {
                MessengerInstance.Send<IGrouping<int, Shield>>(value);
                RaisePropertyChanged("SelectedLevel");
            }
        }
        public LevelsViewModel()
        {
            if (!IsInDesignMode)
            {
                LoadData();
            }
        }

        async void LoadData()
        {
            var shields = await ShieldService.Load();
            Levels = shields.GroupBy(s => s.Level);
            RaisePropertyChanged("Levels");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Scudetti.Model;

namespace SocceramaWin8.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        private IEnumerable<Shield> _shields;
        public IEnumerable<Shield> Shields
        {
            get { return _shields; }
            set
            {
                _shields = value;
                RaisePropertyChanged("Shields");
            }
        }

        public LevelViewModel()
        {
            if (!IsInDesignMode)
            {
                LoadData();
            }
        }

        async void LoadData()
        {
            Shields = await ShieldService.Load();
        }
    }
}

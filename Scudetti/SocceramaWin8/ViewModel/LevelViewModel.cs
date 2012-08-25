using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using Windows.UI.Xaml.Data;

namespace SocceramaWin8.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {

        //private IEnumerable<IGrouping<int, Shield>> _levels;
        //public IEnumerable<IGrouping<int, Shield>> Levels
        //{
        //    get { return _levels; }
        //    set
        //    {
        //        _levels = value;
        //        RaisePropertyChanged("Levels");
        //    }
        //}

        //private IEnumerable<Shield> _shields;
        //public IEnumerable<Shield> Shields
        //{
        //    get { return _shields; }
        //    set
        //    {
        //        _shields = value;
        //        RaisePropertyChanged("Shields");
        //    }
        //}

        public CollectionViewSource cvs { get; set; }

        public LevelViewModel()
        {
            if (!IsInDesignMode)
            {
                LoadData();
            }
        }

        async void LoadData()
        {
            var shields = await ShieldService.Load();
            cvs = new CollectionViewSource()
            {
                IsSourceGrouped = true,
                Source =  shields.GroupBy(s => s.Level)
            };
            RaisePropertyChanged("cvs");
        }
    }
}

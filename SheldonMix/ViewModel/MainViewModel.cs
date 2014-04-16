using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WPCommon.Helpers;

namespace SheldonMix.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private IList<SoundViewModel> _suoniCLAS;
        public IList<SoundViewModel> SuoniCLAS
        {
            get
            {
                if (AppContext.AllSound == null) return null;
                if (_suoniCLAS == null)
                    _suoniCLAS = AppContext.AllSound
                        .Where(s => s.Lang == AppContext.Lang)
                        .Where(s => s.Category == SoundCategory.CLAS)
                        .ToList();
                return _suoniCLAS;
            }
        }

        private IList<SoundViewModel> _suoniZAZZ;
        public IList<SoundViewModel> SuoniZAZZ
        {
            get
            {
                if (AppContext.AllSound == null) return null;
                if (_suoniZAZZ == null)
                    _suoniZAZZ = AppContext.AllSound
                        .Where(s => s.Lang == AppContext.Lang)
                        .Where(s => s.Category == SoundCategory.ZAZZ)
                        .ToList();
                return _suoniZAZZ;
            }
        }

        private IList<SoundViewModel> _suoniTBBT;
        public IList<SoundViewModel> SuoniTBBT
        {
            get
            {
                if (AppContext.AllSound == null) return null;
                if (_suoniTBBT == null)
                    _suoniTBBT = AppContext.AllSound
                        .Where(s => s.Lang == AppContext.Lang)
                        .Where(s => s.Category == SoundCategory.TBBT)
                        .ToList();
                return _suoniTBBT;
            }
        }
    }
}
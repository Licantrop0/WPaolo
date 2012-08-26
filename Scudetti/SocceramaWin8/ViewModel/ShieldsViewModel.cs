﻿using GalaSoft.MvvmLight;
using Scudetti.Model;
using Windows.ApplicationModel.Resources;

namespace SocceramaWin8.ViewModel
{
    public class ShieldsViewModel : ViewModelBase
    {
        ResourceLoader resources = new ResourceLoader();

        private LevelViewModel _selectedLevel;
        public LevelViewModel SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                if (SelectedLevel == value) return;
                _selectedLevel = value;
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public string LevelName
        {
            get
            {
                if (SelectedLevel == null) return string.Empty;
                return SelectedLevel.IsBonus ?
                    string.Format("{0} {1}", resources.GetString("BonusLevel"), SelectedLevel.Number / 100) :
                    string.Format("{0} {1}", resources.GetString("Level"), SelectedLevel.Number);
            }
        }

        public ShieldsViewModel()
        {
            MessengerInstance.Register<LevelViewModel>(this, m =>
            {
                SelectedLevel = m;
            });
        }
    }
}

﻿using System;
using System.Windows.Media.Imaging;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Scudetti.Localization;
using Scudetti.Sound;
using System.Linq;

namespace Scudetti.View
{
    public partial class ShieldsPage : PhoneApplicationPage
    {
        public ShieldsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var levelIndex = int.Parse(NavigationContext.QueryString["level"]);
            LayoutRoot.DataContext = AppContext.Levels.Single(l => l.Number == levelIndex);

            var newLevelTreshold = AppContext.TotalShieldUnlocked % AppContext.LockTreshold;
            int levelNumber = AppContext.TotalShieldUnlocked / AppContext.LockTreshold;

            if(e.NavigationMode != NavigationMode.Back) return;
            if(AppContext.TotalShieldUnlocked == 0) return;

            if (newLevelTreshold == 0 && levelNumber < 6)
            {
                SoundManager.PlayGoal();
                new ToastPrompt
                {
                    Message = string.Format(AppResources.NewLevel, levelNumber + 1),
                    ImageSource = new BitmapImage(new Uri("..\\Images\\soccer icon.png", UriKind.Relative))
                }.Show();
            }

            base.OnNavigatedTo(e);
        }
    }
}
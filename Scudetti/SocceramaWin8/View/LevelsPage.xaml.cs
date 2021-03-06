﻿using Callisto.Controls;
using SocceramaWin8.Common;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SocceramaWin8.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LevelsPage : LayoutAwarePage
    {
        public LevelsPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += LevelsPage_CommandsRequested;
        }

        void LevelsPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            var logoutCmd = new SettingsCommand("settings", "Settings", (cmd) =>
            {
                new SettingsFlyout()
                {
                    HeaderText = "Settings",
                    ContentBackgroundBrush = new SolidColorBrush(Colors.Black),
                    Content = new SettingsControl(),
                    IsOpen = true
                };
            });
            args.Request.ApplicationCommands.Add(logoutCmd);

            var aboutCmd = new SettingsCommand("about", "About", (cmd) =>
            {
                new SettingsFlyout()
                {
                    HeaderText = "About",
                    ContentBackgroundBrush = new SolidColorBrush(Colors.Black),
                    Content = new AboutControl(),
                    IsOpen = true
                };
            });
            args.Request.ApplicationCommands.Add(aboutCmd);
        }
    }
}

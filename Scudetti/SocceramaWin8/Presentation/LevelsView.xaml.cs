using SocceramaWin8.Common;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SocceramaWin8.Presentation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LevelsView : LayoutAwarePage
    {
        public LevelsView()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += LevelsPage_CommandsRequested;
            //this.ApplicationViewStates.CurrentStateChanged += ApplicationViewStates_CurrentStateChanged;
        }

        void LevelsPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var logoutCmd = new SettingsCommand("settings", "Settings", (cmd) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new SettingsControl());
            });
            args.Request.ApplicationCommands.Add(logoutCmd);

            var aboutCmd = new SettingsCommand("about", "About", (cmd) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new AboutControl());
            });
            args.Request.ApplicationCommands.Add(aboutCmd);
        }

        private void ApplicationViewStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (ApplicationView.Value == ApplicationViewState.FullScreenPortrait)
            {
                itemGridView.Width = 1080;
                itemGridView.Height = 1920;
                ((RotateTransform)itemGridView.RenderTransform).Angle = 90;
            }
            else
            {
                itemGridView.Width = 1920;
                itemGridView.Height = 1080;
                ((RotateTransform)itemGridView.RenderTransform).Angle = 0;
            }
        }
    }
}

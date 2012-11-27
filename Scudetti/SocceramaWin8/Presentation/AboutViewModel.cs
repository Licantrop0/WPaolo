using Radical.Windows.Presentation.Conventions.Settings;
using Radical.Windows.Presentation.Conventions.Settings.ComponentModel;
using Topics.Radical.Windows.Presentation;
using Windows.UI;
using Windows.UI.Xaml.Media;


namespace SocceramaWin8.Presentation
{
    [SettingDescriptor(CommandId = "about", CommandLabel = "About")]
    class AboutViewModel : AbstractViewModel, ISettingsViewModel
    {
        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"]; }
        }

        public SolidColorBrush HeaderBrush
        {
            get { return new SolidColorBrush(Colors.White); }
        }

        public string HeaderText
        {
            get { return "About"; }
        }
    }
}

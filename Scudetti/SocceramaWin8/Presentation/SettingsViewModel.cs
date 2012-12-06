using Radical.Windows.Presentation.Conventions.Settings;
using Radical.Windows.Presentation.Conventions.Settings.ComponentModel;
using System;
using System.Windows.Input;
using Topics.Radical.Windows.Input;
using Topics.Radical.Windows.Presentation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;

namespace SocceramaWin8.Presentation
{
    [SettingDescriptor(CommandId = "Settings", CommandLabel = "Settings")]
    public class SettingsViewModel : AbstractViewModel, ISettingsViewModel
    {
        public bool SoundEnabled
        {
            get { return AppContext.SoundEnabled; }
            set { AppContext.SoundEnabled = value; }
        }

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
            get { return "Impostazioni"; }
        }

        private ICommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = DelegateCommand.Create().OnExecute(async (m) =>
                {
                    var d = new MessageDialog("Vuoi Cancellare tutto?"); //AppResources.ConfirmResetTitle);
                    d.Commands.Add(new UICommand("Yes", async (e) => await AppContext.ResetShields()));
                    d.Commands.Add(new UICommand("No"));
                    await d.ShowAsync();
                }));
            }
        }
    }
}

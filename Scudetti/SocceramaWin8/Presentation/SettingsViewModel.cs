using Radical.Windows.Presentation.Conventions.Settings;
using Radical.Windows.Presentation.Conventions.Settings.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //private RelayCommand _resetCommand;
        //public RelayCommand ResetCommand
        //{
        //    get
        //    {
        //        return _resetCommand ?? (_resetCommand = new RelayCommand(async () =>
        //            {
        //                var d = new MessageDialog("Vuoi Cancellare tutto?"); //AppResources.ConfirmResetTitle);
        //                d.Commands.Add(new UICommand("Yes", ResetAction));
        //                d.Commands.Add(new UICommand("No"));
        //                await d.ShowAsync();
        //            }));
        //    }
        //}

        private void ResetAction(IUICommand e)
        {
            AppContext.ResetShields();
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
    }
}

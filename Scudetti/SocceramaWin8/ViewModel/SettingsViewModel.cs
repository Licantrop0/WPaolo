using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;
using GalaSoft.MvvmLight;

namespace SocceramaWin8.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        ResourceLoader resources = new ResourceLoader();

        public bool SoundEnabled
        {
            get { return AppContext.SoundEnabled; }
            set { AppContext.SoundEnabled = value; }
        }

        private RelayCommand _resetCommand;
        public RelayCommand ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = new RelayCommand(() =>
                {
                    var d = new MessageDialog(resources.GetString("ConfirmReset"),
                        resources.GetString("ConfirmResetTitle"));
                    d.Commands.Add(new UICommand(resources.GetString("Reset"), ResetAction));
                    d.Commands.Add(new UICommand(resources.GetString("Cancel")));
                    d.ShowAsync();
                }));
            }
        }

        private async void ResetAction(IUICommand e)
        {
            await AppContext.ResetShields();
            MessengerInstance.Send<string>("", "reset");
            MessengerInstance.Send<LevelsViewModel>(null);
        }
    }
}

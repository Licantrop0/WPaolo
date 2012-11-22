using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SocceramaWin8.ViewModel
{
    public class SettingsViewModel
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
    }
}

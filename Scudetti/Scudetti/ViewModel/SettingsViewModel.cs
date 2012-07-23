
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Scudetti.Localization;

namespace Scudetti.ViewModel
{
    public class SettingsViewModel
    {
        public bool SoundEnabled
        {
            get { return AppContext.SoundEnabled; }
            set { AppContext.SoundEnabled = value; }
        }

        private RelayCommand _resetCommand;
        public RelayCommand ResetCommand
        {
            get { return _resetCommand ?? (_resetCommand = new RelayCommand(ResetAction)); }
        }
        private void ResetAction()
        {
            if (MessageBox.Show(AppResources.ConfirmReset, AppResources.ConfirmResetTitle,
                MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;

            AppContext.ResetShields();
        }
    }
}

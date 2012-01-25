using GalaSoft.MvvmLight;

namespace IDecide.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {

        public bool SoundEnabled
        {
            get { return AppContext.SoundEnabled; }
            set { AppContext.SoundEnabled = value; }
        }

        public bool VibrationEnabled
        {
            get { return AppContext.VibrationEnabled; }
            set { AppContext.VibrationEnabled = value; }
        }

    }
}
using GalaSoft.MvvmLight.Messaging;
using SocceramaWin8.Common;
using SocceramaWin8.ViewModel;
using Windows.UI.Xaml.Controls;

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
        }

        private void levelClick(object sender, ItemClickEventArgs e)
        {
            var level = (LevelViewModel)e.ClickedItem;
            if (level.IsUnlocked)
            {
                //SoundManager.PlayFischietto();
                Messenger.Default.Send<LevelViewModel>(level);
                this.Frame.Navigate(typeof(ShieldsPage));
            }
        }
    }
}

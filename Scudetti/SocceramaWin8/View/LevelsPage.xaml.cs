using GalaSoft.MvvmLight.Messaging;
using SocceramaWin8.Common;
using SocceramaWin8.ViewModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            this.ApplicationViewStates.CurrentStateChanged += ApplicationViewStates_CurrentStateChanged;
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

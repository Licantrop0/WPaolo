using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace TouchColors.View
{
    public sealed partial class QuestionsPage : Page
    {
        public QuestionsPage()
        {
            this.InitializeComponent();
            HideStatusBar();
        }

        private async static void HideStatusBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
               await StatusBar.GetForCurrentView().HideAsync();
            }
        }
    }
}
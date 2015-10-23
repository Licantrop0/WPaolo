using Microsoft.Phone.Marketplace;
using System.Windows.Controls;
using System.Linq;
using System;

namespace WPCommon.Controls
{
    public partial class AdSwitcher : UserControl
    {
        public string MSAdId { get; set; }
        public string MSAppId { get; set; }
        public string ADAppId { get; set; }
        public Action<string> LoadingError;

        public AdSwitcher()
        {
            InitializeComponent();
        }

        public void AddAdvertising()
        {
            if (AdPlaceholder.Children.Any()) return;
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                MSAppId = "test_client";
                MSAdId = "Image480_80";
            }

#if debug 
                MSAppId = "test_client";
                MSAdId = "Image480_80";
#endif

            this.Height = 80;

            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var msad = new Microsoft.Advertising.Mobile.UI.AdControl(
                    MSAppId, MSAdId, true) { Width = 480, Height = 80 };
                msad.ErrorOccurred += msad_ErrorOccurred;
                AdPlaceholder.Children.Add(msad);
            });
        }

        private void msad_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            AdPlaceholder.Children.Clear();
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var adad = new AdDuplex.AdControl() { AdUnitId = ADAppId };
                adad.AdLoadingError += adad_AdLoadingError;
                AdPlaceholder.Children.Add(adad);
            });
        }

        void adad_AdLoadingError(object sender, AdDuplex.AdLoadingErrorEventArgs e)
        {
            this.Height = 0;
            if (LoadingError != null)
                LoadingError(e.Error.Message);
        }

        public void RemoveAdvertising()
        {
            AdPlaceholder.Children.Clear();
        }
    }
}

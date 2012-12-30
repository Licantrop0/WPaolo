using GalaSoft.MvvmLight.Messaging;
using SocceramaWin8.ViewModel;
using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;

namespace SocceramaWin8.Helpers
{
    public static class NotificationHelper
    {
        private static XmlDocument _toastTemplate;
        private static XmlDocument ToastTemplate
        {
            get
            {
                if (_toastTemplate == null)
                {
                    _toastTemplate = ToastNotificationManager.GetTemplateContent(
                        ToastTemplateType.ToastImageAndText01);

                    var toastImageElement = _toastTemplate.GetElementsByTagName("image");
                    ((XmlElement)toastImageElement[0]).SetAttribute("src", "ms-appx:///Assets/soccer icon.png");
                    ((XmlElement)toastImageElement[0]).SetAttribute("alt", "Soccer Icon");

                    var toastNode = _toastTemplate.SelectSingleNode("/toast");
                    var audio = _toastTemplate.CreateElement("audio");
                    audio.SetAttribute("silent", "true");
                    toastNode.AppendChild(audio);
                }
                return _toastTemplate;
            }
        }

        public static void DisplayToast(string text, LevelViewModel level)
        {
            var toastTextElements = ToastTemplate.GetElementsByTagName("text");
            toastTextElements[0].InnerText = "\n" + text;

            ToastNotification toast = new ToastNotification(ToastTemplate);

            //TODO: capire perchè non va
            //if (level != null)
            //{
            //    var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            //    toast.Activated += (sender, e) => dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //        {
            //            Messenger.Default.Send<LevelViewModel>(level);
            //        });
            //}
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private static XmlDocument _simpleWide;
        private static XmlDocument SimpleWide
        {
            get
            {
                if (_simpleWide == null)
                {
                    _simpleWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideImage);
                    var tileImageAttributes = _simpleWide.GetElementsByTagName("image");
                    ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Logos/WideLogo.png");
                    ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "Wide Logo");

                    // removes logo from lower left-hand corner of tile
                    var sqTileBinding = (XmlElement)_simpleWide.GetElementsByTagName("binding").Item(0);
                    sqTileBinding.SetAttribute("branding", "none");
                }
                return _simpleWide;
            }
        }

        private static XmlDocument _dynamicWide;
        private static XmlDocument DynamicWide
        {
            get
            {
                if (_dynamicWide == null)
                {
                    TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                    _dynamicWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideImageAndText01);

                    // removes logo from lower left-hand corner of tile
                    var sqTileBinding = (XmlElement)_dynamicWide.GetElementsByTagName("binding").Item(0);
                    sqTileBinding.SetAttribute("branding", "none");

                    var tileImageAttributes = _dynamicWide.GetElementsByTagName("image");
                    ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Logos/WideLogo2.png");
                    ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "Wide Logo 2");
                }
                return _dynamicWide;
            }
        }

        public static void UpdateTile(string text)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            var tileTextAttributes = DynamicWide.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = text;

            //Creation
            var tileNotification = new TileNotification(DynamicWide);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            tileNotification = new TileNotification(SimpleWide);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }
}
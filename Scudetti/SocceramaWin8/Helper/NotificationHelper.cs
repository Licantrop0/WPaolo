using GalaSoft.MvvmLight.Messaging;
using SocceramaWin8.ViewModel;
using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;

namespace SocceramaWin8.Helpers
{
    public static class NotificationHelper
    {
        public static void DisplayToast(string text, LevelViewModel level)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(
                ToastTemplateType.ToastImageAndText01);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("\n" + text));

            var toastImageElement = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageElement[0]).SetAttribute("src", "ms-appx:///Assets/soccer icon.png");
            ((XmlElement)toastImageElement[0]).SetAttribute("alt", "Soccer Icon");

            var toastNode = toastXml.SelectSingleNode("/toast");
            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("silent", "true");
            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);

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

        public static void UpdateTile(string text)        
        {
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideImageAndText01);
            var tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = text;

            var tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Logos/WideLogo2.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "Wide Logo 2");

            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }
}

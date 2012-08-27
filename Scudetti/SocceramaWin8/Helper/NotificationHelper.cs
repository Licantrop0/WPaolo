using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace SocceramaWin8.Helpers
{
    public static class NotificationHelper
    {
        public static void DisplayToast(string text)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(
                ToastTemplateType.ToastImageAndText01);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("\n" + text));

            var toastImageElement = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageElement[0]).SetAttribute("src", "ms-appx:///Assets/soccer icon.png");
            //((XmlElement)toastImageElement[0]).SetAttribute("alt", text);

            var toastNode = toastXml.SelectSingleNode("/toast");
            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("silent", "true");
            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}

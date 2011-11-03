using System;
using Microsoft.Phone.Tasks;

namespace iCub.Helpers
{
    public static class LinksHelper
    {
        public static void OpenUrl(Uri url)
        {
            new WebBrowserTask() { Uri = url }.Show();
        }

        public static void ComposeMail(string mail)
        {
            new EmailComposeTask() { To = mail }.Show();
        }
    }
}
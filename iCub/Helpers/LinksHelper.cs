using System;
using Microsoft.Phone.Tasks;

namespace iCub.Helpers
{
    public static class LinksHelper
    {
        public static void OpenUrl(Uri url)
        {
            try
            {
                new WebBrowserTask() { Uri = url }.Show();
            }
            catch (Exception)
            { }
        }

        public static void ComposeMail(string mail)
        {
            try
            {
                new EmailComposeTask() { To = mail }.Show();
            }
            catch (Exception)
            { }
        }
    }
}
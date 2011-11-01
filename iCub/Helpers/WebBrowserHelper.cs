using System;
using Microsoft.Phone.Tasks;

namespace iCub.Helpers
{
    public static class WebBrowserHelper
    {
        public static void Open(Uri url)
        {
            new WebBrowserTask() { Uri = url }.Show();
        }
    }
}
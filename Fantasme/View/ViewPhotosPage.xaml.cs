using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace NascondiChiappe
{
    public partial class ViewPhotosPage : PhoneApplicationPage
    {
        public ViewPhotosPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
                return;
            }

            if (NavigationContext.QueryString.ContainsKey("path"))
                CreateHtml(NavigationContext.QueryString["path"]);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Uri.OriginalString == "/View/PasswordPage.xaml")
                NavigationService.RemoveBackEntry();
        }

        public void CreateHtml(string path)
        {
            var html = new XDocument(
                new XElement("html",
                    new XElement("head",
                        new XElement("meta",
                            new XAttribute("name", "viewport"),
                            new XAttribute("content", "width=480,height=800")),
                        new XElement("body",
                            new XAttribute("style", "background-color:black"),
                            new XElement("img",
                                new XAttribute("src", path),
                                new XAttribute("width", "480"),
                                new XAttribute("style", "margin-top:auto; margin-bottom:auto;")
                                            )))));


            var isf = IsolatedStorageFile.GetUserStoreForApplication();
            using (var isfs = isf.OpenFile("image.html", FileMode.Create))
            {
                using (var sw = new StreamWriter(isfs))
                {
                    sw.Write(html);
                    sw.Close();
                }
                isfs.Close();
            }

            Wb.Navigate(new Uri("image.html", UriKind.Relative));
        }
    }
}
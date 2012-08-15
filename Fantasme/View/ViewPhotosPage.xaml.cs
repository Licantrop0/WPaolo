using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class ViewPhotosPage : PhoneApplicationPage
    {

        private static string html =
@"<html><head>
   <meta name='viewport' content='width=480,height=800' />
   <body style='background-color:black'>
     <img src='{0}' width='480' style='margin-top:auto; margin-bottom:auto;' />
   </body>
  </head></html>";

        private static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        private AlbumViewModel currentAlbum = null;
        private int _currentIndex;
        private int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                CreateHtml(currentAlbum.Photos[CurrentIndex].Path);
                UpdateAppBarStatus();
            }
        }

        public ViewPhotosPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
                return;
            }

            if (NavigationContext.QueryString.ContainsKey("album") &&
                NavigationContext.QueryString.ContainsKey("index"))
            {
                currentAlbum = AppContext.Albums.Single(a => a.Name == NavigationContext.QueryString["album"]);
                CurrentIndex = int.Parse(NavigationContext.QueryString["index"]);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Uri.OriginalString == "/View/PasswordPage.xaml")
                NavigationService.RemoveBackEntry();
        }

        public void CreateHtml(string path)
        {
            using (var isfs = isf.OpenFile("image.html", FileMode.Create))
            {
                using (var sw = new StreamWriter(isfs))
                {
                    sw.Write(string.Format(html, path));
                    sw.Close();
                }
                isfs.Close();
            }

            Wb.Navigate(new Uri("image.html", UriKind.Relative));

            #region old html generation
            //var html = new XDocument(
            //    new XElement("html",
            //        new XElement("head",
            //            new XElement("meta",
            //                new XAttribute("name", "viewport"),
            //                new XAttribute("content", "width=480,height=800")),
            //            new XElement("body",
            //                new XAttribute("style", "background-color:black"),
            //                new XElement("img",
            //                    new XAttribute("src", path),
            //                    new XAttribute("width", "480"),
            //                    new XAttribute("style", "margin-top:auto; margin-bottom:auto;")
            //                                )))));
            #endregion
        }

        #region AppBar Management

        private ApplicationBarIconButton PreviousAppBarButton;
        private ApplicationBarIconButton NextAppBarButton;
        private void InitializeApplicationBar()
        {
            PreviousAppBarButton = new ApplicationBarIconButton();
            PreviousAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_previous.png", UriKind.Relative);
            PreviousAppBarButton.Text = NascondiChiappe.Localization.AppResources.Previous;
            PreviousAppBarButton.Click += (sender, e) => CurrentIndex--;
            ApplicationBar.Buttons.Add(PreviousAppBarButton);

            NextAppBarButton = new ApplicationBarIconButton();
            NextAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_next.png", UriKind.Relative);
            NextAppBarButton.Text = NascondiChiappe.Localization.AppResources.Next;
            NextAppBarButton.Click += (sender, e) => CurrentIndex++;
            ApplicationBar.Buttons.Add(NextAppBarButton);
        }

        private void UpdateAppBarStatus()
        {
            PreviousAppBarButton.IsEnabled = CurrentIndex > 0;
            NextAppBarButton.IsEnabled = CurrentIndex < currentAlbum.Photos.Count - 1;
        }

        #endregion
    }
}
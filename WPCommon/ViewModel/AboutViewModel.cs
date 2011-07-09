using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media;
using System.Xml.Linq;
using WPCommon.Model;
using System.ComponentModel;
using WPCommon.Localization;

namespace WPCommon.ViewModel
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        XNamespace nsAtom = "http://www.w3.org/2005/Atom";
        XNamespace nsZune = "http://schemas.zune.net/catalog/apps/2008/02";
        string cultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;

        private IEnumerable<AppTile> _appList;
        public IEnumerable<AppTile> AppList
        {
            get { return _appList; }
        }

        public AboutViewModel()
        {
            InitializeWPMEApps();
        }

        private void InitializeWPMEApps()
        {
            var wc = new WebClient();
            wc.OpenReadAsync(new Uri(string.Format(
                 "http://catalog.zune.net/v3.2/{0}/apps?q=WPME&clientType=WinMobile%207.1&store=zest",
                 cultureName)));

            wc.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null) return;

                XDocument response = XDocument.Load(e.Result);
                _appList = from n in response.Descendants(nsAtom + "entry")
                          let imageId = n.Element(nsZune + "image")
                              .Element(nsZune + "id").Value.Substring(9) //rimozione di "urn:uuid:"
                          select new AppTile(
                              new Guid(n.Element(nsAtom + "id").Value.Substring(9)),
                              n.Element(nsAtom + "title").Value,
                              new Uri(string.Format(
                                  "http://image.catalog.zune.net/v3.2/{0}/image/{1}?width=200&height=200",
                                  cultureName, imageId)));
            };
        }

        #region App Data

        private string _appName = "Application Name";
        public string AppName
        {
            get { return _appName; }
            set
            {
                if (AppName == value) return;
                _appName = value;
                RaisePropertyChanged("AppName");
            }
        }

        //var name = Assembly.GetExecutingAssembly().FullName;
        //WPMEAbout.ApplicationVersion = new AssemblyName(name).Version.ToString(); 
        private string _appVersion = "1.0.0";
        public string AppVersion
        {
            get { return _appVersion; }
            set
            {
                if (AppVersion == value) return;
                _appVersion = value;
                RaisePropertyChanged("ApplicationVersion");
            }
        }
        #endregion

        #region Visual

        private ImageSource _customLogo;
        public ImageSource CustomLogo
        {
            get { return _customLogo; }
            set
            {
                if (CustomLogo == value) return;
                _customLogo = value;
                RaisePropertyChanged("CustomLogo");
            }
        }

        private Brush _backgroundStackPanel = new SolidColorBrush(Colors.Black);
        public Brush BackgroundStackPanel
        {
            get { return _backgroundStackPanel; }
            set
            {
                if (_backgroundStackPanel == value) return;
                _backgroundStackPanel = value;
                RaisePropertyChanged("BackgroundStackPanel");
            }
        }

        private FontFamily _defaultFont = new FontFamily("Segoe WP SemiLight");
        public FontFamily DefaultFont
        {
            get { return _defaultFont; }
            set
            {
                if (DefaultFont == value) return;
                _defaultFont = value;
                RaisePropertyChanged("DefaultFont");
            }
        }

        private double _minFontSize = 19;
        public double MinFontSize
        {
            get { return _minFontSize; }
            set
            {
                if (MinFontSize == value) return;
                _minFontSize = value;
                RaisePropertyChanged("MinFontSize");
                RaisePropertyChanged("AppNameFontSize");
                RaisePropertyChanged("AppVersionFontSize");
                RaisePropertyChanged("HyperLinkFontSize");
            }
        }

        public double AppNameFontSize
        { get { return MinFontSize * (32d / 19); } }

        public double AppVersionFontSize
        { get { return MinFontSize; } }

        public double HyperLinkFontSize
        { get { return MinFontSize * (24d / 19); } }

        #endregion

        #region Localization

        public string ApplicationText
        { get { return AppResources.Application; } }

        public string ContactUsText
        { get { return AppResources.ContactUs; } }

        public string GetOtherAppsText
        { get { return AppResources.GetOtherApps; } }

        public string OtherAppsText
        { get { return AppResources.OtherApps; } }

        public string RateText
        { get { return AppResources.RateThisApp; } }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

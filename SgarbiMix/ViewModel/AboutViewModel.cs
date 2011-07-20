using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media;
using System.Xml.Linq;
using WPCommon.Model;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;

namespace SgarbiMix.ViewModel
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
                if (_appList != null) return;

                XDocument response = XDocument.Load(e.Result);
                _appList = from n in response.Descendants(nsAtom + "entry")
                           let imageId = n.Element(nsZune + "image")
                               .Element(nsZune + "id").Value.Substring(9) //rimozione di "urn:uuid:"
                           let appId = n.Element(nsAtom + "id").Value.Substring(9)
                           where appId != AppId
                           select new AppTile(new Guid(appId), n.Element(nsAtom + "title").Value, new Uri(
                               string.Format("http://image.catalog.zune.net/v3.2/{0}/image/{1}?width=200&height=200",
                                   cultureName, imageId)));
            };
        }

        #region App Data

        public string AppName { get; set; }

        private string _appVersion;
        public string AppVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_appVersion))
                {
                    var name = Assembly.GetExecutingAssembly().FullName;
                    _appVersion = new AssemblyName(name).Version.ToString();
                }
                return _appVersion;
            }
        }

        //SET THIS VALUE TO THE MARKETPLACE Product ID (to filter out in the Ohter Apps List)
        public string AppId { get; set; }

        public string CustomText
        {
            get { return ""; }
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

        private Thickness _customLogoMargin = new Thickness(0);
        public Thickness CustomLogoMargin
        {
            get { return _customLogoMargin; }
            set { _customLogoMargin = value; }
        }


        private Brush _defaultBackground = new SolidColorBrush(Colors.Black);
        public Brush DefaultBackground
        {
            get { return _defaultBackground; }
            set
            {
                if (DefaultBackground == value) return;
                _defaultBackground = value;
                RaisePropertyChanged("DefaultBackground");
            }
        }

        private Brush _defaultForeground = new SolidColorBrush(Colors.White);
        public Brush DefaultForeground
        {
            get { return _defaultForeground; }
            set
            {
                if (DefaultForeground == value) return;
                _defaultForeground = value;
                RaisePropertyChanged("DefaultForeground");
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
        { get { return WPCommon.Localization.AppResources.Application; } }

        public string ContactUsText
        { get { return WPCommon.Localization.AppResources.ContactUs; } }

        public string GetOtherAppsText
        { get { return WPCommon.Localization.AppResources.GetOtherApps; } }

        public string OtherAppsText
        { get { return WPCommon.Localization.AppResources.OtherApps; } }

        public string RateText
        { get { return WPCommon.Localization.AppResources.RateThisApp; } }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

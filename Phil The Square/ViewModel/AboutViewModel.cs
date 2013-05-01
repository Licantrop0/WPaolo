using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using FillTheSquare.Localization;
using WPCommon.Controls.Model;

namespace FillTheSquare.ViewModel
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
            private set
            {
                _appList = value;
                RaisePropertyChanged("AppList");
            }
        }

        public AboutViewModel()
        {
            InitializeWPMEApps();
        }

        private void InitializeWPMEApps()
        {
            var wc = new WebClient();
            wc.OpenReadAsync(new Uri(string.Format(
                 "http://marketplaceedgeservice.windowsphone.com/v3.2/{0}/apps?q=WPME&clientType=WinMobile+7.1&store=zest",
                 cultureName)));

            wc.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null) return;
                if (AppList != null) return;

                XDocument response = XDocument.Load(e.Result);
                AppList = from n in response.Descendants(nsAtom + "entry")
                          let imageId = n.Element(nsZune + "image")
                              .Element(nsZune + "id").Value.Substring(9) //rimozione di "urn:uuid:"
                          let appId = n.Element(nsAtom + "id").Value.Substring(9)
                          where appId != AppId
                          select new AppTile(new Guid(appId), n.Element(nsAtom + "title").Value, new Uri(
                              string.Format("http://cdn.marketplaceimages.windowsphone.com/v3.2/{0}/image/{1}?width=200&height=200&resize=true&contenttype=image/png",
                                  cultureName, imageId)));
            };
        }

        #region App Data
        /// <summary>Set this value to the Marketplace Product ID</summary>
        public string AppId { get; set; }
        public string AppName
        {
            get { return AppResources.AppName; }
        }

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

        #endregion

        #region Visual

        public FontFamily DefaultFont { get; set; }

        public string CustomText { get; set; }

        private FontFamily _customTextFontFamily;
        public FontFamily CustomTextFontFamily
        {
            get { return _customTextFontFamily ?? DefaultFont; }
            set { _customTextFontFamily = value; }
        }

        private double? _customTextFontSize;
        public double CustomTextFontSize
        {
            get { return _customTextFontSize ?? MinFontSize; }
            set { _customTextFontSize = value; }
        }

        private Brush _customTextForeground;
        public Brush CustomTextForeground
        {
            get { return _customTextForeground ?? DefaultForeground ?? (Brush)Application.Current.Resources["PhoneForegroundBrush"]; }
            set { _customTextForeground = value; }
        }


        private Thickness _appNameMargin = new Thickness(0);
        public Thickness AppNameMargin
        {
            get { return _appNameMargin; }
            set { _appNameMargin = value; }
        }

        public ImageSource CustomLogo { get; set; }

        private Thickness _logoMargin = new Thickness(24);
        public Thickness LogoMargin
        {
            get { return _logoMargin; }
            set { _logoMargin = value; }
        }

        private Thickness _customLogoMargin = new Thickness(0);
        public Thickness CustomLogoMargin
        {
            get { return _customLogoMargin; }
            set { _customLogoMargin = value; }
        }

        public Brush DefaultBackground { get; set; }

        public Brush DefaultForeground { get; set; }

        private Brush _headerForeground;
        public Brush HeaderForeground
        {
            get { return _headerForeground ?? DefaultForeground ?? (Brush)Application.Current.Resources["PhoneForegroundBrush"]; }
            set { _headerForeground = value; }
        }

        private double _minFontSize = 19;
        public double MinFontSize
        {
            get { return _minFontSize; }
            set { _minFontSize = value; }
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
        { get { return WPCommon.Controls.Localization.AppResources.Application; } }

        public string ContactUsText
        { get { return WPCommon.Controls.Localization.AppResources.ContactUs; } }

        public string GetOtherAppsText
        { get { return AppResources.GetOtherApps; } }

        public string OtherAppsText
        { get { return WPCommon.Controls.Localization.AppResources.OtherApps; } }

        public string RateText
        { get { return WPCommon.Controls.Localization.AppResources.RateThisApp; } }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

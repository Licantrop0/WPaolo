using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media;
using System.Xml.Linq;
using WPCommon.Controls.Model;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace SheldonMix.ViewModel
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        private readonly string _countryName = System.Globalization.CultureInfo.CurrentUICulture.Name.Split('-')[1];

        private IList<AppTile> _appList;
        public IList<AppTile> AppList
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

        private async void InitializeWPMEApps()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(new Uri($"http://wpdevinfo.azurewebsites.net/api/wpdev/{_countryName}/WPME"));
            if (!response.IsSuccessStatusCode) return;
            var serializer = new DataContractJsonSerializer(typeof(AppList));
            var json = await response.Content.ReadAsStringAsync();
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                AppList = ((AppList)serializer.ReadObject(stream)).applications;
            }
        }

        #region App Data

        /// <summary>Set this value to the Marketplace Product ID</summary>
        public string AppId { get; set; }

        public string AppName { get; set; }
        //{
        //    get { return AppResources.AppName; }
        //}

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


        public Thickness AppNameMargin { get; set; } = new Thickness(0);

        public ImageSource CustomLogo { get; set; } = new BitmapImage(new Uri("/WPCommon.Controls;component/Img/logo.png", UriKind.Relative));

        public Thickness LogoMargin { get; set; } = new Thickness(24);

        public Brush DefaultBackground { get; set; }

        public Brush DefaultForeground { get; set; }

        private Brush _headerForeground;
        public Brush HeaderForeground
        {
            get { return _headerForeground ?? DefaultForeground ?? (Brush)Application.Current.Resources["PhoneForegroundBrush"]; }
            set { _headerForeground = value; }
        }

        public double MinFontSize { get; set; } = 19;

        public double AppNameFontSize => MinFontSize * (32d / 19);

        public double AppVersionFontSize => MinFontSize;

        public double HyperLinkFontSize => MinFontSize * (24d / 19);

        #endregion

        #region Localization

        public string ApplicationText => WPCommon.Controls.Localization.AppResources.Application;

        public string ContactUsText => WPCommon.Controls.Localization.AppResources.ContactUs;

        public string GetOtherAppsText => WPCommon.Controls.Localization.AppResources.GetOtherApps;

        public string OtherAppsText => WPCommon.Controls.Localization.AppResources.OtherApps;

        public string RateText => WPCommon.Controls.Localization.AppResources.RateThisApp;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

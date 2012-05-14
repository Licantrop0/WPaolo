using System;
using System.Device.Location;
using System.IO;
using System.Net;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace GeoWar
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string BING_MAPS_KEY = "AtC1WBJDHDCuaHKrh8v6RzJ-264eJ199X7MTfBYQY3Z8SFp_F2TNVDBBWIjDTS0K";
        public MainPage()
        {
            InitializeComponent();

            var geo = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            geo.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(geo_PositionChanged);
            geo.Start();
        }

        void geo_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var location = e.Position.Location;
            var reqstring = string.Format("http://dev.virtualearth.net/REST/v1/Locations/{0},{1}?includeEntityTypes={2}&key={3}",
                location.Latitude, 
                location.Longitude,
                string.Join(",", "Neighborhood", "PopulatedPlace", "Postcode1", "CountryRegion"),
                BING_MAPS_KEY);

            var webreq = (HttpWebRequest)HttpWebRequest.Create(reqstring);
            webreq.BeginGetResponse((result) =>
            {
                var response = webreq.EndGetResponse(result);
                var sr = new StreamReader(response.GetResponseStream());

                Dispatcher.BeginInvoke(() => GeoInfoTextBlock.Text = sr.ReadToEnd());
            }, null);


        }
    }
}
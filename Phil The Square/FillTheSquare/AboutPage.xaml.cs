using System.Reflection;
using Microsoft.Phone.Controls;
using FillTheSquare.Localization;
using Microsoft.Phone.Tasks;
using FillTheSquare.ViewModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace FillTheSquare
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Youtube_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new WebBrowserTask() { URL = "http://www.youtube.com/watch?v=6PBK-sg2Zr0" }.Show();
        }
    }
}
using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace FillTheSquare
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=6PBK-sg2Zr0") }.Show();
        }
    }
}
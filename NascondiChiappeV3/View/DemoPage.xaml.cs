using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace NascondiChiappe
{
    public partial class DemoPage : PhoneApplicationPage
    {
        public DemoPage()
        {
            InitializeComponent();
        }

        private void BuyAppClick(object sender, RoutedEventArgs e)
        {
            var detailTask = new MarketplaceDetailTask();
            detailTask.Show();
        }
    }
}
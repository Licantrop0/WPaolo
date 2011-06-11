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
using WPCommon;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;


namespace IamSuperRichI
{
    public partial class InstructionsPivotPage : PhoneApplicationPage
    {
        private string[] appId;

        public InstructionsPivotPage()
        {
            InitializeComponent();
            InitializeGrid();
        }

        void InitializeGrid()
        {
              for (int i = 0; i < 8; i++)
            {
                appLinksGrid.RowDefinitions.Add(new RowDefinition());
                appLinksGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var b = new Border();
                b.BorderThickness = new Thickness(0);
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("numbered/%28" + i + "%29.png", UriKind.Relative));
                b.Background = brush;
                b.Height = 200;
                b.Width = 200;
                b.SetRow(i/2);
                b.SetColumn(i%2);
                b.MouseLeftButtonDown += Image_Click;
                appLinksGrid.Children.Add(b);
            }

            appId = new string[8];
            appId[0] = "59073daf-bb2b-e011-854c-00237de2db9e";
            appId[1] = "a9fb8160-c22b-e011-854c-00237de2db9e";
            appId[2] = "4941be10-c32b-e011-854c-00237de2db9e";
            appId[3] = "29c30692-c42b-e011-854c-00237de2db9e";
            appId[4] = "9996581f-c52b-e011-854c-00237de2db9e";
            appId[5] = "d982f2a3-c52b-e011-854c-00237de2db9e";
            appId[6] = "a921e145-c62b-e011-854c-00237de2db9e";
            appId[7] = "e9376fb6-c62b-e011-854c-00237de2db9e";
        }

        void Image_Click(object sender, RoutedEventArgs e)
        {
            Border b = (Border)sender;
            int appNo = 2 * b.GetRow() + b.GetColumn();
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = appId[appNo];
            marketplaceDetailTask.Show();

        }

        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { URL = "http://www.youtube.com/watch?v=BiklqLjr9lg" }.Show();
        }
    }
}
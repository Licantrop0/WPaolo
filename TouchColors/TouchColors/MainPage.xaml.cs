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

namespace TouchColors
{
    public partial class MainPage : PhoneApplicationPage
    {
        Random rnd = new Random();
        Dictionary<string, Color> colors = new Dictionary<string, Color>();


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            colors.Add("Black", Colors.Black);
            colors.Add("Blue", Colors.Blue);
            colors.Add("Brown", Colors.Brown);
            colors.Add("Cyan", Colors.Cyan);
            colors.Add("Dark Gray", Colors.DarkGray);
            colors.Add("Gray", Colors.Gray);
            colors.Add("Green", Colors.Green);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var c1 = colors.ToArray()[rnd.Next(colors.Count)];
            var c2 = colors.ToArray()[rnd.Next(colors.Count)];
            PageTitle.Text = c1.Key;
            border1.Background = new SolidColorBrush(c1.Value);
            border2.Background = new SolidColorBrush(c2.Value);
        }

        private void border1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var c1 = colors.ToArray()[rnd.Next(colors.Count)];
            var c2 = colors.ToArray()[rnd.Next(colors.Count)];
            PageTitle.Text = c1.Key;
            border1.Background = new SolidColorBrush(c1.Value);
            border2.Background = new SolidColorBrush(c2.Value);

        }
    }
}
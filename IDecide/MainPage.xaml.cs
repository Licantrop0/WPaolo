﻿using System;
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
using Microsoft.Phone.Shell;

namespace IDecide
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            ApplicationBar = new ApplicationBar();
            CreateAppBar();
        }

        private void CreateAppBar()
        {
            var EditChoicesAppBarButton = new ApplicationBarIconButton();
            EditChoicesAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
            EditChoicesAppBarButton.Text = AppResources.EditChoices;
            EditChoicesAppBarButton.Click += delegate(object sender, EventArgs e)
            { NavigationService.Navigate(new Uri("/GroupChoicesPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(EditChoicesAppBarButton);
        }

        private void DecideButton_Click(object sender, RoutedEventArgs e)
        {
            var SelectedChoices = Settings.ChoicesGroup
                .Where(c => c.Key == Settings.SelectedGroup)
                .Select(c => c.Value)
                .ToList();

            if (SelectedChoices.Count > 0)
                MessageBox.Show(SelectedChoices[new Random().Next(SelectedChoices.Count)]);
            else
                MessageBox.Show(AppResources.NothingToDecide);
        }

    }
}
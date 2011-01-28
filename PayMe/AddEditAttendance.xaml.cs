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
using Microsoft.Phone.Shell;

namespace PayMe
{
    public partial class AddEditAttendance : PhoneApplicationPage
    {
        Attendance CurrentAttendance;

        public AddEditAttendance()
        {
            InitializeComponent();
            CreateAppBar();

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentAttendance = Settings.Attendances.Where(a =>
                a.Id == Convert.ToInt32(NavigationContext.QueryString["id"])).First();

            AttendanceStackPanel.DataContext = CurrentAttendance;
        }
        
        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();

            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                CustomerNameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                DescriptionTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                NavigationService.GoBack();
            };

            ApplicationBar.Buttons.Add(SaveAppBarButton);

            var DeleteAppBarButton = new ApplicationBarIconButton();
            DeleteAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar.delete.png", UriKind.Relative);
            DeleteAppBarButton.Text = AppResources.Delete;
            DeleteAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                NavigationService.GoBack();
                Settings.Attendances.Remove(CurrentAttendance);
            };
            ApplicationBar.Buttons.Add(DeleteAppBarButton);
        }

        private void CustomerNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DescriptionTextBox.Focus();
            }
        }

        private void DescriptionTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

    }
}
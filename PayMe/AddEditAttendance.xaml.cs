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
            
            var DeleteAppBarButton = new ApplicationBarIconButton();
            DeleteAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar.delete.png", UriKind.Relative);
            DeleteAppBarButton.Text = AppResources.Delete;
            DeleteAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                Settings.Attendances.Remove(CurrentAttendance);
                NavigationService.GoBack();
            };
            ApplicationBar.Buttons.Add(DeleteAppBarButton);
        }

    }
}
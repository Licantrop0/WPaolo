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

namespace PayMe
{
    public partial class AttendancesListPage : PhoneApplicationPage
    {
        public AttendancesListPage()
        {
            InitializeComponent();
            AttendanceListBox.ItemsSource = Settings.Attendances;
        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
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
using System.Reflection;
using DeathTimerz.Localization;

namespace DeathTimerz
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            WPMEAbout.ApplicationName = AppResources.AppName;
            WPMEAbout.GetOtherAppsText = AppResources.GetOtherApps;
            var name = Assembly.GetExecutingAssembly().FullName;
            WPMEAbout.ApplicationVersion = new AssemblyName(name).Version.ToString(); 
        }
    }
}
﻿using Microsoft.Phone.Controls;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;

namespace SheldonMix.View
{
    public partial class UpdatePage : PhoneApplicationPage
    {
        public UpdatePage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                if (!isf.FileExists(AppContext.XmlPath))
                    Application.Current.Terminate();
        }
    }
}
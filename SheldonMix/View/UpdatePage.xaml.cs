using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.ComponentModel;

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
                    AppContext.CloseApp();
        }
    }
}
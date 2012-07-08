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
using Scudetti.ViewModel;

namespace Scudetti.View
{
    public partial class ShieldsPage : PhoneApplicationPage
    {
        private ShieldsViewModel _vM;
        public ShieldsViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as ShieldsViewModel;
                return _vM;
            }
        }

        public ShieldsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            VM.Level = int.Parse(NavigationContext.QueryString["level"]);
            base.OnNavigatedTo(e);
        }
    }
}
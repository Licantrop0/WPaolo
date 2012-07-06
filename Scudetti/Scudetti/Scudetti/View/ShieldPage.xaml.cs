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
    public partial class ShieldPage : PhoneApplicationPage
    {
        private ShieldViewModel _vM;
        public ShieldViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = this.DataContext as ShieldViewModel;
                return _vM;
            }
        }

        public ShieldPage()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            ShieldNameTextbox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            VM.Validate();
        }
    }
}
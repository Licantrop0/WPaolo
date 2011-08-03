using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using WPCommon;
using IDecide.Localization;
using GalaSoft.MvvmLight.Messaging;
using IDecide.ViewModel;

namespace IDecide
{
    public partial class AddEditChoicesPage : PhoneApplicationPage
    {
        private AddEditChoicesViewModel _vM;
        public AddEditChoicesViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as AddEditChoicesViewModel;
                return _vM;
            }
        }

        public AddEditChoicesPage()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, MouseButtonEventArgs e)
        {
            VM.AddChoice.Execute(ChoiceTextBox.Text);
            ChoiceTextBox.Text = string.Empty;
            ChoiceTextBox.Focus();
        }

        private void ChoiceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AddButton_Click(sender, null);
        }

        private void RemoveButton_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedChoice = ((Rectangle)sender).DataContext as string;
            VM.DeleteChoice.Execute(selectedChoice);
        }
    }
}
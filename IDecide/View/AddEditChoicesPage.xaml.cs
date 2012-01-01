using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IDecide.ViewModel;
using Microsoft.Phone.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Linq;

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

        private void ChoiceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChoiceTextBox_ActionIconTapped(sender, null);
        }

        private void GroupNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChoiceTextBox.Focus();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedChoice = (((Button)sender).DataContext).ToString();
            VM.DeleteChoice.Execute(selectedChoice);
        }

        private void ChoiceTextBox_ActionIconTapped(object sender, EventArgs e)
        {
            ChoiceTextBox.GetBindingExpression(PhoneTextBox.TextProperty).UpdateSource();
            VM.AddChoice.Execute(null);
            ChoiceTextBox.Focus();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            VM.SaveGroup();
        }
    }
}
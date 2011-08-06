using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IDecide.Localization;
using IDecide.ViewModel;
using Microsoft.Phone.Controls;

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

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (VM.EditMode)
                return;

            if (string.IsNullOrEmpty(GroupNameTextBox.Text))
            {
                e.Cancel = true;
                MessageBox.Show(AppResources.InsertGroupNameAlert);
                GroupNameTextBox.Focus();
                return;
            }
        }

        private void ChoiceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AddButton_Click(sender, null);
        }

        private void GroupNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChoiceTextBox.Focus();
        }

        private void AddButton_Click(object sender, MouseButtonEventArgs e)
        {
            VM.AddChoice.Execute(ChoiceTextBox.Text);
            ChoiceTextBox.Text = string.Empty;
            ChoiceTextBox.Focus();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedChoice = ((Button)sender).DataContext as string;
            VM.DeleteChoice.Execute(selectedChoice);
        }
    }
}
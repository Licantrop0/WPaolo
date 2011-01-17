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

namespace IDecide
{
    public partial class EditChoicesPage : PhoneApplicationPage
    {
        string CurrentGroup;
        ObservableCollection<string> CurrentChoices = new ObservableCollection<string>();

        public EditChoicesPage()
        {
            InitializeComponent();
            BuildApplicationBar();
        }

        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentGroup = NavigationContext.QueryString["key"];
            foreach (var c in Settings.ChoicesGroup.Where(c => c.Key == CurrentGroup))
                CurrentChoices.Add(c.Value);

            ChoicesListBox.ItemsSource = CurrentChoices;
        }

        private void AddButton_Click(object sender, MouseButtonEventArgs e)
        {
            CurrentChoices.Add(ChoiceTextBox.Text);
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
            string name = ((Rectangle)sender).DataContext as string;
            if (name != null) CurrentChoices.Remove(name);
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                var OldItems = Settings.ChoicesGroup.Where(c => c.Key == CurrentGroup).ToList();
                foreach (var c in OldItems)
                    Settings.ChoicesGroup.Remove(c);

                foreach (var c in CurrentChoices)
                    Settings.ChoicesGroup.Add(new KeyValuePair<string, string>(CurrentGroup, c));

                NavigationService.GoBack();
            };
            ApplicationBar.Buttons.Add(SaveAppBarButton);

            var ClearChoicesAppBarMenuItem = new ApplicationBarMenuItem();
            ClearChoicesAppBarMenuItem.Text = AppResources.ClearChoices;
            ClearChoicesAppBarMenuItem.Click += delegate(object sender, EventArgs e) { CurrentChoices.Clear(); };
            ApplicationBar.MenuItems.Add(ClearChoicesAppBarMenuItem);
        }
    }
}
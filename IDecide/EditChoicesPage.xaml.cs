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

namespace IDecide
{
    public partial class EditChoicesPage : PhoneApplicationPage
    {
        private ObservableCollection<string> ChoicesInternal { get; set; }

        public EditChoicesPage()
        {
            InitializeComponent();
            ChoicesInternal = Settings.Choices;
            BuildApplicationBar();
        }

        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ChoicesListBox.ItemsSource = ChoicesInternal;
        }

        private void AddButton_Click(object sender, MouseButtonEventArgs e)
        {
            ChoicesInternal.Add(ChoiceTextBox.Text);
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
            if (name != null) ChoicesInternal.Remove(name);
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                Settings.Choices = ChoicesInternal;
                NavigationService.GoBack();
            };
            ApplicationBar.Buttons.Add(SaveAppBarButton);

            var MagicBallAppBarMenuItem = new ApplicationBarMenuItem();
            MagicBallAppBarMenuItem.Text = AppResources.MagicBall;
            MagicBallAppBarMenuItem.Click += delegate(object sender, EventArgs e) { this.AddItems(Settings.GetMagicBall()); };
            ApplicationBar.MenuItems.Add(MagicBallAppBarMenuItem);

            var YesNoMaybeAppBarMenuItem = new ApplicationBarMenuItem();
            YesNoMaybeAppBarMenuItem.Text = AppResources.YesNoMaybe;
            YesNoMaybeAppBarMenuItem.Click += delegate(object sender, EventArgs e) { this.AddItems(Settings.GetYesNoMaybe()); };
            ApplicationBar.MenuItems.Add(YesNoMaybeAppBarMenuItem);

            var PercentageAppBarMenuItem = new ApplicationBarMenuItem();
            PercentageAppBarMenuItem.Text = AppResources.Percentage;
            PercentageAppBarMenuItem.Click += delegate(object sender, EventArgs e) { this.AddItems(Settings.GetPercentage()); };
            ApplicationBar.MenuItems.Add(PercentageAppBarMenuItem);

            var HeadOrTailAppBarMenuItem = new ApplicationBarMenuItem();
            HeadOrTailAppBarMenuItem.Text = AppResources.HeadTail;
            HeadOrTailAppBarMenuItem.Click += delegate(object sender, EventArgs e) { this.AddItems(Settings.GetHeadOrTail()); };
            ApplicationBar.MenuItems.Add(HeadOrTailAppBarMenuItem);

            var RPSLSAppBarMenuItem = new ApplicationBarMenuItem();
            RPSLSAppBarMenuItem.Text = AppResources.RPSLS;
            RPSLSAppBarMenuItem.Click += delegate(object sender, EventArgs e) { this.AddItems(Settings.GetRPSLS()); };
            ApplicationBar.MenuItems.Add(RPSLSAppBarMenuItem);

            var ClearChoicesAppBarMenuItem = new ApplicationBarMenuItem();
            ClearChoicesAppBarMenuItem.Text = AppResources.ClearChoices;
            ClearChoicesAppBarMenuItem.Click += delegate(object sender, EventArgs e) { this.AddItems(new List<string>()); };
            ApplicationBar.MenuItems.Add(ClearChoicesAppBarMenuItem);
        }

        private void AddItems(IEnumerable<string> items)
        {
            ChoicesInternal.Clear();
            foreach (var item in items)
                ChoicesInternal.Add(item);

            ChoicesListBox.Focus();
        }

    }
}
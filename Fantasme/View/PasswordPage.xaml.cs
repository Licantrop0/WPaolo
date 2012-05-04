using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.Localization;
using System.IO.IsolatedStorage;
using System.Windows.Navigation;

namespace NascondiChiappe
{
    public partial class PasswordPage : PhoneApplicationPage
    {
        private bool IsNewPasswordMode { get { return Settings.Password == null; } }
        private bool IsChangePasswordMode { get { return NavigationContext.QueryString.ContainsKey("ChangePassword"); } }

        public PasswordPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsNewPasswordMode)
            {
                TitleTextBlock.Text = AppResources.SetPassword;
                PasswordStackPanel.Visibility = Visibility.Collapsed;
                NewPasswordStackPanel.Visibility = Visibility.Visible;
                NewPasswordBox.Focus();
            }
            else if (IsChangePasswordMode)
            {
                TitleTextBlock.Text = AppResources.ChangePassword;
                PasswordStackPanel.Visibility = Visibility.Collapsed;
                NewPasswordStackPanel.Visibility = Visibility.Visible;
                OldPasswordStackPanel.Visibility = Visibility.Visible;
                OldPasswordBox.Focus();
            }
            else //Insert Password Mode
            {
                AddChangePasswordMenuItem();
                ChangePasswordHintTextBlock.Visibility = Visibility.Visible;
                MainPasswordBox.Focus();
            }
        }

        private void AddChangePasswordMenuItem()
        {
            if (ApplicationBar.MenuItems.Count > 0)
                return;

            var ChangePasswordAppBarMenuItem = new ApplicationBarMenuItem();
            ChangePasswordAppBarMenuItem.Text = AppResources.ChangePassword;
            ChangePasswordAppBarMenuItem.Click += (sender1, e1) =>
            { NavigationService.Navigate(new Uri("/View/PasswordPage.xaml?ChangePassword=1", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(ChangePasswordAppBarMenuItem);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //La pagina della password non deve stare nel back-stack
            NavigationService.RemoveBackEntry();
        }

        private void InitializeApplicationBar()
        {
            var OkAppBarButton = new ApplicationBarIconButton();
            OkAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_check.png", UriKind.Relative);
            OkAppBarButton.Text = AppResources.Ok;
            OkAppBarButton.Click += new EventHandler(OkAppBarButton_Click);
            ApplicationBar.Buttons.Add(OkAppBarButton);
        }

        private void ForgotPasswordHLButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AppResources.Fucked);
            MainPasswordBox.Focus();
            MainPasswordBox.SelectAll();
        }

        void OkAppBarButton_Click(object sender, EventArgs e)
        {
            if (IsNewPasswordMode || IsChangePasswordMode)
            {
                if (IsChangePasswordMode && (Settings.Password != OldPasswordBox.Password)) //Controllo anche la vecchia password
                {
                    MessageBox.Show(AppResources.PasswordWrong);
                    OldPasswordBox.Focus();
                    OldPasswordBox.SelectAll();
                    return;
                }

                if (NewPasswordBox.Password.Length < Settings.PasswordMinLenght)
                {
                    MessageBox.Show(string.Format(AppResources.PasswordMinLenght, Settings.PasswordMinLenght));
                    ConfirmPasswordBox.Password = string.Empty;
                    NewPasswordBox.Focus();
                    NewPasswordBox.SelectAll();
                    return;
                }

                if (string.IsNullOrEmpty(ConfirmPasswordBox.Password))
                {
                    ConfirmPasswordBox.Focus();
                    return;
                }

                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show(AppResources.PasswordsDoesNotMatch);
                    ConfirmPasswordBox.Focus();
                    ConfirmPasswordBox.SelectAll();
                    return;
                }

                MessageBox.Show(AppResources.PasswordHint);
                Settings.Password = NewPasswordBox.Password;
            }
            else
            {
                if (Settings.Password != MainPasswordBox.Password)
                {
                    MessageBox.Show(AppResources.PasswordWrong);
                    MainPasswordBox.SelectAll();
                    return;
                }
            }

            AppContext.IsPasswordInserted = true;
            //Go to Albums Page
            NavigationService.Navigate(new Uri("/View/AlbumsPage.xaml", UriKind.Relative));
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OkAppBarButton_Click(sender, EventArgs.Empty);
        }

        private void OldPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NewPasswordBox.Focus();
                NewPasswordBox.SelectAll();
            }
        }

        private void NewPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmPasswordBox.Focus();
                ConfirmPasswordBox.SelectAll();
            }
            else if (e.Key == Key.Back && string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                OldPasswordBox.Focus();
            }
        }

        private void ConfirmPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OkAppBarButton_Click(sender, EventArgs.Empty);
            else if (e.Key == Key.Back && string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                NewPasswordBox.Focus();
            }
        }

    }
}
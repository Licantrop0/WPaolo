using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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
            else
            {
                MainPasswordBox.Focus();
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Settings.PasswordInserted & !IsChangePasswordMode)
                throw new Exception("ForceExit");
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            var OkAppBarButton = new ApplicationBarIconButton();
            OkAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_check.png", UriKind.Relative);
            OkAppBarButton.Text = AppResources.Ok;
            OkAppBarButton.Click += new EventHandler(OkAppBarButton_Click);
            ApplicationBar.Buttons.Add(OkAppBarButton);
        }

        private void ForgotPasswordHLButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AppResources.Fucked);
        }

        void OkAppBarButton_Click(object sender, EventArgs e)
        {
            if (IsNewPasswordMode || IsChangePasswordMode)
            {
                if (IsChangePasswordMode && (Settings.Password != OldPasswordBox.Password)) //Controllo anche la vecchia password
                {
                    MessageBox.Show(AppResources.PasswordWrong);
                    OldPasswordBox.SelectAll();
                    return;
                }

                if (NewPasswordBox.Password.Length < Settings.PasswordMinLenght)
                {
                    MessageBox.Show(string.Format(AppResources.PasswordMinLenght, Settings.PasswordMinLenght));
                    NewPasswordBox.SelectAll();
                    return;
                }

                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show(AppResources.PasswordsDoesNotMatch);
                    ConfirmPasswordBox.SelectAll();
                    return;
                }

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

            Settings.PasswordInserted = true;
            NavigationService.Navigate(new Uri("/AlbumsPage.xaml", UriKind.Relative));
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) OkAppBarButton_Click(sender, EventArgs.Empty);
        }

        private void OldPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) NewPasswordBox.Focus();
        }

        private void NewPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ConfirmPasswordBox.SelectAll();
        }
    }
}
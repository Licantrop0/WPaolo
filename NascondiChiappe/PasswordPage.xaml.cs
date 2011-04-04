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
using Microsoft.Phone.Shell;

namespace NascondiChiappe
{
    public partial class PasswordPage : PhoneApplicationPage
    {
        private bool IsNewPasswordMode { get { return Settings.Password == null; } }
        private bool IsChangePasswordMode
        {
            get
            {
                return NavigationContext.QueryString.ContainsKey("ChangePassword") &&
                    NavigationContext.QueryString["ChangePassword"] == "1";
            }
        }

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
            }
            else if (IsChangePasswordMode)
            {
                TitleTextBlock.Text = AppResources.ChangePassword;
                PasswordStackPanel.Visibility = Visibility.Collapsed;
                OldPasswordStackPanel.Visibility = Visibility.Visible;
                NewPasswordStackPanel.Visibility = Visibility.Visible;
            }
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Buttons.Add(CreateOkAppBarButton());
            ApplicationBar.Buttons.Add(CreateCancelAppBarButton());
        }

        private ApplicationBarIconButton CreateOkAppBarButton()
        {
            var OkAppBarButton = new ApplicationBarIconButton();
            OkAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Check.png", UriKind.Relative);
            OkAppBarButton.Text = AppResources.Ok;
            OkAppBarButton.Click += (sender, e) =>
            {
                if (IsChangePasswordMode)
                {
                    if (Settings.Password != OldPasswordBox.Password)
                    {
                        MessageBox.Show(AppResources.PasswordWrong);
                        return;
                    }

                    if (NewPasswordBox.Password.Length < Settings.PasswordMinLenght)
                    {
                        MessageBox.Show(string.Format(AppResources.PasswordMinLenght, Settings.PasswordMinLenght));
                        return;
                    }

                    if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                    {
                        MessageBox.Show(AppResources.PasswordsDoesNotMatch);
                        return;
                    }

                    Settings.Password = NewPasswordBox.Password;

                }
                if (IsNewPasswordMode)
                {
                    if (NewPasswordBox.Password.Length < Settings.PasswordMinLenght)
                    {
                        MessageBox.Show(string.Format(AppResources.PasswordMinLenght, Settings.PasswordMinLenght));
                        return;
                    }

                    if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                    {
                        MessageBox.Show(AppResources.PasswordsDoesNotMatch);
                        return;
                    }

                    Settings.Password = NewPasswordBox.Password;
                }
                else
                {
                    if (Settings.Password != MainPasswordBox.Password)
                    {
                        MessageBox.Show(AppResources.PasswordWrong);
                        return;
                    }
                }
                NavigationService.Navigate(new Uri("/AlbumsPage.xaml", UriKind.Relative));
            };
            return OkAppBarButton;
        }

        private ApplicationBarIconButton CreateCancelAppBarButton()
        {
            var CancelAppBarButton = new ApplicationBarIconButton();
            CancelAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Cancel.png", UriKind.Relative);
            CancelAppBarButton.Text = AppResources.Cancel;
            CancelAppBarButton.Click += (sender, e) => { NavigationService.GoBack(); };
            return CancelAppBarButton;
        }

        private void ForgotPasswordHLButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AppResources.Fucked);
        }


    }
}
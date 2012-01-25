using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace NientePanico
{
    public partial class PasswordPage : PhoneApplicationPage
    {
        private bool IsNewPasswordMode { get { return  AppContext.Password == null; } }
        private bool IsChangePasswordMode { get { return NavigationContext.QueryString.ContainsKey("ChangePassword"); } }

        public PasswordPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (IsNewPasswordMode)
            {
                TitleTextBlock.Text = "Imposta Password";
                PasswordStackPanel.Visibility = Visibility.Collapsed;
                NewPasswordStackPanel.Visibility = Visibility.Visible;
                NewPasswordBox.Focus();
            }
            else if (IsChangePasswordMode)
            {
                TitleTextBlock.Text = "Cambia Password";
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

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationService.RemoveBackEntry();
        }

        private void AddChangePasswordMenuItem()
        {
            if (ApplicationBar.MenuItems.Count > 0)
                return;

            var ChangePasswordAppBarMenuItem = new ApplicationBarMenuItem();
            ChangePasswordAppBarMenuItem.Text = "Cambia Password";
            ChangePasswordAppBarMenuItem.Click += (sender1, e1) =>
            { NavigationService.Navigate(new Uri("/PasswordPage.xaml?ChangePassword=1", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(ChangePasswordAppBarMenuItem);
        }

        private void ForgotPasswordHLButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mi spiace, te l'avevo detto che non potevi recuperare la password!");
            MainPasswordBox.Focus();
            MainPasswordBox.SelectAll();
        }

        void OkAppBarButton_Click(object sender, EventArgs e)
        {
            if (IsNewPasswordMode || IsChangePasswordMode)
            {
                if (IsChangePasswordMode && (AppContext.Password != OldPasswordBox.Password)) //Controllo anche la vecchia password
                {
                    MessageBox.Show("La password che hai inserito è sbagliata, riprova");
                    OldPasswordBox.Focus();
                    OldPasswordBox.SelectAll();
                    return;
                }

                if (NewPasswordBox.Password.Length < 1)
                {
                    MessageBox.Show("La password deve essere almeno di 1 carattere");
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
                    MessageBox.Show("Le password non corrispondono");
                    ConfirmPasswordBox.Focus();
                    ConfirmPasswordBox.SelectAll();
                    return;
                }

                MessageBox.Show("ATTENZIONE: non dimenticare la Password, altrimenti non sarai più in grado di accedere alle tue foto.\nIl recupero password NON è disponibile.");
                AppContext.Password = NewPasswordBox.Password;
            }
            else
            {
                if (AppContext.Password != MainPasswordBox.Password)
                {
                    MessageBox.Show("La Password inserita è errata");
                    MainPasswordBox.SelectAll();
                    return;
                }
            }

            //Go to Albums Page
            NavigationService.Navigate(new Uri("/SensitiveDataPage.xaml", UriKind.Relative));
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
using System;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.ViewModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;
using NascondiChiappe.Localization;

namespace NascondiChiappe.View
{
    public partial class AddRenameAlbumPage : PhoneApplicationPage
    {
        private AddRenameAlbumViewModel _vM;
        public AddRenameAlbumViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as AddRenameAlbumViewModel;
                return _vM;
            }
        }

        public AddRenameAlbumPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
            }
        }

        private void AlbumNameTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AlbumNameTextBox.Focus();
            AlbumNameTextBox.SelectAll();
        }

        private void AlbumNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveAlbum();
        }

        private void SaveAlbum()
        {
            if (!CheckAlbumName())
                return;

            this.Focus();
            AlbumNameTextBox.GetBindingExpression(
                TextBox.TextProperty).UpdateSource();

            VM.SaveAlbum.Execute(null);
        }

        private bool CheckAlbumName()
        {
            if (string.IsNullOrEmpty(AlbumNameTextBox.Text))
            {
                MessageBox.Show(AppResources.AlbumNameRequired);
                AlbumNameTextBox.Focus();
                return false;
            }
            return true;
        }

        private void InitializeApplicationBar()
        {
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = NascondiChiappe.Localization.AppResources.Save;
            SaveAppBarButton.Click += (sender, e) => { SaveAlbum(); };
            ApplicationBar.Buttons.Add(SaveAppBarButton);
        }
    }
}
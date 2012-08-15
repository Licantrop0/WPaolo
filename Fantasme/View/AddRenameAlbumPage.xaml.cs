using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.Localization;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe.View
{
    public partial class AddRenameAlbumPage : PhoneApplicationPage
    {
        private AddRenameAlbumViewModel _vm;

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
                return;
            }

            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                var id = int.Parse(NavigationContext.QueryString["id"]);
                _vm = new AddRenameAlbumViewModel(AppContext.Albums[id]);
            }
            else
            {
                _vm = new AddRenameAlbumViewModel(new AlbumViewModel());
            }
            LayoutRoot.DataContext = _vm;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Uri.OriginalString == "/View/PasswordPage.xaml")
                NavigationService.RemoveBackEntry();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
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

            _vm.SaveAlbum.Execute(null);
            NavigationService.GoBack();
        }

        private bool CheckAlbumName()
        {
            if (string.IsNullOrEmpty(AlbumNameTextBox.Text))
            {
                MessageBox.Show(AppResources.AlbumNameRequired);
                AlbumNameTextBox.Focus();
                return false;
            }

            if (AppContext.Albums.Any(a => a.Name == AlbumNameTextBox.Text))
            {
                MessageBox.Show(AppResources.AlbumNameDuplicated);
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

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (AppContext.Albums.Count == 0)
                throw new Exception("ForceExit");
        }
    }
}
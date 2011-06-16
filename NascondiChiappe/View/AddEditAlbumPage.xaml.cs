using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.Localization;
using NascondiChiappe.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Messages;

namespace NascondiChiappe
{
    public partial class AddEditAlbumPage : PhoneApplicationPage
    {
        ApplicationBarIconButton DeletePhotosAppBarButton = new ApplicationBarIconButton();
        ApplicationBarIconButton MovePhotosAppBarButton = new ApplicationBarIconButton();

        private AddEditAlbumViewModel _vM;
        public AddEditAlbumViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as AddEditAlbumViewModel;
                return _vM;
            }
        }

        public AddEditAlbumPage()
        {
            InitializeComponent();
            InitializeApplicationBar();

            Messenger.Default.Register<CanExecuteOnSelectedPhotosMessage>(
                this, m => ImagesSelectionChanged(m.CanExecute));
        }

        private void ImagesSelectionChanged(bool canExecute)
        {
            DeletePhotosAppBarButton.IsEnabled = canExecute;
            MovePhotosAppBarButton.IsEnabled = canExecute;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (VM.EditMode && ApplicationBar.Buttons.Count == 1)
            {
                DeletePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_cancel.png", UriKind.Relative);
                DeletePhotosAppBarButton.Text = AppResources.DeleteSelectedPhotos;
                DeletePhotosAppBarButton.IsEnabled = false;
                DeletePhotosAppBarButton.Click += (sender1, e1) => { VM.DeletePhotos.Execute(null); };
                ApplicationBar.Buttons.Add(DeletePhotosAppBarButton);

                var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
                DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
                DeleteAlbumAppBarMenuItem.Click += (sender1, e1) => { VM.DeleteAlbum.Execute(null); };
                ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);
            }
            else //BUG SUL BACK dalla ViewPhotosPage
                AlbumNameTextBox.Focus();

            if (AppContext.Albums.Count > 1 && ApplicationBar.Buttons.Count < 3)
            {
                MovePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_move.png", UriKind.Relative);
                MovePhotosAppBarButton.Text = AppResources.MoveSelectedPhotos;
                MovePhotosAppBarButton.IsEnabled = false;
                MovePhotosAppBarButton.Click += new EventHandler(MovePhotosAppBarButton_Click);
                ApplicationBar.Buttons.Add(MovePhotosAppBarButton);
            }
        }

        private void InitializeApplicationBar()
        {
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += new EventHandler(SaveAppBarButton_Click);
            ApplicationBar.Buttons.Add(SaveAppBarButton);
        }

        void MovePhotosAppBarButton_Click(object sender, EventArgs e)
        {
            if (VM.ImageList.SelectedPhotos.Count == 0)
            {
                MessageBox.Show(AppResources.SelectPhotos);
                return;
            }
            PopupBackground.Visibility = Visibility.Visible;
            PopupBorder.Visibility = Visibility.Visible;
        }

        private void AlbumsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.MovePhotos.Execute(e.AddedItems[0]);

            PopupBackground.Visibility = Visibility.Collapsed;
            PopupBorder.Visibility = Visibility.Collapsed;
        }


        void SaveAppBarButton_Click(object sender, EventArgs e)
        {
            if (!CheckAlbumName())
                return;

            //Fix per aggiornare il ViewModel programmaticamente
            //non si scatena il LostFocus alla pressione dell'appbarbutton
            var be = AlbumNameTextBox.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource(); 
            
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

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (PopupBorder.Visibility == Visibility.Visible)
            {
                PopupBackground.Visibility = Visibility.Collapsed;
                PopupBorder.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
        }

        private void AlbumNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveAppBarButton_Click(sender, EventArgs.Empty);
        }
    }
}

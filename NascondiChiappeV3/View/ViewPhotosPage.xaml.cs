using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class ViewPhotosPage : PhoneApplicationPage
    {
        private ViewPhotosViewModel _vM;
        public ViewPhotosViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as ViewPhotosViewModel;
                return _vM;
            }
        }

        public ViewPhotosPage()
        {
            InitializeComponent();
            VM.CreateHtml();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
                return;
            }
        }

        private void Wb_Loaded(object sender, RoutedEventArgs e)
        {
            Wb.Navigate(new Uri("image.html", UriKind.Relative));
        }
    }
}
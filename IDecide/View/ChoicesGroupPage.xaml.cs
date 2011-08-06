using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using IDecide.Localization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using IDecide.Model;
using IDecide.ViewModel;

namespace IDecide
{
    public partial class ChoicesGroupPage : PhoneApplicationPage
    {
        private ChoicesGroupViewModel _vM;
        public ChoicesGroupViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as ChoicesGroupViewModel;
                return _vM;
            }
        }

        public ChoicesGroupPage()
        {
            InitializeComponent();
            CreateAppBar();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = ((Button)sender).DataContext as ChoiceGroupViewModel;
            VM.DeleteGroup.Execute(selectedGroup);
        }

        private void EditChoicesButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = ((Button)sender).DataContext as ChoiceGroupViewModel;
            VM.EditGroup.Execute(selectedGroup);
        }

        private void CreateAppBar()
        {
            var AddChoiceGroupAppBarButton = new ApplicationBarIconButton();
            AddChoiceGroupAppBarButton.IconUri = new Uri("Toolkit.Content\\add_white.png", UriKind.Relative);
            AddChoiceGroupAppBarButton.Text = AppResources.AddGroup;
            AddChoiceGroupAppBarButton.Click += (sender, e) => { VM.AddGroup.Execute(null); };
            ApplicationBar.Buttons.Add(AddChoiceGroupAppBarButton);
        }

    }
}
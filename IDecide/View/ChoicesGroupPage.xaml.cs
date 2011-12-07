using System;
using IDecide.Localization;
using IDecide.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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
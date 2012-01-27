using Microsoft.Phone.Controls;
using NientePanico.ViewModel;
using System.ComponentModel;
using System.Windows.Navigation;

namespace NientePanico
{
    public partial class CardDataPage : PhoneApplicationPage
    {
        public CardDataPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex;
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                DataContext = new CardDataViewModel(AppContext.Cards[index]);
            }
            else
            {
                DataContext = new CardDataViewModel();
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            var vm = (CardDataViewModel)DataContext;
            if (!vm.IsEditMode && !string.IsNullOrEmpty(NameTextBox.Text))
            {
                AppContext.Cards.Add(vm.CurrentCard);
            }
        }
    }
}
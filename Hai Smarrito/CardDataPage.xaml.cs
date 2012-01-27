using Microsoft.Phone.Controls;
using NientePanico.ViewModel;

namespace NientePanico
{
    /// <summary>
    /// Description for CardDataPage.
    /// </summary>
    public partial class CardDataPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the CardDataPage class.
        /// </summary>
        public CardDataPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
                return;

            var vm = (CardDataViewModel)LayoutRoot.DataContext;

            if (!vm.IsEditMode)
                AppContext.Cards.Add(vm);
        }
    }
}
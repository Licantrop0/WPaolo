using TouchColors.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TouchColors.View
{
    public sealed partial class QuestionsPage : Page
    {
        QuestionsViewModel ViewModel => (QuestionsViewModel)this.DataContext;

        public QuestionsPage()
        {
            this.InitializeComponent();
        }
    }
}

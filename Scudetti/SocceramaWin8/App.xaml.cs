using SocceramaWin8.View;
using Topics.Radical.Windows.Presentation.Boot;
using Windows.UI.Xaml;


namespace SocceramaWin8
{
    sealed partial class App : Application
    {
        ApplicationBootstrapper bootstrapper;
        public App()
        {
            this.bootstrapper = new PuzzleApplicationBootstrapper<LevelsView>();
        }
    }
}

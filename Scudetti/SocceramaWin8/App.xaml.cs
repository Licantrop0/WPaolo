using SocceramaWin8.Presentation;
using Topics.Radical.Windows.Presentation.Boot;
using Windows.UI.Xaml;

namespace SocceramaWin8
{
    sealed partial class App : Application
    {
        ApplicationBootstrapper _bootstrapper;
        public App()
        {
            _bootstrapper = new PuzzleApplicationBootstrapper<LevelsView>();
            //_bootstrapper.OnBoot(arg => AppContext.LoadShieldsAsync());
        }
    }
}

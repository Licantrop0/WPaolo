using GalaSoft.MvvmLight.Messaging;
using Scudetti.Model;
using SocceramaWin8.View;
using SocceramaWin8.ViewModel;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Callisto.Controls.SettingsManagement;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Media;
using Windows.UI;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace SocceramaWin8
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                RegisterNavigationMessages(rootFrame);

                if (args.PreviousExecutionState != ApplicationExecutionState.Suspended)
                {
                    if (AppContext.Shields == null)
                        await AppContext.LoadShieldsAsync();
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(LevelsPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
        }

        private void RegisterNavigationMessages(Frame rootFrame)
        {
            Messenger.Default.Register<string>(this, "navigation", m => { if (m == "goback") rootFrame.GoBack(); });
            Messenger.Default.Register<LevelsViewModel>(this, vm => rootFrame.Navigate(typeof(LevelsPage)));
            Messenger.Default.Register<LevelViewModel>(this, vm => rootFrame.Navigate(typeof(LevelPage), vm));
            Messenger.Default.Register<ShieldViewModel>(this, vm => rootFrame.Navigate(typeof(ShieldPage), vm));
        }
    }
}

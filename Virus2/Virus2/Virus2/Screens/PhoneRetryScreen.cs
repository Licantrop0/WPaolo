using System;
using System.Diagnostics;

namespace Virus
{
    /// <summary>
    /// A basic retry screen for Windows Phone
    /// </summary>
    class PhoneRetryScreen : PhoneMenuScreen
    {
        VirusLevel level;  // reference to current level

        public PhoneRetryScreen(VirusLevel level)
            : base("Game Over")
        {
            this.level = level;

            // Create the "Retry" and "Exit" buttons for the screen
            Button retryButton = new Button("Retry");
            retryButton.Tapped += retryButton_Tapped;
            MenuButtons.Add(retryButton);

            Button backToMainMenuButton = new Button("Back to main menu");
            backToMainMenuButton.Tapped += backToMainMenuButton_Tapped;
            MenuButtons.Add(backToMainMenuButton);
        }

        /// <summary>
        /// The "Retry" button handler just calls the OnCancel method so that 
        /// pressing the "Resume" button is the same as pressing the hardware back button.
        /// </summary>
        void retryButton_Tapped(object sender, EventArgs e)
        {
            OnRetry();
        }

        /// <summary>
        /// The "Exit" button handler uses the LoadingScreen to take the user out to the main menu.
        /// </summary>
        void backToMainMenuButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        private void OnRetry()
        {
            level.InitializeLevel(GameGlobalState.ActualLevel);
            ExitScreen();
            base.OnCancel();
        }
    }
}

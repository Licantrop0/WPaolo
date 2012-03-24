using System;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;

namespace WizardMario
{
    /// <summary>
    /// A basic retry screen for Windows Phone
    /// </summary>
    class PhoneRetryScreen : PhoneMenuScreen
    {
        //WizardMarioLevel level;  // reference to current level

        public PhoneRetryScreen(/*WizardMarioLevel level*/)
            : base("Game Over")
        {
            //this.level = level;

            // Create the "Retry" and "Exit" buttons for the screen
            Button retryButton = new Button("Retry");
            retryButton.Tapped += retryButton_Tapped;
            MenuButtons.Add(retryButton);

            Button exitButton = new Button("Exit");
            exitButton.Tapped += exitButton_Tapped;
            MenuButtons.Add(exitButton);
        }

        /// <summary>
        /// </summary>
        void retryButton_Tapped(object sender, EventArgs e)
        {
            //level.InitializeLevel(GameGlobalState.ActualLevel, false);
            ExitScreen();
            //base.OnCancel();
        }

        /// <summary>
        /// The "Exit" button handler uses the LoadingScreen to take the user out to the main menu.
        /// </summary>
        void exitButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }
    }
}

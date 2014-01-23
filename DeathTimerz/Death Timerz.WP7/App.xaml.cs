﻿using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Navigation;
using DeathTimerz.Sounds;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using System;
using DeathTimerz.Localization;

namespace DeathTimerz
{
    public partial class App : Application
    {
        IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        // Easy access to the root frame
        public PhoneApplicationFrame RootFrame { get; private set; }

        // Constructor
        public App()
        {
            // Global handler for uncaught exceptions. 
            // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }


        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            SoundManager.Instance.RestoreMusicStatus();
            InitializeScheduledAgent();
        }

        private void InitializeScheduledAgent()
        {
            var TASK_NAME = "UpdateHealthAdvicesTask";

            var action = ScheduledActionService.Find(TASK_NAME) as PeriodicTask;
            if (action != null) ScheduledActionService.Remove(action.Name);
            PeriodicTask task = new PeriodicTask(TASK_NAME) { Description = AppResources.TaskDescription };

            try
            {
                ScheduledActionService.Add(task);
            }
            catch (InvalidOperationException)
            {
                //creo la tile manualmente se c'è un errore nello scheduled agent
                UpdateHealthAdvicesTask.ScheduledAgent.UpdateTileData();
            }


#if DEBUG
            // If we're debugging, attempt to start the task immediately 
            ScheduledActionService.LaunchForTest(TASK_NAME, new TimeSpan(0, 0, 1));
#endif
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                SoundManager.Instance.RestoreMusicStatus();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            ////TODO: testare il tombstoning
            //using (var fs = new IsolatedStorageFileStream("Test1.xml", FileMode.Create, FileAccess.Write, isf))
            //{
            //    Settings.Test1.Save(fs);
            //}

            //using (var fs = new IsolatedStorageFileStream("Test2.xml", FileMode.Create, FileAccess.Write, isf))
            //{
            //    Settings.Test2.Save(fs);
            //}
        }

        // Code to execute if a navigation fails
        void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}
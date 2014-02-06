using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace UpdateHealthAdvicesTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized) return;
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
            });
            _classInitialized = true;
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            Deployment.Current.Dispatcher.BeginInvoke(UpdateTileData);
            NotifyComplete();
        }

        private const string TilePath = "/Shared/ShellContent/LiveTileIcon.jpg";
        public static void UpdateTileData()
        {
            try
            {
                using (var iss = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var file = iss.OpenFile(TilePath, FileMode.OpenOrCreate))
                    {
                        //avoid unnecessary operations (the tile changes only once a day)
                        var lastWrite = iss.GetLastWriteTime(TilePath).DayOfYear;
                        if (lastWrite == DateTime.Now.DayOfYear && file.Length != 0) return;
                        (new TileControl()).Update(file);
                    }
                }

                var tileData = new StandardTileData() { BackBackgroundImage = new Uri("isostore:" + TilePath) };
                foreach (var tile in ShellTile.ActiveTiles)
                    tile.Update(tileData);
            }
            catch (IsolatedStorageException)
            { /* boh */ }
        }

    }
}
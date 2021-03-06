﻿using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace UpdateHealthAdvicesTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;
        private const string TilePath = "/Shared/ShellContent/LiveTileIcon.jpg";

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
            Deployment.Current.Dispatcher.BeginInvoke(WriteFile);
            NotifyComplete();
        }

        public static void WriteFile()
        {
            using (var iss = IsolatedStorageFile.GetUserStoreForApplication())
            using (var file = iss.OpenFile(TilePath, FileMode.OpenOrCreate))
            {
                //avoid unnecessary operations (the tile changes only once a day)
                var lastWrite = iss.GetLastWriteTime(TilePath).DayOfYear;
                if (lastWrite != DateTime.Now.DayOfYear || file.Length == 0)
                    UpdateTiles(file);
            }
        }

        private static void UpdateTiles(IsolatedStorageFileStream file)
        {
            string advice = HealthAdvices.HealthAdvice.GetAdviceOfTheDay();
            var t = new TileControl(advice);
            t.Loaded += (sender, e) =>
            {
                WriteableBitmap wbmp = t.ToTile();
                wbmp.SaveJpeg(file, 336, 336, 0, 80);
                var tileData = new StandardTileData() { BackBackgroundImage = new Uri("isostore:" + TilePath) };
                foreach (var tile in ShellTile.ActiveTiles) tile.Update(tileData);
            };
        }
    }
}
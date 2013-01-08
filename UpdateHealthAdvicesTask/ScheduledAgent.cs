using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
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
            //var ctl = CreateTileControl("TEST test TEST test");
            //var bmp = new WriteableBitmap(336, 336);
            //bmp.Render(ctl, null);
            //bmp.Invalidate();

            //var iss = IsolatedStorageFile.GetUserStoreForApplication();
            //using (var stm = iss.CreateFile("/Shared/ShellContent/backTile.jpg"))
            //{
            //    bmp.SaveJpeg(stm, 336, 336, 0, 80);
            //}

            var mainTile = ShellTile.ActiveTiles.First();
            mainTile.Update(new StandardTileData
            {
                BackContent= "suggerimento di salute di test evviva"
                //BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/backTile.jpg", UriKind.Absolute)
            });

            NotifyComplete();
        }

        private FrameworkElement CreateTileControl(string tileText)
        {
            var b = new Border();
            b.Background = new SolidColorBrush(Colors.Red);

            var txt = new TextBlock()
            {
                Text = tileText
            };
            b.Child = txt;

            b.Measure(new Size(336, 336));
            b.Arrange(new Rect(0, 0, 336, 336));
            return b;
        }
    }
}
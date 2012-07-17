using System;
using System.Windows;
using Microsoft.Xna.Framework;
using System.Windows.Threading;

namespace WPCommon.Helpers
{
    //Riga da inserire dentro <Application.ApplicationLifetimeObjects> nell'App.xaml
    //<wph:XNAAsyncDispatcher xmlns:wph="clr-namespace:WPCommon.Helpers;assembly=WPCommon.Helpers.XNAAsyncDispatcher"/>

    public class XNAAsyncDispatcher : IApplicationService
    {
        private readonly DispatcherTimer _frameworkDispatcherTimer;

        public XNAAsyncDispatcher()
        {
            _frameworkDispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _frameworkDispatcherTimer.Tick += frameworkDispatcherTimer_Tick;
            frameworkDispatcherTimer_Tick(this, EventArgs.Empty);
        }

        void IApplicationService.StartService(ApplicationServiceContext context)
        {
            _frameworkDispatcherTimer.Start();
        }

        void IApplicationService.StopService()
        {
            _frameworkDispatcherTimer.Stop();
        }

        void frameworkDispatcherTimer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }
    }
}
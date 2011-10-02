using System;
using System.Windows;
using Microsoft.Xna.Framework;
using System.Windows.Threading;

namespace WPCommon.Helpers
{
    //Riga da inserire dentro <Application.ApplicationLifetimeObjects> nell'App.xaml
    //<wph:XNAAsyncDispatcher xmlns:wph="clr-namespace:WPCommon.Helpers;assembly=WPCommon.Helpers"/>

    public class XNAAsyncDispatcher : IApplicationService
    {
        private DispatcherTimer frameworkDispatcherTimer;

        public XNAAsyncDispatcher()
        {
            frameworkDispatcherTimer = new DispatcherTimer();
            frameworkDispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            frameworkDispatcherTimer.Tick += frameworkDispatcherTimer_Tick;
            frameworkDispatcherTimer_Tick(this, EventArgs.Empty);
        }

        void IApplicationService.StartService(ApplicationServiceContext context)
        {
            frameworkDispatcherTimer.Start();
        }
        
        void IApplicationService.StopService()
        {
            frameworkDispatcherTimer.Stop();
        }
        
        void frameworkDispatcherTimer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }
    }
}
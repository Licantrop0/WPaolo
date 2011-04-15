using System;
using System.Windows;
using Microsoft.Xna.Framework;
using System.Windows.Threading;

namespace WPCommon
{
    //Riga da inserire dentro <Application.ApplicationLifetimeObjects> nell'App.xaml
    //<wp:XNAAsyncDispatcher xmlns:wp="clr-namespace:WPCommon;assembly=WPCommon"/>

    public class XNAAsyncDispatcher : IApplicationService
    {
        private DispatcherTimer frameworkDispatcherTimer;

        public XNAAsyncDispatcher()
        {
            frameworkDispatcherTimer = new DispatcherTimer();
            frameworkDispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            frameworkDispatcherTimer.Tick += frameworkDispatcherTimer_Tick;
            frameworkDispatcherTimer_Tick(null, EventArgs.Empty);
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
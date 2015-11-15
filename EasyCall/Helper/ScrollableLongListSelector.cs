using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace EasyCall.Helper
{
    public class ScrollableLongListSelector : LongListSelector
    {
        public double ScrollPosition
        {
            get { return (double)GetValue(ViewPortProperty); }
            set { SetValue(ViewPortProperty, value); }
        }

        public static readonly DependencyProperty ViewPortProperty = DependencyProperty.Register(
            "ScrollPosition",
            typeof(double),
            typeof(ScrollableLongListSelector),
            new PropertyMetadata(0d, OnViewPortChanged));

        private ViewportControl _viewport;        
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _viewport = (ViewportControl)GetTemplateChild("ViewportControl");
            _viewport.ViewportChanged += OnViewportChanged;
        }

        private void OnViewportChanged(object sender, ViewportChangedEventArgs args)
        {
            ScrollPosition = _viewport.Viewport.Top;
        }

        private static void OnViewPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lls = (ScrollableLongListSelector)d;

            if (lls._viewport.Viewport.Top.Equals(lls.ScrollPosition)) return;

            lls._viewport.SetViewportOrigin(new Point(0, lls.ScrollPosition));
        }
    }
}

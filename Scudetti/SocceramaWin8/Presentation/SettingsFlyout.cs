using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace SocceramaWin8.Presentation
{
	class SettingsFlyout
	{
		private const int _width = 346;
		private Popup _popup;
        public event EventHandler FlyoutClosed;

		public void ShowFlyout(UserControl control)
		{
			_popup = new Popup();
            _popup.Closed += OnPopupClosed;
			Window.Current.Activated += OnWindowActivated;
			_popup.IsLightDismissEnabled = true;
			_popup.Width = _width;
			_popup.Height = Window.Current.Bounds.Height;

			control.Width = _width;
			control.Height = Window.Current.Bounds.Height;

			_popup.Child = control;
			_popup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - _width);
			_popup.SetValue(Canvas.TopProperty, 0);
			_popup.IsOpen = true;
		}

		private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
		{
			if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
			{
				_popup.IsOpen = false;
			}
		}

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;

            if(FlyoutClosed != null)
                FlyoutClosed.Invoke(sender, EventArgs.Empty);
        }
	}
}
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using NientePanico.ViewModel;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Media;

namespace NientePanico.Carte_Di_Credito
{
    public partial class NazioniPage : PhoneApplicationPage
    {
        private LongListSelector currentSelector;

        public NazioniPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var VM = (NazioniViewModel)LayoutRoot.DataContext;
            VM.CardType = NavigationContext.QueryString["type"];
        }

        private void LongListSelector_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            //Hold a reference to the active long list selector.
            currentSelector = sender as LongListSelector;

            //Construct and begin a swivel animation to pop in the group view.
            IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            Storyboard _swivelShow = new Storyboard();
            ItemsControl groupItems = e.ItemsControl;

            foreach (var item in groupItems.Items)
            {
                UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                if (container != null)
                {
                    Border content = VisualTreeHelper.GetChild(container, 0) as Border;
                    if (content != null)
                    {
                        DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

                        EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
                        showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
                        showKeyFrame1.Value = -60;
                        showKeyFrame1.EasingFunction = quadraticEase;

                        EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
                        showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(85);
                        showKeyFrame2.Value = 0;
                        showKeyFrame2.EasingFunction = quadraticEase;

                        showAnimation.KeyFrames.Add(showKeyFrame1);
                        showAnimation.KeyFrames.Add(showKeyFrame2);

                        Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
                        Storyboard.SetTarget(showAnimation, content.Projection);

                        _swivelShow.Children.Add(showAnimation);
                    }
                }
            }

            _swivelShow.Begin();
        }

        private void LongListSelector_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        {
            //Cancelling automatic closing and scrolling to do it manually.
            e.Cancel = true;
            if (e.SelectedGroup != null)
            {
                currentSelector.ScrollToGroup(e.SelectedGroup);
            }

            //Dispatch the swivel animation for performance on the UI thread.
            Dispatcher.BeginInvoke(() =>
            {
                //Construct and begin a swivel animation to pop out the group view.
                IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                Storyboard _swivelHide = new Storyboard();
                ItemsControl groupItems = e.ItemsControl;

                foreach (var item in groupItems.Items)
                {
                    UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                    if (container != null)
                    {
                        Border content = VisualTreeHelper.GetChild(container, 0) as Border;
                        if (content != null)
                        {
                            DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

                            EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
                            showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
                            showKeyFrame1.Value = 0;
                            showKeyFrame1.EasingFunction = quadraticEase;

                            EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
                            showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(125);
                            showKeyFrame2.Value = 90;
                            showKeyFrame2.EasingFunction = quadraticEase;

                            showAnimation.KeyFrames.Add(showKeyFrame1);
                            showAnimation.KeyFrames.Add(showKeyFrame2);

                            Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
                            Storyboard.SetTarget(showAnimation, content.Projection);

                            _swivelHide.Children.Add(showAnimation);
                        }
                    }
                }

                _swivelHide.Completed += _swivelHide_Completed;
                _swivelHide.Begin();

            });
        }

        private void _swivelHide_Completed(object sender, EventArgs e)
        {
            //Close group view.
            if (currentSelector != null)
            {
                currentSelector.CloseGroupView();
                currentSelector = null;
            }
        }
    }
}
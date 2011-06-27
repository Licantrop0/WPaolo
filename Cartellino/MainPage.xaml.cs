using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Cartellino.Helpers;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;

namespace Cartellino
{
    public partial class MainPage : PhoneApplicationPage
    {
        //al di sotto di questo valore l'immagine non scrolla
        //e torna nella sua posizione originaria
        const double SCROLLING_TOLLERANCE = 80;

        // indica la direzione dell'animazione
        public enum Direction
        {
            Left,
            Right,
        }

        DoubleAnimation HorizontalScrollingAnimation;
        Storyboard HorizontalScrollingStoryboard;

        int PagesCount;

        public MainPage()
        {
            InitializeComponent();

            // il numero totale di pagine è uguale al numero di figli dello StackPanel contenuto nello ScrollViewer
            PagesCount = (LogicalScrollViewer.Content as StackPanel).Children.Count;

            #region Animazione
            HorizontalScrollingAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTargetProperty(HorizontalScrollingAnimation,
                new PropertyPath(ScrollViewerUtilities.HorizontalOffsetProperty));

            HorizontalScrollingStoryboard = new Storyboard();
            HorizontalScrollingStoryboard.Children.Add(HorizontalScrollingAnimation);
            Storyboard.SetTarget(HorizontalScrollingAnimation, LogicalScrollViewer);
            HorizontalScrollingStoryboard.Completed += new EventHandler(HorizontalScrollingStoryboard_Completed);
            #endregion
        }

        private static SoundEffect _fischiettoSound;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_fischiettoSound == null)
                _fischiettoSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("fischietto.wav", UriKind.Relative)).Stream);

            _fischiettoSound.Play();
        }

        private void LogicalScrollViewer_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            // di quanti pixel ci siamo spostati orizzontalmente?
            double x = Math.Abs(e.TotalManipulation.Translation.X);

            // di quanti pixel ci siamo spostati verticalmente?
            double y = Math.Abs(e.TotalManipulation.Translation.Y);

            // orientamento dello scrolling -- orizzontale o verticale
            System.Windows.Controls.Orientation? scrollingOrientation; ;

            // se ci siamo spostati maggiormente sull'ascissa, allora si tratta di uno scrolling orizzontale
            if (Math.Max(x, y) == x)
                scrollingOrientation = System.Windows.Controls.Orientation.Horizontal;
            // se ci siamo spostati maggiormente sull'ordinata, allora si tratta di uno scrolling verticale
            else if (Math.Max(x, y) == y)
                scrollingOrientation = System.Windows.Controls.Orientation.Vertical;
            // altrimenti il valore resta null
            else
                scrollingOrientation = null;

            if (scrollingOrientation == System.Windows.Controls.Orientation.Horizontal)
            {
                // se il movimento è inerziale...
                if (e.IsInertial)
                {
                    // ...cerchiamo di capire se il "lancio" è verso dx...
                    if (e.FinalVelocities.LinearVelocity.X < 0)
                    {
                        if (Persistance.CurrentPage < PagesCount - 1)
                            ScrollTo(Direction.Right);
                        else
                            ScrollTo(Direction.Left);
                    }
                    // ...o verso sx
                    else
                    {
                        if (Persistance.CurrentPage > 0)
                            ScrollTo(Direction.Left);
                        else
                            ScrollTo(Direction.Right);
                    }
                }

                //se il movimento non è inerziale,
                //dobbiamo capire dove abbiamo lasciato il dito e se rientra nella tolleranza
                else
                {
                    double initialScrollingOffset = Persistance.PORTRAIT_WORKING_AREA_WIDTH * Persistance.CurrentPage;

                    if (Math.Abs(LogicalScrollViewer.HorizontalOffset - initialScrollingOffset) < SCROLLING_TOLLERANCE)
                        ScrollTo(null);
                    else
                    {
                        if (LogicalScrollViewer.VerticalOffset > initialScrollingOffset)
                            ScrollTo(Direction.Left);
                        else
                            ScrollTo(Direction.Right);
                    }
                }
            }
        }

        // gestisce lo scrolling sx/dx/null
        void ScrollTo(Direction? scrollingDirection)
        {
            IsHitTestVisible = false;
            int step = 0;

            switch (scrollingDirection)
            {
                case Direction.Left:
                    step = -1;
                    break;

                case Direction.Right:
                    step = 1;
                    break;

                default:
                    step = 0;
                    break;
            }

            HorizontalScrollingAnimation.From = LogicalScrollViewer.HorizontalOffset;
            HorizontalScrollingAnimation.To = Persistance.PORTRAIT_WORKING_AREA_WIDTH * (Persistance.CurrentPage + step);
            HorizontalScrollingStoryboard.Begin();
        }

        void HorizontalScrollingStoryboard_Completed(object sender, EventArgs e)
        {
            IsHitTestVisible = true;
            Persistance.HorizontalOffset = LogicalScrollViewer.HorizontalOffset;
        }

        private void LogicalScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            LogicalScrollViewer.ScrollToHorizontalOffset(Persistance.HorizontalOffset);
        }
    }
}
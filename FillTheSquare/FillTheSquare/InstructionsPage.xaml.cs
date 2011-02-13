using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WPCommon;

namespace FillTheSquare
{
    public partial class InstructionsPage : PhoneApplicationPage
    {
        // altezza dello schermo -- devi sottrarre eventuale menu e system tray
        // preferisco usare le costanti perché ho avuto problemi con ActualHeight
        //const int PORTRAIT_WORKING_AREA_HEIGHT = 800;

        // larghezza dello schermo
        const int PORTRAIT_WORKING_AREA_WIDTH = 480;

        // tolleranza per lo scrolling -- al di sotto di questo valore l'immagine non scrolla e torna nella sua posizione originaria
        // ho copiato "a occhio" quello delle foto, è sembra essere circa 1/10 dell'area da scrollare
        // puoi aumentarlo o diminuirlo a tuo piacimento
        const double SCROLLING_TOLLERANCE = 80;

        // indica la direzione dell'animazione -- nel tuo caso solo SU e GIU'
        public enum Direction
        {
            Left,
            Right,
            //Up,
            //Down,
        }

        // animazione per lo scrolling verticale
        //Storyboard verticalScrollingStoryboard;
        //DoubleAnimation verticalScrollingImageAnimation;

        // animazione per lo scrolling orizzontale
        Storyboard horizontalScrollingStoryboard;
        DoubleAnimation horizontalScrollingImageAnimation;

        int currentSubpage, subpagesCount;

        public InstructionsPage()
        {
            InitializeComponent();

            // la pagina corrente è la prima
            currentSubpage = 0;

            // il numero totale di pagine è uguale al numero di figli dello StackPanel contenuto nello ScrollViewer
            subpagesCount = (LogicalScrollViewer.Content as StackPanel).Children.Count;

            #region animazione

            //verticalScrollingImageAnimation = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(500), EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut } };
            //Storyboard.SetTargetProperty(verticalScrollingImageAnimation, new PropertyPath(ScrollViewerUtilities.VerticalOffsetProperty));

            //verticalScrollingStoryboard = new Storyboard();
            //verticalScrollingStoryboard.Children.Add(verticalScrollingImageAnimation);
            //Storyboard.SetTarget(verticalScrollingImageAnimation, LogicalScrollViewer);

            //verticalScrollingStoryboard.Completed += new EventHandler(VerticalScrollingStoryboard_Completed);


            horizontalScrollingImageAnimation = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(500), EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut } };
            Storyboard.SetTargetProperty(horizontalScrollingImageAnimation, new PropertyPath(ScrollViewerUtilities.HorizontalOffsetProperty));

            horizontalScrollingStoryboard = new Storyboard();
            horizontalScrollingStoryboard.Children.Add(horizontalScrollingImageAnimation);
            Storyboard.SetTarget(horizontalScrollingImageAnimation, LogicalScrollViewer);
            horizontalScrollingStoryboard.Completed += new EventHandler(HorizontalScrollingStoryboard_Completed);

            #endregion
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
            // se ci siamo spostati maggiormente sull'ordinata, allora si tratta di uno scrolling verticale -- è quello che serve a noi
            else if (Math.Max(x, y) == y)
                scrollingOrientation = System.Windows.Controls.Orientation.Vertical;
            // altrimenti il valore resta null
            else
                scrollingOrientation = null;

            // prendiamo in considerazione solo lo scrolling verticale...
            //if (scrollingOrientation == System.Windows.Controls.Orientation.Vertical)
            //{
            //    // se il movimento è inerziale...
            //    if (e.IsInertial)
            //    {
            //        // ...cerchiamo di capire se il "lancio" è verso l'alto...
            //        if (e.FinalVelocities.LinearVelocity.Y < 0)
            //        {
            //            if (currentSubpage < subpagesCount - 1)
            //                ScrollToSubpage(Direction.Up);
            //            else
            //                ScrollToSubpage(null);
            //        }
            //        // ...o verso il basso
            //        else
            //        {
            //            if (currentSubpage > 0)
            //                ScrollToSubpage(Direction.Down);
            //            else
            //                ScrollToSubpage(null);
            //        }
            //    }
            //    // se invece il movimento non è inerziale, dobbiamo capire dove abbiamo lasciato il dito e se rientra nella tolleranza
            //    else
            //    {
            //        double initialScrollingOffset = PORTRAIT_WORKING_AREA_HEIGHT * currentSubpage;

            //        if (Math.Abs(LogicalScrollViewer.VerticalOffset - initialScrollingOffset) < SCROLLING_TOLLERANCE)
            //            ScrollToSubpage(null);
            //        else
            //        {
            //            if (LogicalScrollViewer.VerticalOffset > initialScrollingOffset)
            //                ScrollToSubpage(Direction.Up);
            //            else
            //                ScrollToSubpage(Direction.Down);
            //        }
            //    }

            // prendiamo in considerazione lo scrolling orizzontale...
            if (scrollingOrientation == System.Windows.Controls.Orientation.Horizontal)
            {
                // se il movimento è inerziale...
                if (e.IsInertial)
                {
                    // ...cerchiamo di capire se il "lancio" è verso dx...
                    if (e.FinalVelocities.LinearVelocity.X < 0)
                    {
                        if (currentSubpage < subpagesCount - 1)
                            ScrollToPage(Direction.Right);
                        else
                            ScrollToPage(null);
                    }
                    // ...o verso sx
                    else
                    {
                        if (currentSubpage > 0)
                            ScrollToPage(Direction.Left);
                        else
                            ScrollToPage(null);
                    }
                }
                // se invece il movimento non è inerziale, dobbiamo capire dove abbiamo lasciato il dito e se rientra nella tolleranza
                else
                {
                    double initialScrollingOffset = PORTRAIT_WORKING_AREA_WIDTH * currentSubpage;

                    if (Math.Abs(LogicalScrollViewer.HorizontalOffset - initialScrollingOffset) < SCROLLING_TOLLERANCE)
                        ScrollToPage(null);
                    else
                    {
                        if (LogicalScrollViewer.VerticalOffset > initialScrollingOffset)
                            ScrollToPage(Direction.Left);
                        else
                            ScrollToPage(Direction.Right);
                    }
                }
            }
        }

        // gestisce lo scrolling su/giù/null
        //void ScrollToSubpage(Direction? scrollingDirection)
        //{
        //    IsHitTestVisible = false;
        //    int step = 0;

        //    switch (scrollingDirection)
        //    {
        //        case Direction.Up:
        //            step = 1;
        //            break;

        //        case Direction.Down:
        //            step = -1;
        //            break;

        //        case null:
        //            step = 0;
        //            break;
        //    }

        //    verticalScrollingImageAnimation.From = LogicalScrollViewer.VerticalOffset;
        //    verticalScrollingImageAnimation.To = PORTRAIT_WORKING_AREA_HEIGHT * (currentSubpage + step);

        //    verticalScrollingStoryboard.Begin();
        //}

        // gestisce lo scrolling sx/dx/null
        void ScrollToPage(Direction? scrollingDirection)
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

                case null:
                    step = 0;
                    break;
            }

            horizontalScrollingImageAnimation.From = LogicalScrollViewer.HorizontalOffset;
            horizontalScrollingImageAnimation.To = PORTRAIT_WORKING_AREA_WIDTH * (currentSubpage + step);

            horizontalScrollingStoryboard.Begin();
        }

        //void VerticalScrollingStoryboard_Completed(object sender, EventArgs e)
        //{
        //    IsHitTestVisible = true;
        //    currentSubpage = (int)LogicalScrollViewer.VerticalOffset / PORTRAIT_WORKING_AREA_HEIGHT;
        //}

        void HorizontalScrollingStoryboard_Completed(object sender, EventArgs e)
        {
            IsHitTestVisible = true;
            currentSubpage = (int)LogicalScrollViewer.HorizontalOffset / PORTRAIT_WORKING_AREA_WIDTH;
        }

    }
}
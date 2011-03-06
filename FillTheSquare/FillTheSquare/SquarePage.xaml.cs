using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using System.Windows.Media.Animation;
using System.Linq;
using WPCommon;

namespace FillTheSquare
{
    public partial class SquarePage : PhoneApplicationPage
    {
        public MagicSquare Square;
        DispatcherTimer dt;
        Stopwatch sw;
        bool end;

        public SquarePage()
        {
            InitializeComponent();
            end = false;
            InitializeTimers();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var size = int.Parse(NavigationContext.QueryString["size"]);
            Square = new MagicSquare(size);
            for (int i = 0; i < size; i++)
            {
                MagicGrid.RowDefinitions.Add(new RowDefinition());
                MagicGrid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < size; j++)
                {
                    var b = new Border()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(5),
                        BorderBrush = new SolidColorBrush(Colors.White),
                    };
                    b.SetRow(i);
                    b.SetColumn(j);
                    b.MouseLeftButtonDown += Button_Click;
                    MagicGrid.Children.Add(b);
                }
            }
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            TimeElapsedTextBlock.Text = "Seconds: " + sw.Elapsed.TotalSeconds.ToString("0");
        }

        private void InitializeTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
            sw = new Stopwatch();
            sw.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (end) return;

            var currentButton = (Border)sender;
            Point p = new Point(currentButton.GetColumn(), currentButton.GetRow());

            //vorrei eliminare questa variabile semplificando la logica (ci sono troppi if!)
            bool cancel = false;
            if (Square.positionHistory.Count != 0)
            {
                if (p == Square.positionHistory.Peek())
                {
                    currentButton.Child = null;
                    cancel = true;
                }
            }

            bool res = Square.PressButton(p);
            if (res && !cancel)
            {
                currentButton.Child = new TextBlock()
                {
                    Text = Square.positionHistory.Count.ToString(),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 30
                };

                Completed.Stop();
                Storyboard.SetTarget(Completed, currentButton);
                Settings.MoveSound.Play();
                Completed.Begin();

                if (Square.positionHistory.Count == (MagicGrid.RowDefinitions.Count * MagicGrid.ColumnDefinitions.Count))
                {
                    dt.Stop();
                    MessageBox.Show("Congratulations! Magic Square completed in " + sw.Elapsed.TotalSeconds + " seconds!");
                    sw.Reset();
                    Settings.ErrorSound.Play();
                    end = true;
                    Settings.RecordsList.Add(new Record(Square.ActualSize, DateTime.Now, sw.Elapsed.TotalSeconds));
                }
            }
            else if (res && cancel)
            {
                Completed.Stop();
                if (Square.positionHistory.Count > 0)
                {
                    var lastButton = MagicGrid.Children
                        .Where(b => b.GetRow() == Square.positionHistory.Peek().Y)
                        .Where(b => b.GetColumn() == Square.positionHistory.Peek().X).Single();

                    Storyboard.SetTarget(Completed, lastButton);
                    Completed.Begin();
                }
                Settings.UndoSound.Play();
            }
            else
            {
                //l'utente ha violato le regole, faccio un flash rosso sul tasto
                RedFlash.Stop();
                Storyboard.SetTarget(RedFlash, currentButton);
                Settings.ErrorSound.Play();
                RedFlash.Begin();
            }
        }

        private void BackgroundMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var me = (MediaElement)sender;
            me.Stop();
            me.Play();
        }

    }
}
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
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace FillTheSquare
{
    public partial class TenSquarePage : PhoneApplicationPage
    {
        public MagicSquare TenSquare;
        public TimeSpan TimeLeft;
        DispatcherTimer dt;
        private int seconds;
        bool end;
        public TenSquarePage()
        {
            InitializeComponent();
            TenSquare = new MagicSquare(SquareSize.Ten);
            seconds = -1;
            end = false;
            InitializeTimer();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            dt.Start();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            dt.Stop();
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            seconds++;
            secondsTextBlock.Text = seconds.ToString();
        }

        private void InitializeTimer()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt_Tick(null, EventArgs.Empty);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (end)
            {
                return;
            }
            var currentButton = (Button)sender;
            Point p = new Point(currentButton.GetColumn(), currentButton.GetRow());
            bool cancel = false;
            if (p.Equals(TenSquare.actualPosition))
            {
                currentButton.Content = "";
                cancel = true;
            }
            bool res = TenSquare.PressButton(p);
            if (res && !cancel)
            {
                currentButton.Content = TenSquare.actualValue.ToString();
                currentButton.Background = new SolidColorBrush(Colors.Green);
                if (TenSquare.actualValue == 100)
                {
                    //TODO aggiungere il punteggio nei records
                    dt.Stop();
                    MessageBox.Show("Congratulations! 10 x 10 Magic Square completed in " + seconds + " seconds!");
                    end = true;
                }
            }
            else if (res && cancel)
            {
                currentButton.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                //l'utente ha violato le regole quindi non si fa nulla, magari coloro per mezzo secondo il tasto di rosso?
            }
        }
    }
}
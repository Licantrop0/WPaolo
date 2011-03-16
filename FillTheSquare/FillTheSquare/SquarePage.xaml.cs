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

        public SquarePage()
        {
            InitializeComponent();
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
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Colors.White),
                    };
                    b.SetRow(i);
                    b.SetColumn(j);
                    b.MouseLeftButtonDown += Button_Click;
                    MagicGrid.Children.Add(b);
                }
            }
        }

        private void InitializeTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += (sender, e) =>
                { TimeElapsedTextBlock.Text = "Seconds " + sw.Elapsed.TotalSeconds.ToString("000"); };
            dt.Start();

            sw = new Stopwatch();
            sw.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (WPCommon.TrialManagement.IsTrialMode && sw.Elapsed.TotalSeconds >= 90)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return;
            }


            var currentButton = (Border)sender;
            var p = new GridPoint(currentButton.GetColumn(), currentButton.GetRow());

            //tutta la logica della griglia è dentro il metodo PressButton
            var result = Square.PressButton(p);

            if (result == true)     //caso creazione andato a buon fine
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
                Completed.Begin();
                Settings.MoveSound.Play();

                if (Square.GetMovesLeft() == 0)  //non ci sono più mosse disponibili
                {
                    //PhilPiangeAppear.Stop();
                    //PhilPiangeDisappear.Stop();
                    //TODO: Inserire suono di disperazione
                    PhilPiangeAppear.Begin();
                }

                if (Square.IsCompleted)
                {
                    dt.Stop();
                    Settings.VictorySound.Play();

                    //TODO: inserire il proprio nome e nascondere il risultato
                    MessageBox.Show("Congratulations! Magic Square completed in "
                        + sw.Elapsed.TotalSeconds.ToString("0.00") + " seconds!");
                    Settings.Records.Add(new Record(Square.Size, DateTime.Now, sw.Elapsed));
                    NavigationService.Navigate(new Uri("/RecordsPage.xaml", UriKind.Relative));
                }
            }
            else if (result == false) //caso di creazione fallito
            {
                RedFlash.Stop();
                Storyboard.SetTarget(RedFlash, currentButton);
                RedFlash.Begin();
                Settings.ErrorSound.Play();
            }
            else if (result == null) //caso di cancellazione
            {
                currentButton.Child = null;

                //PhilPiangeDisappear.Stop();
                //PhilPiangeAppear.Stop();
                PhilPiangeDisappear.Begin();

                //Evidenzio la casella sull'ultimo premuto
                var lastValue = Square.positionHistory.Peek();
                var lastButton = MagicGrid.Children
                    .Where(b => b.GetRow() == lastValue.Y)
                    .Where(b => b.GetColumn() == lastValue.X).First();

                Completed.Stop();
                Storyboard.SetTarget(Completed, lastButton);
                Completed.Begin();
                Settings.UndoSound.Play();
            }
        }

        private void BackgroundMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var me = (MediaElement)sender;
            me.Stop();
            me.Play();
        }

        private void ResetPage()
        {
            PhilPiangeDisappear.Stop();
            PhilPiangeDisappear.Begin();

            sw.Reset();
            sw.Start();

            Square.Clear();
            MagicGrid.Children
                .Where(ctrl => ctrl is Border)
                .Cast<Border>()
                .ForEach(b => b.Child = null);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetPage();
        }
    }
}
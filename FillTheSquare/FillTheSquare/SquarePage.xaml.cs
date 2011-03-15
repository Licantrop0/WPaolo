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
        Stopwatch TrialWatch;
        bool end;

        public SquarePage()
        {
            InitializeComponent();
            end = false;
            InitializeTimers();
        }

        private void creagriglia(int size)
        {
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var size = int.Parse(NavigationContext.QueryString["size"]);
            creagriglia(size);
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            TimeElapsedTextBlock.Text = "Seconds " + sw.Elapsed.TotalSeconds.ToString("0");
        }

        private void InitializeTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
            sw = new Stopwatch();
            sw.Start();

            if (WPCommon.TrialManagement.IsTrialMode)
            {
                TrialWatch = new Stopwatch();
                TrialWatch.Start();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (end) return;

            if (WPCommon.TrialManagement.IsTrialMode)
            {
                if (TrialWatch.Elapsed.Seconds >= 10)
                {
                    //TODO: qui dovresti far andare l'utente in una nuova pagina dove c'è phil triste e
                    //il pulsante che punta al marketplace per acquistare l'app.
                    //scrivici una cazzata tipo "fai felice phil e compra l'app!"
                    MessageBox.Show("Please buy the full version!");
                    ResetPage();
                }
            }

            var currentButton = (Border)sender;
            Point p = new Point(currentButton.GetColumn(), currentButton.GetRow());

            //controllo se è un'azione di cancellazione di una casella o di un'aggiunta: se il tasto premuto è uguale all'ultimo
            //nella history vuol dire che sto cancellando
            bool cancel = false;
            if (Square.positionHistory.Count != 0)
            {
                if (p == Square.positionHistory.Peek())
                {
                    currentButton.Child = null;
                    cancel = true;
                }
            }

            //tutta la logica della griglia è dentro il metodo PressButton
            bool res = Square.PressButton(p);
            if (res && !cancel)     //caso creazione andato a buon fine
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

                if (Square.NumberMovesLeft() == 0)  //non ci sono più mosse disponibili
                {
                    PhilPiangeAppear.Stop();
                    PhilPiangeDisappear.Stop();
                    PhilPiangeAppear.Begin();
                }

                if (Square.positionHistory.Count == (MagicGrid.RowDefinitions.Count * MagicGrid.ColumnDefinitions.Count))
                {
                    dt.Stop();
                    Settings.VictorySound.Play();
                    MessageBox.Show("Congratulations! Magic Square completed in " + sw.Elapsed.TotalSeconds + " seconds!");
                    sw.Reset();
                    end = true;
                    Settings.RecordsList.Add(new Record(Square.ActualSize, DateTime.Now, sw.Elapsed.TotalSeconds));
                    NavigationService.Navigate(new Uri("/RecordsPage.xaml", UriKind.Relative));
                }
            }
            else if (res && cancel) //caso di cancellazione andato a buon fine
            {
                PhilPiangeDisappear.Stop();
                PhilPiangeAppear.Stop();
                PhilPiangeDisappear.Begin();

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
            else                    //caso di creazione fallito
            {
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

        private void ResetPage()
        {
            PhilPiangeDisappear.Stop();
            PhilPiangeAppear.Stop();
            PhilPiangeDisappear.Begin();
            sw.Reset();
            sw.Start();
            if (WPCommon.TrialManagement.IsTrialMode)
            {
                TrialWatch.Reset();
                TrialWatch.Start();
            }
            Square.Reset();
            int size = MagicGrid.RowDefinitions.Count;
            MagicGrid.Children.Clear();
            MagicGrid.RowDefinitions.Clear();
            MagicGrid.ColumnDefinitions.Clear();
            creagriglia(size);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetPage();
        }
    }
}
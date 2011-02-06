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
using Bao;

namespace BaoGame
{
    public enum MatchState
    {
        initialization,
        player1EvaluatingPossibleMoves,
        player1PickFirstShimo,
        player1PickSecondShimo,
        player1Move,
        cpuMove
    }

    public enum FirstMove
    {
        random,
        player1,
        CPU
    }

    public partial class GamePage : PhoneApplicationPage
    {
        # region private members

        GameType _gameType;
        FirstMove _firstMove;
        BaoGameState _actualGameState;
        MatchState _state;
        List<BaoMove> _player1PossibleMoves;
        MustMove _player1MustMove;
        Shimo _player1FirstShimo;
        BaoMove _player1Move;
        List<ShimoBaoMove> _possibleSecondShimoBaoMove;
        ImageSource[] _imageSourceArray;

        #endregion


        public GamePage()
        {
            InitializeComponent();

            _firstMove = FirstMove.random;

            _imageSourceArray = new ImageSource[9];

            for (int i = 0; i < 9; i++)
            {
                _imageSourceArray[i] = new ImageSourceConverter().ConvertFromString("img/semini_" + i.ToString() + ".png") as ImageSource; 
            }       
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            int rowCount = 4;
            int colCount = 8;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 1; j <= colCount; j++)
                {
                    var b = new Border()
                    {
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(0),
                        BorderBrush = new SolidColorBrush(Colors.Transparent),
                        CornerRadius = new CornerRadius(0)
                    };

                    b.SetRow(i);
                    b.SetColumn(j);
                    b.MouseLeftButtonDown += Shimo_Click;
                    BoardGrid.Children.Add(b);

                    Image boxImage = new Image();
                    boxImage.HorizontalAlignment = HorizontalAlignment.Center;
                    boxImage.VerticalAlignment = VerticalAlignment.Center;

                    if ((i == 1 && j == 4) || (i == 2 && j == 5))
                        boxImage.Source = new ImageSourceConverter().ConvertFromString("img/nyumba_85x85.png") as ImageSource;
                    else
                        boxImage.Source = new ImageSourceConverter().ConvertFromString("img/shimo_85x85.png") as ImageSource;

                    ImageBrush dummy = new ImageBrush();
                    dummy.ImageSource = boxImage.Source;
                    b.Background = dummy;

                    Image seedImage = new Image();
                    seedImage.HorizontalAlignment = HorizontalAlignment.Center;
                    seedImage.VerticalAlignment = VerticalAlignment.Center;
                    seedImage.Stretch = Stretch.None;
                    //seedImage.Source = new ImageSourceConverter().ConvertFromString("img/semini_2.png") as ImageSource;
                    b.Child = seedImage;
                }
            }

            // decide whether LaKujiFunza or LaKiswahili will be played (TODO: later must be taken from Settings Page)
            _gameType = GameType.laKiswahili;

            _state = MatchState.initialization;
            Game();
        }

        private void Game()
        {
            bool gameCall = false;

            switch (_state)
            {
                case MatchState.initialization:

                    Byte playerToMove;

                    // decide wheter player 1 or cpu will move first
                    switch (_firstMove)
                    {
                        case FirstMove.random:
                            Random randNum = new Random(DateTime.Now.Millisecond);
                            playerToMove = (Byte)randNum.Next(1, 1);        // TODO: per adesso forzo il giocatore ad effettuare la prima mossa...
                            break;

                        case FirstMove.player1:
                            playerToMove = 1;
                            break;

                        case FirstMove.CPU:
                            playerToMove = 2;
                            break;

                        default:
                            playerToMove = 1;
                            break;
                    }

                    if (playerToMove == 1)
                    {
                        _actualGameState = new BaoGameState(_gameType, 2);
                        _state = MatchState.player1EvaluatingPossibleMoves;
                    }
                    else
                    {
                        _actualGameState = new BaoGameState(_gameType, 1);
                        _state = MatchState.cpuMove;
                    }

                    if (_gameType == GameType.laKiswahili)
                    {
                        player1BarnTextBlock.Text = "22";
                        player1BarnTextBlock.Visibility = Visibility.Visible;
                        cpuBarnTextBlock.Text = "22";
                        cpuBarnTextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        player1BarnTextBlock.Visibility = Visibility.Collapsed;
                        cpuBarnTextBlock.Visibility = Visibility.Collapsed;
                    }

                    DrawShimos();

                    gameCall = true;
                    break;
                   
                case MatchState.player1EvaluatingPossibleMoves:

                    // ask game engine all valid moves for player 1
                    _player1PossibleMoves = _actualGameState.GetPossibleMoves(1, out _player1MustMove);
                    _state = MatchState.player1PickFirstShimo;
                    gameCall = true;
                    break;

                case MatchState.player1PickFirstShimo:

                    // player must pick the first shimo...
                    messageBox.Text = "Pick first shimo!";

                    break;
             
                case MatchState.player1PickSecondShimo:

                    // payer must pick the second shimo
                    messageBox.Text = "Now pick the second shimo!";

                    break;

                case MatchState.player1Move:

                    bool askPlayNyumba = false;
                    List<ScreenAction> sa = _actualGameState.ExecutePlayerMove(_player1Move, 1, out askPlayNyumba);

                    GraphicEngine(sa);

                    break;
            }

            if (gameCall)
                Game();
        }

       

        
        private void Shimo_Click(object sender, RoutedEventArgs e)
        {
            Border currentShimo = (Border)sender;
            Byte row = (Byte)currentShimo.GetRow();
            Byte col = (Byte)currentShimo.GetColumn();

            bool gameCall = false;
            switch(_state)
            {
                case MatchState.player1PickFirstShimo:

                    if (row < 2)
                    {
                        messageBox.Text = "You must select in your own territory";
                        return;
                    }

                    bool shimoAllowed = false;
                    BaoMove tmp = new BaoMove(row, col, false);
                    foreach (BaoMove m in _player1PossibleMoves)
                    {
                        if (tmp.ShimoEqual(m))
                        {
                            shimoAllowed = true;
                            break;
                        }
                    }

                    if (shimoAllowed)
                    {
                        _player1FirstShimo.Set(row, col);
                        _possibleSecondShimoBaoMove = _actualGameState.GetPossibleSecondShimo(_player1FirstShimo, 1, _player1PossibleMoves, _player1MustMove);

                        if (_possibleSecondShimoBaoMove.Count == 0)
                        {
                            // this is namua capture from kitwa - kimbi shimos, I istantiate the move to be executed!
                            _player1Move = new BaoMove(row, col, false);
                            _state = MatchState.player1Move;
                            gameCall = true;
                        }
                        else
                        {
                            // options will be evaluated later
                            _state = MatchState.player1PickSecondShimo;
                            gameCall = true;
                        }   
                    }
                    else
                    {
                        // explain the reasons why player cannot pick this shimo
                        // 1) He does not take an empty shimo
                        // 2) Othere reasons depend on mustMove variable
                        if (_actualGameState.Board[row, col] == 0)
                        {
                            messageBox.Text = "You cannot pick an empty shimo!";
                            return;
                        }
                        else
                        {
                            switch(_player1MustMove)
                            {
                                case MustMove.namuaCapture:
                                case MustMove.mtagiCapture:
                                    messageBox.Text = "You must capture if you can!";
                                    return;

                                case MustMove.namuaTakasaNonSingletonNonNyumba:
                                case MustMove.namuaLonelyShimoTakasaAwayException:
                                    if (row == 3)
                                    {
                                        messageBox.Text = "You must select from inner row if you can!";
                                        return;
                                    }
                                    else if (row == 2 && col == 5)
                                    {
                                        messageBox.Text = "You cannot pick you nyumba now!";
                                        return;
                                    }
                                    else
                                    {
                                        messageBox.Text = "You must select a non singleton shimo if you can!";
                                        return;
                                    }
                                case MustMove.namuaTakasaSingleton:
                                     messageBox.Text = "You must select from inner row if you can!";
                                     return;
                                case MustMove.namuaTaxRule:
                                     messageBox.Text = "Namua tax rule is the only option!";
                                     return;
                                case MustMove.mtagiInnerRowTakasa:
                                     if (row == 3)
                                     {
                                         messageBox.Text = "You must select from inner row if you can!";
                                         return;
                                     }
                                     else
                                     {
                                         messageBox.Text = "You must select a non singleton shimo!";
                                         return;
                                     }
                                case MustMove.mtagiOuterRowTakasa:
                                    messageBox.Text = "You must select a non singleton shimo!";
                                    return;
                            }
                        }
                    }

                    break;

                case MatchState.player1PickSecondShimo:

                    if (row < 2)
                    {
                        messageBox.Text = "You must select in your own territory";
                        return;
                    }
                    else
                    {
                        bool secondShimoAllowed = false;
                        Shimo actualShimo = new Shimo(row, col);
                        foreach (ShimoBaoMove sbm in _possibleSecondShimoBaoMove)
                        {
                            if (actualShimo.row == sbm.shimo.row && actualShimo.col == sbm.shimo.col)
                            {
                                // ok, I found the move
                                _player1Move = sbm.baoMove;
                                _state = MatchState.player1Move;
                                gameCall = true;
                                break;
                            }
                        }

                        if (!secondShimoAllowed)
                        {
                            messageBox.Text = "Shimo not allowed, pick another one!";
                        }
                    }                                                   

                    break;

                default:
                    break;
            }

            if (gameCall)
                Game();
        }

        private void DrawShimos()
        {
            // TODO

            foreach (Border b in BoardGrid.Children)
            {
                int row = b.GetRow();
                int col = b.GetColumn();

                Image img = b.Child as Image;

                img.Source = _imageSourceArray[_actualGameState.Board[row, col]];
            }

            return;
        }


        private void GraphicEngine(List<ScreenAction> sa)
        {
            foreach (ScreenAction s in sa)
            {
                switch (s.Action)
                {
                    case ActionType.askPlayNyumba:

                        break;

                    case ActionType.barnPicked:

                        break;

                    case ActionType.captureUnderlined:

                        break;

                    case ActionType.relaySowingUnderlined:

                        break;

                    case ActionType.startMtajiMoveUnderlined:

                        break;

                    case ActionType.startNamuaMoveUnderlined:

                        break;

                    case ActionType.sowOneSeed:

                        break;

                    case ActionType.pickOpponentSeeds:

                        break;

                    case ActionType.pickOwnSeeds:

                        break;

                    case ActionType.pickTwoSeeds:

                        break;

                    case ActionType.playNyumba:

                        break;

                    case ActionType.stayNyumba:

                        break;

                    default:
                        break;
                }
            }
        }

    }
}
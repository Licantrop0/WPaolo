using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Bao;
using Microsoft.Phone.Controls;
using MiniMaxTree;
using WPCommon;

namespace BaoGame
{
    public enum MatchState
    {
        initialization,
        playerEvaluatingPossibleMoves,
        playerMustPickShimo,
        playerPickedShimo,
        playerMustPickSecondShimo,
        playerPickedSecondShimo,
        playerAskedToPlayNyumba,
        playerPlayNyumba,
        playerMove,
        testWinCondition,
        cpuMove,
        end
    }

    public enum GameModality
    {
        playerVsCpu,
        playerVsPlayer
    }

    public enum FirstMove
    {
        random,
        player1,
        player2
    }

    public struct ShimoUtility
    {
        public Shimo underlinedShimo;

        public bool allowed;

        public Shimo sowingShimo;

        public string message;

        public bool chosen;
    }

    public partial class GamePage : PhoneApplicationPage
    {
        # region private members

        // game states variables
        GameModality _gameModality;
        GameType _gameType;
        FirstMove _firstMove;
        BaoGameState _actualGameState;
        MatchState _state;
        Byte _playerTurn;
        int _numKeteInHand;
        List<BaoMove> _playerPossibleMoves;
        MustMove _playerMustMove;
        Shimo _playerShimo;
        ShimoUtility _shimoUtilityLeft;
        ShimoUtility _shimoUtilityRight;
        Byte _winner;

        // animation engine state variables
        Queue<ScreenAction> _sa;
        ScreenAction _handeldAction;
        DispatcherTimer _actionTimer;
        bool _bGraphicEngineRunning;

        // graphical resources
        ImageSource[] _imageSourceArray;

        AbsAI _ai;

        #endregion

        public GamePage()
        {
            InitializeComponent();

            _firstMove = FirstMove.random;

            _imageSourceArray = new ImageSource[21];

            for (int i = 0; i < 21; i++)
            {
                _imageSourceArray[i] = new ImageSourceConverter().ConvertFromString("img/semini_" + i.ToString() + ".png") as ImageSource; 
            }

            _sa = new Queue<ScreenAction>();

            _actionTimer = new DispatcherTimer();
            _actionTimer.Tick += new EventHandler(ActionHandler);
            _actionTimer.Stop();

            _bGraphicEngineRunning = false;

            // Initialize AI
            _ai = new AbsAI();
            _ai.Pli = 7;
            _ai.Level = 1;  // 0 very hard - 1 hard - 2 medium - 3 easy
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            int rowCount = 4;
            int colCount = 8;

            // initialize board graphics
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
                    b.Child = seedImage;
                }
            }

            // decide whether LaKujiFunza or LaKiswahili will be played (TODO: later must be taken from Settings Page)
            //_gameType = GameType.laKiswahili;
            _gameType = GameType.laKujifunza;

            // decide wheter pvp or pvc will be played (TODO: later must be taken from Settings Page)
            //_gameModality = GameModality.playerVsPlayer;
            _gameModality = GameModality.playerVsCpu;

            _state = MatchState.initialization;
            Game();
        }

        private void Game()
        {
            bool gameCall = false;

            switch (_state)
            {
                case MatchState.initialization:     // pvp ok

                    Byte playerToMove;

                    // decide wheter player 1 or player 2 will move first
                    switch (_firstMove)
                    {
                        case FirstMove.random:
                            Random randNum = new Random(DateTime.Now.Millisecond);
                            playerToMove = (Byte)randNum.Next(1, 1);        // TODO: per adesso forzo il giocatore 1 ad effettuare la prima mossa...
                            break;

                        case FirstMove.player1:
                            playerToMove = 1;
                            break;

                        case FirstMove.player2:
                            playerToMove = 2;
                            break;

                        default:
                            playerToMove = 1;
                            break;
                    }

                    if (playerToMove == 1)
                    {
                        _actualGameState = new BaoGameState(_gameType, 2);
                        _state = MatchState.playerEvaluatingPossibleMoves;
                        _playerTurn = 1;
                    }
                    else
                    {
                        _actualGameState = new BaoGameState(_gameType, 1);
                        _playerTurn = 2;

                        if (_gameModality == GameModality.playerVsCpu)
                        {
                            _state = MatchState.cpuMove;
                        }
                        else
                        {
                            _state = MatchState.playerEvaluatingPossibleMoves;
                        }
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

                    _winner = 0;

                    DrawShimos();

                    gameCall = true;
                    break;
                   
                case MatchState.playerEvaluatingPossibleMoves:  // pvp ok

                    // ask game engine all valid moves for player
                    _playerPossibleMoves = _actualGameState.GetPossibleMoves(_playerTurn, out _playerMustMove);
                    _state = MatchState.playerMustPickShimo;
                    gameCall = true;
                    break;

                case MatchState.playerMustPickShimo:    // pvp ok

                    // player must pick the first shimo...
                    messageBox.Text = "Player " + _playerTurn.ToString() + ", pick a shimo!";

                    break;
             
                case MatchState.playerPickedShimo:  // pvp ok

                    _numKeteInHand = 0;

                    // Execute player pick shimo, update state and execute animations
                    _sa = _actualGameState.ExecutePlayerPickShimo(_playerShimo.row, _playerShimo.col, _playerTurn, _playerMustMove, out _numKeteInHand);
                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));
                    GraphicEngine(true);

                    // calculate utility structure (sowing shimo - sowing direction - underlined shimo - allowed - message                    
                    bool bSecondShimoChose = true;

                    Shimo underlinedShimo;

                    _shimoUtilityLeft.allowed = false;
                    _shimoUtilityRight.allowed = false;
                    _shimoUtilityLeft.chosen = false;
                    _shimoUtilityRight.chosen = false;

                    Byte kimbiRow = _playerTurn == 1 ? (Byte)2 : (Byte)1;

                    if( _playerMustMove == MustMove.namuaCapture )
                    {
                        // namua capture case
                        // if first shimo is kitchwa or kimbi sowing shimo is automaticcally chosen
                        if (_playerShimo.col <= 2)
                        {
                            _shimoUtilityRight.allowed = true;
                            _shimoUtilityRight.sowingShimo = new Shimo(kimbiRow, 0);
                            _shimoUtilityRight.chosen = true;
                            bSecondShimoChose = false;
                        }
                        else if (_playerShimo.col >= 7)
                        {
                            _shimoUtilityLeft.allowed = true;
                            _shimoUtilityLeft.sowingShimo = new Shimo(kimbiRow,9);
                            _shimoUtilityLeft.chosen = true;
                            bSecondShimoChose = false;
                        }
                        else
                        {
                            // seconds shimo allowed are left are right kitchwa, no possibility of error
                            _shimoUtilityRight.allowed = true;
                            _shimoUtilityRight.sowingShimo = new Shimo(kimbiRow, 0);
                            underlinedShimo = new Shimo(kimbiRow,1);
                            _shimoUtilityRight.underlinedShimo = underlinedShimo;
                            UnderlineShimo(underlinedShimo, 1);

                            _shimoUtilityLeft.allowed = true;
                            _shimoUtilityLeft.sowingShimo = new Shimo(kimbiRow, 9);
                            underlinedShimo = new Shimo(kimbiRow,8);
                            _shimoUtilityLeft.underlinedShimo = underlinedShimo;
                            UnderlineShimo(underlinedShimo, 1);

                            bSecondShimoChose = true;
                        }
                    }
                    else
                    {
                        // if not namua capture, sowing shimo is current shimo, sowing direction can be either left, rigt or both
                        // it depends on player1 possible moves
                        BaoMove tentative;

                        // underline shimos are the one next to sowing shimo
                        _shimoUtilityLeft.sowingShimo = _playerShimo;
                        _shimoUtilityLeft.underlinedShimo = _actualGameState.NextShimo(_shimoUtilityLeft.sowingShimo, true);
                        UnderlineShimo(_shimoUtilityLeft.underlinedShimo, 2);
                        tentative = new BaoMove(_playerShimo.row, _playerShimo.col, true);
                        foreach (BaoMove m in _playerPossibleMoves)
                        {
                            if (tentative.rowBox == m.rowBox && tentative.columnBox == m.columnBox && tentative.left == m.left)
                            {
                                _shimoUtilityLeft.allowed = true;
                                _shimoUtilityLeft.message = "";
                                break;
                            }
                        }
                        if (!_shimoUtilityLeft.allowed)
                        {
                            // spiegare perchè non è concesso con il must move...
                            switch (_playerMustMove)
                            {
                                case MustMove.namuaLonelyShimoTakasaAwayException:
                                case MustMove.mtagiLonelyShimoTakasaAwayException:
                                    _shimoUtilityLeft.message = "You cannot leave your inner row empty, chose the other direction!";
                                    break;

                                case MustMove.mtagiCapture:
                                    _shimoUtilityLeft.message = "You must chose the other direction!";
                                    break;

                                default:
                                    Debug.Assert(false);    // must never get here!!!
                                    break;
                            }   
                        }

                        _shimoUtilityRight.sowingShimo = _playerShimo;
                        _shimoUtilityRight.underlinedShimo = _actualGameState.NextShimo(_shimoUtilityRight.sowingShimo, false);
                        UnderlineShimo(_shimoUtilityRight.underlinedShimo, 2);
                        tentative = new BaoMove(_playerShimo.row, _playerShimo.col, false);
                        foreach (BaoMove m in _playerPossibleMoves)
                        {
                            if (tentative.rowBox == m.rowBox && tentative.columnBox == m.columnBox && tentative.left == m.left)
                            {
                                _shimoUtilityRight.allowed = true;
                                _shimoUtilityRight.message = "";
                                break;
                            }
                        }
                        if (!_shimoUtilityRight.allowed)
                        {
                            // spiegare perchè non è concesso con il must move...
                            switch (_playerMustMove)
                            {
                                case MustMove.namuaLonelyShimoTakasaAwayException:
                                case MustMove.mtagiLonelyShimoTakasaAwayException:
                                    _shimoUtilityRight.message = "You cannot leave your inner row empty, chose the other direction!";
                                    break;

                                case MustMove.mtagiCapture:
                                    _shimoUtilityRight.message = "You must chose the other direction in order to capture!";
                                    break;

                                default:
                                    Debug.Assert(false);    // must never get here!!!
                                    break;
                            }
                        }
                   }

                    if (bSecondShimoChose)
                    {
                        _state = MatchState.playerMustPickSecondShimo;
                    }
                    else
                    {
                        _state = MatchState.playerPickedSecondShimo;
                    }

                    break;

                case MatchState.playerMustPickSecondShimo:  // pvp ok

                    messageBox.Text = "Now sow the seeds!";

                    break;

                case MatchState.playerPickedSecondShimo:    // pvp ok

                    bool askPlayNyumba = false;

                    if (_shimoUtilityLeft.chosen == true)
                    {
                        _sa = _actualGameState.ExecutePlayerMove(_shimoUtilityLeft.sowingShimo.row, _shimoUtilityLeft.sowingShimo.col, true, _playerTurn,
                            _numKeteInHand, _playerMustMove, out askPlayNyumba);
                    }
                    else if (_shimoUtilityRight.chosen == true)
                    {
                        _sa = _actualGameState.ExecutePlayerMove(_shimoUtilityRight.sowingShimo.row, _shimoUtilityRight.sowingShimo.col, false, _playerTurn,
                            _numKeteInHand, _playerMustMove, out askPlayNyumba);
                    }
                    else
                    {
                        Debug.Assert(false);    // must never get here!!!
                    }
                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));

                    GraphicEngine(true);

                    if (askPlayNyumba)
                    {
                        _state = MatchState.playerAskedToPlayNyumba;
                    }
                    else
                    {
                        SwitchTurn();
                        _state = MatchState.testWinCondition;
                    }

                    break;

                case MatchState.playerAskedToPlayNyumba:    // pvp ok

                    messageBox.Text = "Play Nyumba?";
                    playNyumbaYesButton.Visibility = Visibility.Visible;
                    playNyumbaNoButton.Visibility = Visibility.Visible;

                    break;

                case MatchState.playerPlayNyumba:   // pvp ok

                    if (_shimoUtilityLeft.chosen == true)
                        _sa = _actualGameState.PlayNyumba(true, _playerTurn);
                    else
                        _sa = _actualGameState.PlayNyumba(false, _playerTurn);

                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));

                    SwitchTurn();
                    _state = MatchState.testWinCondition;

                    GraphicEngine(true);

                    break;
                    
                // quando arrivo qua il turno è già switchato...
                case MatchState.testWinCondition:   // pvp ok

                    WinCondition wc = _actualGameState.WinLoseCondition((Byte)_playerTurn);
                    if (wc == WinCondition.Player1)
                    {
                        _state = MatchState.end;
                        _winner = 1;
                    }
                    else if (wc == WinCondition.Player2)
                    {
                        _state = MatchState.end;
                        _winner = 2;
                    }
                    else
                    {
                        if( _gameModality == GameModality.playerVsPlayer )
                        {
                            _state = MatchState.playerEvaluatingPossibleMoves;
                        }
                        else
                        {
                            if (_playerTurn == 2)
                            {
                                _state = MatchState.cpuMove;
                            }
                            else
                            {
                                _state = MatchState.playerEvaluatingPossibleMoves;
                            }
                        }
                    }

                    gameCall = true;
          
                    break;
        
                case MatchState.cpuMove:    // pvp ok

                    messageBox.Text = "Computer is thinking...";

                    //GTree<BaoMinimaxElement> minimaxTree = new GTree<BaoMinimaxElement>(new BaoMinimaxElement(_actualGameState));
                    _ai.InitializeTree(_actualGameState);
                    GC.Collect();   // force the garbace collector to clean the memory
                    BaoMove cpuMove = _ai.MiniMax();

                    messageBox.Text = "Computer moves";

                    // update board
                    _sa = _actualGameState.ExecuteCPUMove(cpuMove, 2);
                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));

                    _playerTurn = 1;

                    _state = MatchState.testWinCondition;

                    GraphicEngine(true);

                    break;

                case MatchState.end:

                    if(_winner == 1)
                    {
                        messageBox.Text = "Player 1 wins!";
                    }
                    else if(_gameModality == GameModality.playerVsPlayer)
                    {
                        messageBox.Text = "Player 2 wins!";
                    }
                    else
                    {
                        messageBox.Text = "CPU wins!";
                    }

                    break;
            }

            if (gameCall)
                Game();
        }

        private void SwitchTurn()
        {
            if (_playerTurn == 1)
            {
                _playerTurn = 2;
            }
            else
            {
                _playerTurn = 1;
            }
        }

        private void Shimo_Click(object sender, RoutedEventArgs e)
        {
            Border currentShimo = (Border)sender;
            Byte row = (Byte)currentShimo.GetRow();
            Byte col = (Byte)currentShimo.GetColumn();

            if (_bGraphicEngineRunning)
            {
                return;
            }

            // TODO: assegnare le giuste componenti grafiche al giocatore...

            bool gameCall = false;
            switch(_state)
            {
                case MatchState.playerMustPickShimo:    // pvp ok

                    bool shimoAllowed = false;
                    BaoMove tmp = new BaoMove(row, col, false);
                    foreach (BaoMove m in _playerPossibleMoves)
                    {
                        if (tmp.ShimoEqual(m))
                        {
                            shimoAllowed = true;
                            break;
                        }
                    }

                    if (shimoAllowed)
                    {
                        _playerShimo.Set(row, col);
                        //UnderlineShimo(_playerShimo, 1);  // viene fatto dal motore grafico
                        _state = MatchState.playerPickedShimo;
                        gameCall = true;
                    }
                    else
                    {
                        // explain the reasons why player cannot pick this shimo
                        // 1) He pick a shimo in opponent territory
                        // 2) He does take an empty shimo
                        // 3) Other reasons depend on mustMove variable
                        //if (row < 2)
                        if ( (row < 2 && _playerTurn == 1) || (row > 1 && _playerTurn == 2) )
                        {
                            messageBox.Text = "You must select in your own territory";
                        }
                        else if (_actualGameState.Board[row, col] == 0)
                        {
                            messageBox.Text = "You cannot pick an empty shimo!";
                        }
                        else
                        {
                            switch(_playerMustMove)
                            {
                                case MustMove.namuaCapture:
                                case MustMove.mtagiCapture:
                                    messageBox.Text = "You must capture if you can!";
                                    break;

                                case MustMove.namuaTakasaNonSingletonNonNyumba:
                                case MustMove.namuaLonelyShimoTakasaAwayException:
                                    if ( (row == 3 && _playerTurn == 1) || (row == 0 && _playerTurn == 2) )
                                    {
                                        messageBox.Text = "You must select from inner row if you can!";
                                        break;
                                    }
                                    else if ( (row == 2 && col == 5 && _playerTurn == 1) || (row == 1 && col == 4 && _playerTurn == 2) )
                                    {
                                        messageBox.Text = "You cannot pick your nyumba now!";
                                    }
                                    else
                                    {
                                        messageBox.Text = "You must select a non singleton shimo if you can!";
                                    }
                                    break;

                                case MustMove.namuaTakasaSingleton:
                                    if ((row == 2 && col == 5 && _actualGameState.Board[2, 5] > 1) || (row == 1 && col == 4 && _actualGameState.Board[1, 4] > 1))
                                    {
                                        messageBox.Text = "You cannot pick your nyumba now!";
                                    }
                                    else
                                    {
                                        messageBox.Text = "You must select from inner row if you can!";
                                    }
                                     break;

                                case MustMove.namuaTaxRule:
                                     messageBox.Text = "Namua tax rule is the only option!";
                                     break;

                                case MustMove.mtagiInnerRowTakasa:
                                     if ((row == 3 && _playerTurn == 1) || (row == 0 && _playerTurn == 2))
                                     {
                                         messageBox.Text = "You must select from inner row if you can!";
                                     }
                                     else
                                     {
                                         messageBox.Text = "You must select a non singleton shimo!";
                                     }
                                     break;

                                case MustMove.mtagiOuterRowTakasa:
                                    messageBox.Text = "You must select a non singleton shimo!";
                                    break;
                            }
                        }
                    }

                    break;

                case MatchState.playerMustPickSecondShimo:

                    Shimo secondShimo = new Shimo(row, col);

                    if  (secondShimo == _shimoUtilityLeft.underlinedShimo)
                    {
                        if (_shimoUtilityLeft.allowed)
                        {
                            _shimoUtilityLeft.chosen = true;
                            _state = MatchState.playerPickedSecondShimo;
                            gameCall = true;
                        }
                        else
                        {
                            messageBox.Text = _shimoUtilityLeft.message;
                        }
                    }
                    else if (secondShimo == _shimoUtilityRight.underlinedShimo)
                    {
                        if (_shimoUtilityRight.allowed)
                        {
                            _shimoUtilityRight.chosen = true;
                            _state = MatchState.playerPickedSecondShimo;
                            gameCall = true;
                        }
                        else
                        {
                            messageBox.Text = _shimoUtilityRight.message;
                        }
                    }

                    break;

                default:
                    break;
            }

            if (gameCall)
                Game();
        }

        private void UnderlineShimo(Shimo _player1FirstShimo, int option)
        {
            // TODO
            return;
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

        private void DrawShimo(int row, int col, int numOfSeeds)
        {
            Border b = BoardGrid.Children[row * 8 + col - 1] as Border;

            Debug.Assert(row == b.GetRow() && col == b.GetColumn());

            Image img = b.Child as Image;

            img.Source = _imageSourceArray[numOfSeeds]; 
        }

        private void GraphicEngine(bool bFirstAction)
        {
            _bGraphicEngineRunning = true;

            if (_sa.Count > 0)
            {
                _handeldAction = _sa.Dequeue();

                if (!bFirstAction)
                {
                    _actionTimer.Interval = new TimeSpan(7000000);
                }
                else
                {
                    _actionTimer.Interval = new TimeSpan(1);
                }
                
                _actionTimer.Start();
            }
            else
            {
                _bGraphicEngineRunning = false;
                Game();
            }
        }

        private void ActionHandler(object sender, EventArgs e)
        {
            _actionTimer.Stop();

            switch (_handeldAction.Action)
            {
                case ActionType.askPlayNyumba:

                    // TODO

                    break;

                case ActionType.barnPicked:

                    // TODO: sounds effect if needed
                    if (_handeldAction.Row == 1)
                    {
                        player1BarnTextBlock.Text = _actualGameState.Barn1.ToString();
                    }
                    else
                    {
                        cpuBarnTextBlock.Text = _actualGameState.Barn2.ToString();
                    }

                    break;

                case ActionType.captureUnderlined:

                    // TODO: graphycal effect (serena o lupo...)
                    messageBox.Text = "Capture!";

                    break;

                case ActionType.relaySowingUnderlined:

                    // TODO: graphycal effect (serena o lupo...)
                    messageBox.Text = "Relay sowing...";

                    break;

                case ActionType.startMtajiMoveUnderlined:

                    // TODO: graphycal effect (serena o lupo...)

                    break;

                case ActionType.startNamuaMoveUnderlined:

                    // TODO: graphycal effect (serena o lupo...)

                    break;

                case ActionType.sowOneSeed:

                    DrawShimo(_handeldAction.Row, _handeldAction.Col, _handeldAction.Seeds);

                    // TODO: capire che giocatore sta seminando sulla base della riga
                    if (Convert.ToInt32(player1KeteInHandTextBlock.Text) >= 1)  // non so se questo controllo è necessario sec me era dovuto per un tappullo, provo a levarlo...
                        player1KeteInHandTextBlock.Text = (Convert.ToInt32(player1KeteInHandTextBlock.Text) - 1).ToString();

                        //messageBox.Text = "Sowing...";

                    break;

                case ActionType.pickOpponentSeeds:

                    DrawShimo(_handeldAction.Row, _handeldAction.Col, 0);
                    player1KeteInHandTextBlock.Text = _handeldAction.Seeds.ToString();

                    break;

                case ActionType.pickOwnSeeds:

                    DrawShimo(_handeldAction.Row, _handeldAction.Col, 0);
                    player1KeteInHandTextBlock.Text = _handeldAction.Seeds.ToString();

                    break;

                case ActionType.pickTwoSeeds:

                    DrawShimo(_handeldAction.Row, _handeldAction.Col, _handeldAction.Seeds);
                    player1KeteInHandTextBlock.Text = "2";

                    break;

                case ActionType.playNyumba:

                    // TODO

                    break;

                case ActionType.stayNyumba:

                    // TODO

                    break;

                default:
                    break;
            }

            GraphicEngine(false);
        }

        private void playNyumbaYesButton_Click(object sender, RoutedEventArgs e)
        {
            _state = MatchState.playerPlayNyumba;

            playNyumbaYesButton.Visibility = Visibility.Collapsed;
            playNyumbaNoButton.Visibility = Visibility.Collapsed;

            Game();
        }

        private void playNyumbaNoButton_Click(object sender, RoutedEventArgs e)
        {
            _state = MatchState.testWinCondition;
            SwitchTurn();

            playNyumbaYesButton.Visibility = Visibility.Collapsed;
            playNyumbaNoButton.Visibility = Visibility.Collapsed;

            Game();
        }
    }
}
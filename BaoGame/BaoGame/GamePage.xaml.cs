using System;
using System.Threading;
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
using MiniMaxTree;
using System.Diagnostics;
using System.Windows.Threading;

namespace BaoGame
{
    public enum MatchState
    {
        initialization,
        player1EvaluatingPossibleMoves,
        player1MustPickShimo,
        player1PickedShimo,
        player1MustPickSecondShimo,
        player1PickedSecondShimo,
        player1AskedToPlayNyumba,
        player1PlayNyumba,
        player1Move,
        testWinCondition,
        cpuMove,
        end
    }

    public enum FirstMove
    {
        random,
        player1,
        CPU
    }

    public struct ShimoUtility
    {
        public Shimo underlinedShimo;

        public bool allowed;

        public Shimo sowingShimo;

        public bool sowingDirection;

        public string message;

        public bool chosen;
    }

    public partial class GamePage : PhoneApplicationPage
    {
        # region private members

        GameType _gameType;
        FirstMove _firstMove;
        BaoGameState _actualGameState;
        MatchState _state;
        int _playerTurn;
        List<BaoMove> _player1PossibleMoves;
        MustMove _player1MustMove;
        Shimo _player1Shimo;

        ImageSource[] _imageSourceArray;

        ShimoUtility _shimoUtilityLeft;
        ShimoUtility _shimoUtilityRight;

        Queue<ScreenAction> _sa;
        ScreenAction _handeldAction;
        DispatcherTimer _actionTimer;

        bool _bGraphicEngineRunning;

        int _numKeteInHand;

        #endregion


        public GamePage()
        {
            InitializeComponent();

            _firstMove = FirstMove.random;

            _imageSourceArray = new ImageSource[16];

            for (int i = 0; i < 16; i++)
            {
                _imageSourceArray[i] = new ImageSourceConverter().ConvertFromString("img/semini_" + i.ToString() + ".png") as ImageSource; 
            }

            _sa = new Queue<ScreenAction>();

            _actionTimer = new DispatcherTimer();
            _actionTimer.Tick += new EventHandler(ActionHandler);
            _actionTimer.Stop();

            _bGraphicEngineRunning = false;
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
            if (_bGraphicEngineRunning)
            {
                return;
            }

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
                    _state = MatchState.player1MustPickShimo;
                    gameCall = true;
                    break;

                case MatchState.player1MustPickShimo:

                    _playerTurn = 1;

                    // player must pick the first shimo...
                    messageBox.Text = "Pick a shimo!";

                    break;
             
                case MatchState.player1PickedShimo:

                    _numKeteInHand = 0;

                    // Execute player pick shimo, update state and execute animations
                    _sa = _actualGameState.ExecutePlayerPickShimo(_player1Shimo.row, _player1Shimo.col, _player1MustMove, out _numKeteInHand);
                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));
                    GraphicEngine(true);

                    // calculate utility structure (sowing shimo - sowing direction - underlined shimo - allowed - message                    
                    bool bSecondShimoChose = true;

                    Shimo underlinedShimo;

                    _shimoUtilityLeft.allowed = false;
                    _shimoUtilityRight.allowed = false;
                    _shimoUtilityLeft.chosen = false;
                    _shimoUtilityRight.chosen = false;

                    if( _player1MustMove == MustMove.namuaCapture )
                    {
                        // namua capture case

                        // if first shimo is kitchwa or kimbi sowing shimo is automaticcally chosen
                        if (_player1Shimo.col <= 2)
                        {
                            _shimoUtilityRight.allowed = true;
                            _shimoUtilityRight.sowingShimo = new Shimo(2, 0);
                            _shimoUtilityRight.sowingDirection = false;   // tautologico?
                            _shimoUtilityRight.chosen = true;
                            bSecondShimoChose = false;
                        }
                        else if (_player1Shimo.col >= 7)
                        {
                            _shimoUtilityLeft.allowed = true;
                            _shimoUtilityLeft.sowingShimo = new Shimo(2,9);
                            _shimoUtilityLeft.sowingDirection = true;   // tautologico?
                            _shimoUtilityLeft.chosen = true;
                            bSecondShimoChose = false;
                        }
                        else
                        {
                            // seconds shimo allowed are left are right kitchwa, no possibility of error
                            _shimoUtilityRight.allowed = true;
                            _shimoUtilityRight.sowingShimo = new Shimo(2, 0);
                            underlinedShimo = new Shimo(2,1);
                            _shimoUtilityRight.underlinedShimo = underlinedShimo;
                            UnderlineShimo(underlinedShimo, 1);
                            _shimoUtilityRight.sowingDirection = false;   // tautologico?

                            _shimoUtilityLeft.allowed = true;
                            _shimoUtilityLeft.sowingShimo = new Shimo(2, 9);
                            underlinedShimo = new Shimo(2,8);
                            _shimoUtilityLeft.underlinedShimo = underlinedShimo;
                            UnderlineShimo(underlinedShimo, 1);
                            _shimoUtilityLeft.sowingDirection = true;   // tautologico?

                            bSecondShimoChose = true;
                        }
                    }
                    else
                    {
                        // if not namua capture sowing shimo is current shimo, sowing direction can be either left, rigt or both
                        // it depends on player1 possible moves
                        BaoMove tentative;

                        // underlined shimos are the one next to sowing shimo

                        _shimoUtilityLeft.sowingShimo = _player1Shimo;
                        _shimoUtilityLeft.underlinedShimo = _actualGameState.NextShimo(_shimoUtilityLeft.sowingShimo, true);
                        UnderlineShimo(_shimoUtilityLeft.underlinedShimo, 2);
                        _shimoUtilityLeft.sowingDirection = true;
                        tentative = new BaoMove(_player1Shimo.row, _player1Shimo.col, true);
                        foreach (BaoMove m in _player1PossibleMoves)
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
                            switch (_player1MustMove)
                            {
                                case MustMove.namuaLonelyShimoTakasaAwayException:
                                case MustMove.mtagiLonelyShimoTakasaAwayException:
                                    _shimoUtilityLeft.message = "You cannot leave your inner row empty, chose the other direction!";
                                    break;

                                case MustMove.mtagiCapture:
                                    _shimoUtilityLeft.message = "You must chose the other direction in order to capture!";
                                    break;

                                default:
                                    Debug.Assert(false);    // must never get here!!!
                                    break;
                            }   
                        }

                        _shimoUtilityRight.sowingShimo = _player1Shimo;
                        _shimoUtilityRight.underlinedShimo = _actualGameState.NextShimo(_shimoUtilityRight.sowingShimo, false);
                        UnderlineShimo(_shimoUtilityRight.underlinedShimo, 2);
                        _shimoUtilityRight.sowingDirection = false;
                        tentative = new BaoMove(_player1Shimo.row, _player1Shimo.col, false);
                        foreach (BaoMove m in _player1PossibleMoves)
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
                            switch (_player1MustMove)
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
                        _state = MatchState.player1MustPickSecondShimo;
                    }
                    else
                    {
                        _state = MatchState.player1PickedSecondShimo;
                    }

                    break;

                case MatchState.player1MustPickSecondShimo:

                    messageBox.Text = "Pick a direction!";

                    break;

                case MatchState.player1PickedSecondShimo:

                    bool askPlayNyumba = false;

                    if (_shimoUtilityLeft.chosen == true)
                    {
                        _sa = _actualGameState.ExecutePlayerMove(_shimoUtilityLeft.sowingShimo.row, _shimoUtilityLeft.sowingShimo.col, _shimoUtilityLeft.sowingDirection, 1,
                            _numKeteInHand, _player1MustMove, out askPlayNyumba);
                    }
                    else if (_shimoUtilityRight.chosen == true)
                    {
                        _sa = _actualGameState.ExecutePlayerMove(_shimoUtilityRight.sowingShimo.row, _shimoUtilityRight.sowingShimo.col, _shimoUtilityRight.sowingDirection, 1,
                            _numKeteInHand, _player1MustMove, out askPlayNyumba);
                    }
                    else
                    {
                        Debug.Assert(false);    // must never get here!!!
                    }
                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));

                    GraphicEngine(true);

                    if (askPlayNyumba)
                    {
                        _state = MatchState.player1AskedToPlayNyumba;
                    }
                    else
                    {
                        _playerTurn = 2;
                        _state = MatchState.testWinCondition;
                    }

                    break;

                case MatchState.player1AskedToPlayNyumba:

                    messageBox.Text = "Play Nyumba?";
                    playNyumbaYesButton.Visibility = Visibility.Visible;
                    playNyumbaNoButton.Visibility = Visibility.Visible;

                    break;

                case MatchState.player1PlayNyumba:

                    if (_shimoUtilityLeft.chosen == true)
                        _sa = _actualGameState.PlayNyumba(true);
                    else
                        _sa = _actualGameState.PlayNyumba(false);

                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));

                    _playerTurn = 2;
                    _state = MatchState.testWinCondition;

                    GraphicEngine(true);

                    break;
                    
                       
                case MatchState.testWinCondition:

                    WinCondition wc = _actualGameState.WinLoseCondition((Byte)_playerTurn);
                    if (wc == WinCondition.Player1)
                    {
                        _state = MatchState.end;
                    }
                    else if (wc == WinCondition.Player2)
                    {
                        _state = MatchState.end;
                    }
                    else
                    {
                        if (_playerTurn == 1)
                        {
                            _state = MatchState.player1EvaluatingPossibleMoves;
                        }
                        else
                        {
                            _state = MatchState.cpuMove;
                        }
                    }

                    gameCall = true;
          
                    break;
        
                case MatchState.cpuMove:

                    messageBox.Text = "Computer is thinking...";

                    GTree<BaoMinimaxElement> minimaxTree = new GTree<BaoMinimaxElement>(new BaoMinimaxElement(_actualGameState));
                    GC.Collect();   // force the garbace collector to clean the memory
                    BaoMove cpuMove = MiniMax(minimaxTree, 5);

                    messageBox.Text = "Computer moves";

                    // update board
                    _sa = _actualGameState.ExecuteCPUMove(cpuMove, 2);
                    _sa.Enqueue(new ScreenAction(ActionType.none, 0, 0, 0));

                    _playerTurn = 1;

                    _state = MatchState.testWinCondition;

                    GraphicEngine(true);

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

            if (_bGraphicEngineRunning)
            {
                return;
            }

            bool gameCall = false;
            switch(_state)
            {
                case MatchState.player1MustPickShimo:

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
                        _player1Shimo.Set(row, col);

                        UnderlineShimo(_player1Shimo, 1);

                        _state = MatchState.player1PickedShimo;

                        gameCall = true;
                    }
                    else
                    {
                        // explain the reasons why player cannot pick this shimo
                        // 1) He pick a shimo in opponent territory
                        // 2) He does not take an empty shimo
                        // 3) Othere reasons depend on mustMove variable
                        if (row < 2)
                        {
                            messageBox.Text = "You must select in your own territory";
                        }
                        else if (_actualGameState.Board[row, col] == 0)
                        {
                            messageBox.Text = "You cannot pick an empty shimo!";
                        }
                        else
                        {
                            switch(_player1MustMove)
                            {
                                case MustMove.namuaCapture:
                                case MustMove.mtagiCapture:
                                    messageBox.Text = "You must capture if you can!";
                                    break;

                                case MustMove.namuaTakasaNonSingletonNonNyumba:
                                case MustMove.namuaLonelyShimoTakasaAwayException:
                                    if (row == 3)
                                    {
                                        messageBox.Text = "You must select from inner row if you can!";
                                        break;
                                    }
                                    else if (row == 2 && col == 5)
                                    {
                                        messageBox.Text = "You cannot pick your nyumba now!";
                                    }
                                    else
                                    {
                                        messageBox.Text = "You must select a non singleton shimo if you can!";
                                    }
                                    break;

                                case MustMove.namuaTakasaSingleton:
                                     messageBox.Text = "You must select from inner row if you can!";
                                     break;

                                case MustMove.namuaTaxRule:
                                     messageBox.Text = "Namua tax rule is the only option!";
                                     break;

                                case MustMove.mtagiInnerRowTakasa:
                                     if (row == 3)
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

                case MatchState.player1MustPickSecondShimo:

                    Shimo secondShimo = new Shimo(row, col);

                    if( secondShimo == _shimoUtilityLeft.underlinedShimo )
                    {
                        _shimoUtilityLeft.chosen = true;
                        _state = MatchState.player1PickedSecondShimo;
                        gameCall = true;
                    }
                    else if (secondShimo == _shimoUtilityRight.underlinedShimo)
                    {
                        _shimoUtilityRight.chosen = true;
                        _state = MatchState.player1PickedSecondShimo;
                        gameCall = true;
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
                    _actionTimer.Interval = new TimeSpan(7500000);
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

                    break;

                case ActionType.relaySowingUnderlined:

                    // TODO: graphycal effect (serena o lupo...)

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
                    if (Convert.ToInt32(player1KeteInHandTextBlock.Text) >= 1)
                        player1KeteInHandTextBlock.Text = (Convert.ToInt32(player1KeteInHandTextBlock.Text) - 1).ToString();

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
            _state = MatchState.player1PlayNyumba;

            playNyumbaYesButton.Visibility = Visibility.Collapsed;
            playNyumbaNoButton.Visibility = Visibility.Collapsed;

            Game();
        }

        private void playNyumbaNoButton_Click(object sender, RoutedEventArgs e)
        {
            _state = MatchState.testWinCondition;
            _playerTurn = 2;

            playNyumbaYesButton.Visibility = Visibility.Collapsed;
            playNyumbaNoButton.Visibility = Visibility.Collapsed;

            Game();
        }

        public static BaoMove MiniMax(GTree<BaoMinimaxElement> minimaxTree, int pli)
        {
            float maxScore;

            // minimax forward tree creation
            int numOfStates = 1;    // for testing and debug and possible extension
            Byte currPlayer = 2;    // CPU move first, then player 1 countermove and so on, until pli depth

            bool newBranch;         // begins true if there is at least one branch from the current layer
            int depth = 0;          // depth of the tree (root is at depth 0)
            do
            {
                newBranch = false;

                int dummyDebug = 0;

                foreach (Node<BaoMinimaxElement> node in minimaxTree.Layers[depth])
                {
                    BaoGameState gs = node.Content.gameState;
                    dummyDebug++;

                    switch (gs.WinLoseCondition(currPlayer))
                    {
                        case WinCondition.Player1:
                            node.Content.Score = float.MinValue;
                            break;

                        case WinCondition.Player2:
                            node.Content.Score = float.MaxValue;
                            break;

                        case WinCondition.none:
                            List<BaoGameState> derivedStates = gs.GenerateDerivedStates(currPlayer);

                            foreach (BaoGameState ds in derivedStates)
                            {
                                BaoMinimaxElement e = new BaoMinimaxElement(ds);
                                node.AddChild(e);
                                numOfStates++;
                            }

                            newBranch = true;
                            break;
                    }
                }

                currPlayer = (currPlayer == 1) ? (Byte)2 : (Byte)1;

                depth++;
            }
            while (depth < pli && newBranch);

            // ok forward state is done, I evaluate all the nodes in the deepest layer
            foreach (Node<BaoMinimaxElement> n in minimaxTree.Layers[minimaxTree.Layers.Count - 1])
            {
                n.Content.Score = n.Content.gameState.EvaluateGameState();
            }

            // now I climb up the tree, resolving minimax
            for (int backDepth = depth - 1; backDepth >= 1; backDepth--)
            {
                // min or max?
                currPlayer = minimaxTree.Layers[backDepth][0].Content.gameState.Player;

                if (currPlayer == 1)    // maximize
                {
                    foreach (var node in minimaxTree.Layers[backDepth].Where(node => node.Children.Count > 0))
                    {
                        node.Content.Score = node.Children.Max(n => n.Content.Score);
                    }

                }
                else                    // minimize
                {
                    foreach (var node in minimaxTree.Layers[backDepth].Where(node => node.Children.Count > 0))
                    {
                        node.Content.Score = node.Children.Min(n => n.Content.Score);
                    }
                }
            }

            // Now I am in the root, I calculate the last argmax, that will be the best possible move!
            BaoMove CPUMove = null;
            maxScore = float.MinValue;
            foreach (Node<BaoMinimaxElement> node in minimaxTree.Root.Children)
            {
                if (node.Content.Score >= maxScore)
                {
                    CPUMove = node.Content.gameState.Move;
                    maxScore = node.Content.Score;
                }
            }

            return CPUMove;
        }

    }
}
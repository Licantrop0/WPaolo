using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Bao
{
    enum Versor
    {
        left,
        right
    }

    public enum GameType
    {
        laKujifunza,
        laKiswahili
    }

    public enum WinCondition
    {
        none,
        Player1,
        Player2
    }

    public enum ActionType
    {
        none,
        captureUnderlined,
        relaySowingUnderlined,
        startNamuaMoveUnderlined,
        startMtajiMoveUnderlined,
        barnPicked,
        pickOpponentSeeds,
        pickOwnSeeds,
        pickTwoSeeds,
        sowOneSeed,
        askPlayNyumba,
        stayNyumba,
        playNyumba
    }

    public enum MustMove
    {
        none,
        namuaCapture,
        namuaTakasaNonSingletonNonNyumba,
        namuaTakasaSingleton,
        namuaTaxRule,
        namuaLonelyShimoTakasaAwayException,
        mtagiCapture,
        mtagiInnerRowTakasa,
        mtagiOuterRowTakasa,
        mtagiLonelyShimoTakasaAwayException
    }

    public struct Shimo
    {
        public Byte row;
        public Byte col;

        public Shimo(byte r, byte c)
        {
            row = r;
            col = c;
        }

        public void Set(byte r, byte c)
        {
            row = r;
            col = c;
        }

        public static bool operator == (Shimo left, Shimo right)
        {
            if (left.row == right.row && left.col == right.col)
                return true;
            else
                return false;
        }

        public static bool operator !=(Shimo left, Shimo right)
        {
            if (left.row != right.row || left.col == right.col)
                return true;
            else
                return false;
        }
    }

    public struct ShimoBaoMove
    {
        public Shimo shimo;
        public BaoMove baoMove;

        public ShimoBaoMove(Shimo sh, BaoMove bm)
        {
            shimo = sh;
            baoMove = bm;
        }
    }

    public class BaoGameState
    {
    # region static members

        static int MAX_SUBMOVES = 50;

    #endregion

    # region private members

        int _moveToAvoidFlag;

        Byte[,] _board;

        Byte _player;   // player whose move got to the current Gamestate

        BaoMove _move;

        Byte _barn1;

        Byte _barn2;

        bool _nyumba1;

        bool _nyumba2;

        bool _namua;

    # endregion

    #region costructors

        public BaoGameState()
        {
        }

        public BaoGameState(GameType gameType, Byte player)
        {
            // note, player is the the one whose move got to the current Gamestate
            // so if player is Player1, the player to move next is Player2 and viceversa
            _player = player;

            _move = new BaoMove();      // will be set to auto move

            if (gameType == GameType.laKiswahili)
            {
                // Bao LaKiswahili
                // initialize board
                _board = new byte[4, 10] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 2, 2, 6, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 6, 2, 2, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
                _barn1 = 22;
                _barn2 = 22;

                // kiswahili
                _nyumba1 = true;
                _nyumba2 = true;
                _namua = true;
            }
            else
            {
                // Bao LaKujifunza
                // initialize board
                _board = new byte[4, 10] { { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 }, { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 }, { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 }, { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 } };
                _barn1 = 0;
                _barn2 = 0;

                // kujifunza
                _nyumba1 = false;
                _nyumba2 = false;
                _namua = false;
            }
            
        }

        public BaoGameState DeepCopy()
        {
            BaoGameState copiedObject = new BaoGameState();

            copiedObject._player = _player;
            copiedObject._move = _move.DeepCopy();
            copiedObject._board = new byte[4, 10];
            Array.Copy(_board, copiedObject._board, copiedObject._board.Length);
            copiedObject._barn1 = _barn1;
            copiedObject._barn2 = _barn2;
            copiedObject._nyumba1 = _nyumba1;
            copiedObject._nyumba2 = _nyumba2;
            copiedObject._namua = _namua;

            return copiedObject;
        }

    #endregion

    # region properties

        public Byte Player { get { return _player; } set { _player = value; } }

        public BaoMove Move { get { return _move; } }

        public int MoveToAvoidFlag { get { return _moveToAvoidFlag; } }

        public Byte[,] Board
        {
            get
            {
                return _board;
            }
        }

        public Byte Barn1 { get { return _barn1; } }

        public Byte Barn2 { get { return _barn2; } }

    #endregion

    #region private methods

        // alla fine l'ho dovuto rendere pubblico, anche se è un po' una merda...
        private void NextShimo(ref Byte row, ref Byte col, ref Versor versor)
        {
            switch (versor)
            {
                case Versor.right:
                    if (col != 8)
                    {
                        col++;
                    }
                    else if (row == 0 || row == 2)
                    {
                        row++;
                        versor = Versor.left;
                    }
                    else if (row == 1 || row == 3)
                    {
                        row--;
                        versor = Versor.left;
                    }
                    break;

                case Versor.left:
                    if (col != 1)
                    {
                        col--;
                    }
                    else if (row == 0 || row == 2)
                    {
                        row++;
                        versor = Versor.right;
                    }
                    else if (row == 1 || row == 3)
                    {
                        row--;
                        versor = Versor.right;
                    }
                    break;

                default:
                    break;
            }
        }

        private bool CanCapture(Byte row, Byte col)
        {
            return (_board[row, col] > 1 && (row == 1 || row == 2) && _board[OppositeRow(row), col] > 0);
        }

        private bool CanRelaySowing(Byte row, Byte col)
        {
            return (_board[row, col] > 1);
        }

        private void CheckNyumbaStates(Byte row, Byte col)
        {
            if (row == 1 && col == 4 && _nyumba2)
            {
                _nyumba2 = false;
            }
            else if (row == 2 && col == 5 && _nyumba1)
            {
                _nyumba1 = false;
            }
        }

        private void CheckGameStage()
        {
            if (_barn1 == 0 && _barn2 == 0)
            {
                _namua = false;
                _nyumba1 = false;
                _nyumba2 = false;
            }
        }

        private bool IsThisShimboActiveNyumba(Byte row, Byte col)
        {
            if (row == 1 && col == 4 && _nyumba2 && _board[row,col] >= 6)
                return true;
            else if (row == 2 && col == 5 && _nyumba1 && _board[row,col] >= 6)
                return true;
            else
                return false;
        }

        private Byte OppositeRow(Byte row)
        {
            Debug.Assert(row == 1 || row == 2);

            if (row == 1)
                return 2;
            else
                return 1;
        }

        private byte OppositePlayer(byte player)
        {
            if (player == 1)
                return 2;
            else
                return 1;
        }  

        private bool IsMarker(Byte row, Byte col)
        {
            if (row == 0 || row == 3)
            // external rows are not markers
            {
                return false;
            }

            if (_board[row, col] >= 1 && _board[OppositeRow(row), col] >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EmptyInnerRow(Byte player)
        {
            Debug.Assert(player == 1 || player == 2);

            int innerRow = (player == 1) ? 2 : 1;

            for (int i = 1; i <= 8; i++)
            {
                if (_board[innerRow, i] > 0)
                    return false;
            }

            return true;
        }

        private void Capture(ref Byte row, ref Byte col, ref Versor versor)
        {
            int numOfKete = Pick(OppositeRow(row), col);
            CheckNyumbaStates(OppositeRow(row), col);

            if (col > 2 && col < 7)
            // central columns
            {
                // keep the verse
                if (versor == Versor.left)
                {
                    col = 9;
                }
                else
                {
                    col = 0;
                }
            }
            else
            // kimbi columns
            {
                // verse is given by the closest kitcwa
                if (col >= 7)
                {
                    col = 9;
                    versor = Versor.left;
                }
                else
                {
                    col = 0;
                    versor = Versor.right;
                }
            }

            Sow(ref row, ref col, ref versor, numOfKete);
        }

        private void CaptureAction(ref Byte row, ref Byte col, ref Versor versor, ref Queue<ScreenAction> actionList)
        {
            int numOfKete = Pick(OppositeRow(row), col);
            actionList.Enqueue(new ScreenAction(ActionType.captureUnderlined, OppositeRow(row), col, 0));
            actionList.Enqueue(new ScreenAction(ActionType.pickOpponentSeeds, OppositeRow(row), col, (Byte)numOfKete));

            CheckNyumbaStates(OppositeRow(row), col);

            if (col > 2 && col < 7)
            // central columns
            {
                // keep the verse
                if (versor == Versor.left)
                {
                    col = 9;
                }
                else
                {
                    col = 0;
                }
            }
            else
            // kimbi columns
            {
                // verse is given by the closest kitcwa
                if (col >= 7)
                {
                    col = 9;
                    versor = Versor.left;
                }
                else
                {
                    col = 0;
                    versor = Versor.right;
                }
            }

            SowAction(ref row, ref col, ref versor, numOfKete, ref actionList);
        }

        private void Sow(ref Byte row, ref Byte col, ref Versor versor, int numOfKete)
        {
            CheckNyumbaStates(row, col);

            while (numOfKete > 0)
            {
                NextShimo(ref row, ref col, ref versor);
                _board[row, col]++;
                
                numOfKete--;
            }
        }

        private void SowAction(ref Byte row, ref Byte col, ref Versor versor, int numOfKete, ref Queue<ScreenAction> actionList)
        {
            CheckNyumbaStates(row, col);

            while (numOfKete > 0)
            {
                NextShimo(ref row, ref col, ref versor);
                _board[row, col]++;

                actionList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, col, _board[row, col]));

                numOfKete--;
            }
        }

        private int Pick(Byte row, Byte col)
        {
            int numOfKete = _board[row, col];
            _board[row, col] = 0;

            return numOfKete;
        }

        private void PutSeedFromBarn(Byte row, Byte col, Byte player, bool nyumbaTax, bool left)
        {
            Debug.Assert((player == 1 && (row == 2 || row == 3)) || (player == 2 && (row == 0 || row == 1)));

            if (player == 1)
                _barn1--;
            else
                _barn2--;

            _board[row, col]++;

            // Nyumba tax rule
            if (nyumbaTax)
            {
                _board[row, col]--;
                _board[row, col]--;

                if (left)
                {
                    _board[row, col - 1]++;
                    _board[row, col - 2]++;
                }
                else
                {
                    _board[row, col + 1]++;
                    _board[row, col + 2]++;
                }
            }
        }

        private void PutSeedFromBarnAction(Byte row, Byte col, Byte player, bool nyumbaTax, bool left, ref Queue<ScreenAction> actionList)
        {
            Debug.Assert((player == 1 && (row == 2 || row == 3)) || (player == 2 && (row == 0 || row == 1)));
            byte barn;

            if (player == 1)
            {
                _barn1--;
                barn = _barn1;
            }
            else
            {
                _barn2--;
                barn = _barn2;
            }

            actionList.Enqueue(new ScreenAction(ActionType.barnPicked, player, player, barn));
                
            _board[row, col]++;

            //actionList.Enqueue(new ScreenAction(ActionType.startNamuaMoveUnderlined, row, col, 0));
            actionList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, col, _board[row, col]));

            // Nyumba tax rule
            if (nyumbaTax)
            {
                _board[row, col]--;
                _board[row, col]--;

                actionList.Enqueue(new ScreenAction(ActionType.pickTwoSeeds, row, col, _board[row, col]));

                if (left)
                {
                    _board[row, col - 1]++;
                    _board[row, col - 2]++;

                    actionList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col - (Byte)1), _board[row, col - (Byte)1]));
                    actionList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col - (Byte)2), _board[row, col - (Byte)2]));

                }
                else
                {
                    _board[row, col + 1]++;
                    _board[row, col + 2]++;

                    actionList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col + (Byte)1), _board[row, col + (Byte)1]));
                    actionList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col + (Byte)2), _board[row, col + (Byte)2]));
                }
            }
        }

        private BaoMove LonelyShimoTakasaOnlyAllowedMove(Byte player)
        {
            Byte row = (player == 2) ? (Byte)1 : (Byte)2;

            int filledShimoCount = 0;

            for (Byte col = 1; col <= 8; col++)
            {
                if (_board[row, col] > 0)
                {
                    filledShimoCount++;
                    if (filledShimoCount > 1)
                    {
                        return null;
                    }
                }
            }

            if (filledShimoCount == 1)
            {
                if (_board[row, 1] > 0)
                {
                    return new BaoMove(row, 1, false);
                }
                else if (_board[row, 8] > 0)
                {
                    return new BaoMove(row, 8, true);
                }
            }

            return null;
        }

        private List<BaoMove> GetPossibleMovesNamua(Byte player, out MustMove explanation)
        {
            Debug.Assert(player == 1 || player == 2);

            explanation = MustMove.none;
            List<BaoMove> returnMoves = new List<BaoMove>();

            Byte row = (player == 2) ? (Byte)1 : (Byte)2;

            BaoMove currMove = null;
            // first all all check if it is possible to capture move
            for (Byte col = 1; col <= 8; col++)
            {
                if (IsMarker(row, col))
                {
                    currMove = new BaoMove(row, col, true);
                    returnMoves.Add(currMove);

                    if (col > 2 && col < 7)
                    {
                        currMove = new BaoMove(row, col, false);
                        returnMoves.Add(currMove);
                    }
                }
            }

            if (returnMoves.Count > 0)
            {
                explanation = MustMove.namuaCapture;
                return returnMoves;
            }

            // must check for lonely shimo takasa away exception, in this case, I have one move possbile, toward the inner row
            BaoMove onlyAllowedTakasaMove = LonelyShimoTakasaOnlyAllowedMove(player);
            if(onlyAllowedTakasaMove != null)
            {
                returnMoves.Add(onlyAllowedTakasaMove);
                explanation = MustMove.namuaLonelyShimoTakasaAwayException;
                return returnMoves;
            }

            // now I check for non singleton shimo, non nyumba takasa move
            // I must check takasa moves starting from holes
            // containing two holes or more (not a six or more seeded nyumba)
            for (Byte col = 1; col <= 8; col++)
            {
                if (_board[row, col] > 1 && !IsThisShimboActiveNyumba(row, col))
                {
                    currMove = new BaoMove(row, col, true);
                    returnMoves.Add(currMove);
                    currMove = new BaoMove(row, col, false);
                    returnMoves.Add(currMove);
                }
            }

            if (returnMoves.Count > 0)
            {
                explanation = MustMove.namuaTakasaNonSingletonNonNyumba;
                return returnMoves;
            }

            // now I check for singleton takasa moves
            // at this point I can move singletons if there are any
            for (Byte col = 1; col <= 8; col++)
            {
                if (_board[row, col] == 1)
                {
                    currMove = new BaoMove(row, col, true);
                    returnMoves.Add(currMove);
                    currMove = new BaoMove(row, col, false);
                    returnMoves.Add(currMove);
                }
            }

            if (returnMoves.Count > 0)
            {
                explanation = MustMove.namuaTakasaSingleton;
                return returnMoves;
            }

            // now the only option is the nyumba tax rule
            if (player == 1 && IsThisShimboActiveNyumba(2, 5))
            {
                currMove = new BaoMove(2, 5, true, false, true);
                returnMoves.Add(currMove);
                currMove = new BaoMove(2, 5, false, false, true);
                returnMoves.Add(currMove);
            }
            else if (player == 2 && IsThisShimboActiveNyumba(1, 4))
            {
                currMove = new BaoMove(1, 4, true, false, true);
                returnMoves.Add(currMove);
                currMove = new BaoMove(1, 4, false, false, true);
                returnMoves.Add(currMove);
            }

            if (returnMoves.Count > 0)
            {
                explanation = MustMove.namuaTaxRule;
                return returnMoves;
            }

            return returnMoves;
        }

        private List<BaoMove> GetPossibleMovesMtaji(Byte player, out MustMove explanation)
        {
            Debug.Assert(player == 1 || player == 2);

            explanation = MustMove.none;
            List<BaoMove> returnMoves = new List<BaoMove>();

            Byte startRow = (player == 2) ? (Byte)0 : (Byte)2;

            BaoMove currMove = null;

            // first of all I chexk for possible capture moves
            for (Byte row = startRow; row < startRow + 2; row++)
            {
                for (Byte col = 1; col <= 8; col++)
                {
                    int numOfKete = _board[row, col];

                    // go to next shimo if there are not any kete or there are more than 15 ketes
                    if (numOfKete <= 1 || numOfKete > 15)
                    {
                        continue;
                    }

                    bool left = false;
                    int i = 0;

                    while (i < 2)
                    {
                        currMove = new BaoMove(row, col, left);

                        Versor versor = left ? Versor.left : Versor.right;
                        Byte destRow = row;
                        Byte destCol = col;

                        // from the original shimo, move in given versor direction until I have kete
                        for (int k = 0; k < numOfKete; k++)
                        {
                            NextShimo(ref destRow, ref destCol, ref versor);
                        }

                        // see if destination shimo is marker
                        if (IsMarker(destRow, destCol))
                        {
                            returnMoves.Add(currMove);
                        }

                        i++;
                        left = !left;
                    }
                }
            }

            if (returnMoves.Count > 0)
            {
                explanation = MustMove.mtagiCapture;
                return returnMoves;
            }

            // must check for lonely shimo takasa away exception, in this case, I have one move possbile, toward the inner row
            BaoMove onlyAllowedTakasaMove = LonelyShimoTakasaOnlyAllowedMove(player);
            if(onlyAllowedTakasaMove != null)
            {
                returnMoves.Add(onlyAllowedTakasaMove);
                explanation = MustMove.mtagiLonelyShimoTakasaAwayException;
                return returnMoves;
            }

            // now I can check for inner row takasa
            Byte innerRow = (player == 2) ? (Byte)1 : (Byte)2;

            for (Byte col = 1; col <= 8; col++)
            {
                if(_board[innerRow, col] > 1)
                {
                    currMove = new BaoMove(innerRow, col, true);
                    returnMoves.Add(currMove);
                    currMove = new BaoMove(innerRow, col, false);
                    returnMoves.Add(currMove);
                }
            }

            if (returnMoves.Count > 0)
            {
                explanation = MustMove.mtagiInnerRowTakasa;
                return returnMoves;
            }

            // now I will check for outer row takasa
            Byte outerRow = (player == 2) ? (Byte)0 : (Byte)3;

            for (Byte col = 1; col <= 8; col++)
            {
                if(_board[outerRow, col] > 1)
                {
                    currMove = new BaoMove(outerRow, col, true);
                    returnMoves.Add(currMove);
                    currMove = new BaoMove(outerRow, col, false);
                    returnMoves.Add(currMove);
                }
            }

            if (returnMoves.Count > 0)
            {
                explanation = MustMove.mtagiOuterRowTakasa;
                return returnMoves;
            }

            return returnMoves;
        }

        public void UpdateGameState(BaoMove move, Byte player, out bool generatePlayNyumba)
        {
            generatePlayNyumba = false;

            // does not check for a valid move, check must be done somewhere else, so move MUST be a valid one
            int submoves = 0;

            // set final state of player and move
            _player = player;
            _move = move;

            Byte row = move.rowBox;
            Byte col = move.columnBox;

            // init the initial versor
            Versor versor = move.left ? Versor.left : Versor.right;
            int numOfKete = 0;

            if (_namua)
            {
                // put the seed from barn to the target shimo
                PutSeedFromBarn(row, col, player, move.nyumbaTax, move.left);          // qui va implementata la regola della tassa della casa!
            }
            else
            {
                // pick all the kete from the target shimo
                numOfKete = Pick(row, col);
                Sow(ref row, ref col, ref versor, numOfKete);
            }

            bool stayNyumba = false;

            // see if I can capture
            if (CanCapture(row, col))
            {
                // turn with captures
                while ((CanCapture(row, col) || CanRelaySowing(row, col)) && !stayNyumba && submoves < MAX_SUBMOVES)
                {
                    if (EmptyInnerRow(OppositePlayer(player)))
                    {
                        break;  // player wins, no need to go on with further sowing...
                    }
                        
                    if (CanCapture(row, col))
                    {
                        Capture(ref row, ref col, ref versor);
                        submoves++;
                    }
                    else if (CanRelaySowing(row, col))
                    {
                        if (IsThisShimboActiveNyumba(row, col) && !move.playNyumba)
                        {
                            generatePlayNyumba = true;
                            stayNyumba = true;
                        }
                        else
                        {
                            numOfKete = Pick(row, col);
                            Sow(ref row, ref col, ref versor, numOfKete);
                            submoves++;
                        }
                    }
                }
            }
            else
            {
                // turn without captures
                while (CanRelaySowing(row, col) && !stayNyumba && submoves < MAX_SUBMOVES)
                {
                    if (IsThisShimboActiveNyumba(row, col) && !move.playNyumba)
                    {
                        stayNyumba = true;
                    }
                    else
                    {
                        numOfKete = Pick(row, col);
                        Sow(ref row, ref col, ref versor, numOfKete);
                        submoves++;
                    }
                }
            }

            // if the move lead to have the nyumba under menace and player nyumba NOT under menace I rise the moveToAvoidFlag
            if (player == 2 && _nyumba2 && _board[2, 4] >= 1 && !(_nyumba1 && _board[1,5] >= 1) )
            {
                _moveToAvoidFlag = 1;
            }
                
            CheckGameStage();
        }

        

    #endregion

    # region public methods

        public void Print()    // only for debug (sarebbe opportuno che tornasse una stringa con lo stato del tabellone, delegando al chiamante la stampa
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    Console.Write(_board[i, j].ToString() + "\t");
                }
                Console.Write("\n");
            }

            Console.WriteLine("");
        }

        public Shimo NextShimo(Shimo shimo, bool direction)
        {
            Byte row = shimo.row;
            Byte col = shimo.col;
            Versor versor = direction == true ? Versor.left : Versor.right;

            NextShimo(ref row, ref col, ref versor);

            return new Shimo(row, col);
        }

        // 2 players ok
        public Queue<ScreenAction> ExecutePlayerPickShimo(Byte row, Byte col, byte player, MustMove mustMove, out int numKeteInHand)
        {
            Queue<ScreenAction> returnList = new Queue<ScreenAction>();
            numKeteInHand = 0;

            if (_namua)
            {
                returnList.Enqueue(new ScreenAction(ActionType.startNamuaMoveUnderlined, row, col, 0));     // qui va implementata la sottolineatura di inizio mossa

                // add one seed to target shimo from Barney
                _board[row, col]++;

                if (player == 1)
                {
                    _barn1--;
                    returnList.Enqueue(new ScreenAction(ActionType.barnPicked, player, player, _barn1));
                }
                else
                {
                    _barn2--;
                    returnList.Enqueue(new ScreenAction(ActionType.barnPicked, player, player, _barn2));
                }

                returnList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, col, _board[row, col]));
            }
            else
            {
                returnList.Enqueue(new ScreenAction(ActionType.startMtajiMoveUnderlined, row, col, 0)); // qui va implementata la sottolineatura di inizio mossa
            }

            // if it is a namua capture move pick opponent seeds
            if (mustMove == MustMove.namuaCapture)
            {
                numKeteInHand = Pick(OppositeRow(row), col);
                CheckNyumbaStates(OppositeRow(row), col);

                returnList.Enqueue(new ScreenAction(ActionType.captureUnderlined, OppositeRow(row), col, 0));
                returnList.Enqueue(new ScreenAction(ActionType.pickOpponentSeeds, OppositeRow(row), col, (Byte)numKeteInHand));
            }
            // if it is namua tax rule just pick two seeds
            else if (mustMove == MustMove.namuaTaxRule)
            {
                numKeteInHand = 2;

                _board[row, col] = (Byte)(_board[row, col] - 2);
                returnList.Enqueue(new ScreenAction(ActionType.pickTwoSeeds, row, col, _board[row, col]));
            }
            // in all the other cases I pick all the seeds in the shimo
            else
            {
                numKeteInHand = Pick(row, col);

                returnList.Enqueue(new ScreenAction(ActionType.pickOwnSeeds, row, col, (Byte)numKeteInHand));
            }

            return returnList;
        }

        // 2 players ok
        public Queue<ScreenAction> ExecutePlayerMove(Byte row, Byte col, bool left, Byte player, int numOfKeteinHand, MustMove mustMove, out bool askPlayNyumba)
        {
            Queue<ScreenAction> returnList = new Queue<ScreenAction>();

            askPlayNyumba = false;
            bool stayNyumba = false;

            int submoves = 0;

            // set final state of player and move
            _player = player;
            _move = new BaoMove(0,0,false); // dummy move, a real move is important to put only for MiniMax algorithm...

            // init the initial versor
            Versor versor = left ? Versor.left : Versor.right;

            // first of all I handle the special case of nyumba tax rule (se questo codice è presente da un altra parte va raggrupato in un metodo privato...)
            if (mustMove == MustMove.namuaTaxRule)
            {
                if (left)
                {
                    _board[row, col - 1]++;
                    _board[row, col - 2]++;
                    returnList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col - (Byte)1), _board[row, col - 1]));
                    returnList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col - (Byte)2), _board[row, col - 2]));

                }
                else
                {
                    _board[row, col + 1]++;
                    _board[row, col + 2]++;
                    returnList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col + (Byte)1), _board[row, col + 1]));
                    returnList.Enqueue(new ScreenAction(ActionType.sowOneSeed, row, (Byte)(col + (Byte)2), _board[row, col + 2]));
                }
                return returnList;
            }

            // I sow every kete I have in hand in given direction
            SowAction(ref row, ref col,ref versor, numOfKeteinHand, ref returnList);

            // now depending on the kind of move I handle the capture and relay sowing cycles
            if (mustMove == MustMove.namuaCapture || mustMove == MustMove.mtagiCapture)
            {
                while ((CanCapture(row, col) || CanRelaySowing(row, col)) && !askPlayNyumba && submoves < MAX_SUBMOVES)
                {
                    if (EmptyInnerRow(OppositePlayer(player)))
                    {
                        break;  // player wins, no need to go on with further sowing...
                    }

                    if (CanCapture(row, col))
                    {
                        CaptureAction(ref row, ref col, ref versor, ref returnList);
                        submoves++;
                    }
                    else if (!IsThisShimboActiveNyumba(row, col))
                    {
                        numOfKeteinHand = Pick(row, col);
                        returnList.Enqueue(new ScreenAction(ActionType.relaySowingUnderlined, row, col, 0));
                        returnList.Enqueue(new ScreenAction(ActionType.pickOwnSeeds, row, col, (Byte)numOfKeteinHand));

                        SowAction(ref row, ref col, ref versor, numOfKeteinHand, ref returnList);
                        submoves++;
                    }
                    else
                    {
                        askPlayNyumba = true;
                        returnList.Enqueue(new ScreenAction(ActionType.askPlayNyumba, player, player, 0));
                    }
                }
            }
            else
            {
                // takasa move
                while (CanRelaySowing(row, col) && !stayNyumba && submoves < MAX_SUBMOVES)
                {
                    if (IsThisShimboActiveNyumba(row, col))
                    {
                        returnList.Enqueue(new ScreenAction(ActionType.stayNyumba, row, col, 0));
                        stayNyumba = true;
                    }
                    else
                    {
                        numOfKeteinHand = Pick(row, col);

                        returnList.Enqueue(new ScreenAction(ActionType.relaySowingUnderlined, row, col, 0));
                        returnList.Enqueue(new ScreenAction(ActionType.pickOwnSeeds, row, col, (Byte)numOfKeteinHand));

                        SowAction(ref row, ref col, ref versor, numOfKeteinHand, ref returnList);
                        submoves++;
                    }
                }
            }

            if (!askPlayNyumba)
            {
                CheckGameStage();
            }
                
            return returnList;
        }

        public Queue<ScreenAction> ExecuteCPUMove(BaoMove move, Byte player)
        {
            Queue<ScreenAction> returnList = new Queue<ScreenAction>();

            // does not check for a valid move, check must be done somewhere else, so move MUST be a valid one
            int submoves = 0;

            // set final state of player and move
            _player = player;
            _move = move;   

            Byte row = move.rowBox;
            Byte col = move.columnBox;

            // init the initial versor
            Versor versor = move.left ? Versor.left : Versor.right;
            int numOfKete = 0;

            if (_namua)
            {
                // put the seed from barn to the target shimo
                PutSeedFromBarnAction(row, col, player, move.nyumbaTax, move.left, ref returnList);
            }
            else
            {
                // pick all the kete from the target shimo
                numOfKete = Pick(row, col);
                returnList.Enqueue(new ScreenAction(ActionType.pickOwnSeeds, row, col, (Byte)numOfKete));
                SowAction(ref row, ref col, ref versor, numOfKete, ref returnList);
            }

            bool stayNyumba = false;
            // see if I can capture
            if (CanCapture(row, col))
            {
                // turn with captures
                while ((CanCapture(row, col) || CanRelaySowing(row, col)) && !stayNyumba && submoves < MAX_SUBMOVES)
                {
                    if (EmptyInnerRow(OppositePlayer(player)))
                    {
                        break;  // cpu wins, no need to go on with further sowing...
                    }

                    if (CanCapture(row, col))
                    {
                        CaptureAction(ref row, ref col, ref versor, ref returnList);
                        submoves++;
                    }
                    else if (CanRelaySowing(row, col))
                    {
                        if (IsThisShimboActiveNyumba(row, col) && !move.playNyumba)
                        {
                            stayNyumba = true;
                        }
                        else
                        {
                            numOfKete = Pick(row, col);
                            returnList.Enqueue(new ScreenAction(ActionType.relaySowingUnderlined, row, col, 0));
                            returnList.Enqueue(new ScreenAction(ActionType.pickOwnSeeds, row, col, (Byte)numOfKete));

                            SowAction(ref row, ref col, ref versor, numOfKete, ref returnList);
                            submoves++;
                        }
                    }
                }
            }
            else
            {
                // turn without captures
                while (CanRelaySowing(row, col) && !stayNyumba && submoves < MAX_SUBMOVES)
                {
                    if (IsThisShimboActiveNyumba(row, col) && !move.playNyumba)
                    {
                        stayNyumba = true;
                    }
                    else
                    {
                        numOfKete = Pick(row, col);
                        returnList.Enqueue(new ScreenAction(ActionType.relaySowingUnderlined, row, col, 0));
                        returnList.Enqueue(new ScreenAction(ActionType.pickOwnSeeds, row, col, (Byte)numOfKete));

                        SowAction(ref row, ref col, ref versor, numOfKete, ref returnList);
                        submoves++;
                    }
                }
            }

            CheckGameStage();

            return returnList;
        }

        // 2 players ok
        public Queue<ScreenAction> PlayNyumba(bool left, byte player)
        {
            // init the initial versor
            Versor versor = left ? Versor.left : Versor.right;

            Byte row = player == 1 ? (byte)2 : (byte)1;
            Byte col = player == 1 ? (byte)5 : (byte)4; 

            Queue<ScreenAction> returnList = new Queue<ScreenAction>();
            int submoves = 1;
            int numOfKete;

            while ((CanCapture(row, col) || CanRelaySowing(row, col)) && submoves < MAX_SUBMOVES)
            {
                if (EmptyInnerRow(OppositePlayer(player)))
                {
                    break;  // player wins, no need to go on with further sowing...
                }

                if (CanCapture(row, col))
                {
                    CaptureAction(ref row, ref col, ref versor, ref returnList);
                    submoves++;
                }
                else if (CanRelaySowing(row, col))
                {
                    numOfKete = Pick(row, col);
                    returnList.Enqueue(new ScreenAction(ActionType.relaySowingUnderlined, row, col, 0));
                    returnList.Enqueue(new ScreenAction(ActionType.pickOwnSeeds, row, col, (Byte)numOfKete));

                    SowAction(ref row, ref col, ref versor, numOfKete, ref returnList);
                    submoves++;   
                } 
            }

            CheckGameStage();

            return returnList;  
        }

        public List<BaoGameState> GenerateDerivedStates(Byte player)
        {
            List<BaoGameState> returnList = new List<BaoGameState>();
            MustMove explanation = MustMove.none;

            List<BaoMove> possibleMoves = GetPossibleMoves(player, out explanation);

            foreach (BaoMove m1 in possibleMoves)
            {
                List<BaoGameState> tmp = NewGameStatesFromExisting(m1, player);

                foreach (BaoGameState m2 in tmp)
                {
                    returnList.Add(m2);
                }
            }

            return returnList;
        }

        public List<BaoGameState> NewGameStatesFromExisting(BaoMove move, Byte player)
        {
            List<BaoGameState> newGameStates = new List<BaoGameState>();
            bool playNyumba = false;

            BaoGameState newGameState = this.DeepCopy();
            newGameState.UpdateGameState(move, player, out playNyumba);
            newGameStates.Add(newGameState);

            if (playNyumba)
            {
                BaoGameState playNyumbaGameState = this.DeepCopy();
                BaoMove nyumbaMove = new BaoMove(move.rowBox, move.columnBox, move.left);
                nyumbaMove.playNyumba = true;
                playNyumbaGameState.UpdateGameState(nyumbaMove, player, out playNyumba);
                newGameStates.Add(playNyumbaGameState);
            }

            return newGameStates;
        }

        public List<BaoMove> GetPossibleMoves(Byte player, out MustMove explanation)
        {
            if (_namua)
            {
                return GetPossibleMovesNamua(player, out explanation);
            }
            else
            {
                return GetPossibleMovesMtaji(player, out explanation);
            }
        }

        public WinCondition WinLoseCondition(Byte currPlayer)
        {
            Debug.Assert(currPlayer == 1 || currPlayer == 2);
            MustMove explanation = MustMove.none;

            List<BaoMove> possibleMoves = GetPossibleMoves(currPlayer, out explanation);

            // if actual player has no possible moves, he loses
            if (possibleMoves.Count == 0)   
            {
                if (currPlayer == 1)    // computer win condition
                {
                    return WinCondition.Player2;
                }
                else                    // computer lose condition
                {
                    return WinCondition.Player1;
                }
            }
            else if (EmptyInnerRow(1))   // player 1 has an empy inner row, CPU wins
            {
                return WinCondition.Player2;
            }
            else if (EmptyInnerRow(2))   // player 2 has an empty inner row, Player 1 wins
            {
                return WinCondition.Player1;
            }
            else
            {
                return WinCondition.none;
            }
        }

        /*public float EvaluateGameState()
        {
            float CPUscore = 0.0f;
            int emptyFrontRowShimo;
            float backRowKeteScore = 9;
            float emptyInnerRowPenalty = 7;

            float[] frontRowKeteLookUpNamua = {0,10,17,24,30,35,39,42,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68};
            float[] frontRowKeteLookUpMtaji = {0, 5,17,24,30,35,39,42,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68};
            float[] nyumbaKeteLookUp        = {0, 7,14,21,28,35,42,49,56,63,70,77,84,91,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116};

            int i;

            // every kete in the back row score backRowKeteScore points
            for (i = 1; i <= 8; i++)
            {
                CPUscore += _board[0, i] * backRowKeteScore;
            }

            // Every kete in the front row score depending on look up and game stage
            emptyFrontRowShimo = 0;
            for (i = 1; i <= 8; i++)
            {
                if (_namua)
                {
                    if (i == 4 && _nyumba2)  // nyumba
                    {
                        CPUscore += nyumbaKeteLookUp[_board[1, i]];
                    }
                    else
                    {
                        CPUscore += frontRowKeteLookUpNamua[_board[1, i]];
                    }
                }
                else
                {
                    CPUscore += frontRowKeteLookUpMtaji[_board[1, i]];
                }
                
                if (_board[1, i] == 0)
                    emptyFrontRowShimo++;
            }

            // Put a penality for every empty front row after the second one
            if (emptyFrontRowShimo >= 2)
            {
                CPUscore -= (emptyFrontRowShimo - 2) * emptyInnerRowPenalty;
            }
                
            emptyFrontRowShimo = 0;

            // same logic for the player, score is the opposite

            // every kete in the back row score backRowKeteScore points
            for (i = 1; i <= 8; i++)
            {
                CPUscore -= _board[3, i] * backRowKeteScore;
            }

            // Every kete in the front row score depending on look up and game stage
            emptyFrontRowShimo = 0;
            for (i = 1; i <= 8; i++)
            {
                if (_namua)
                {
                    if (i == 5 && _nyumba1)  // nyumba
                    {
                        CPUscore -= nyumbaKeteLookUp[_board[2, i]];
                    }
                    else
                    {
                        CPUscore -= frontRowKeteLookUpNamua[_board[2, i]];
                    }
                }
                else
                {
                    CPUscore -= frontRowKeteLookUpMtaji[_board[2, i]];
                }

                if (_board[1, i] == 0)
                    emptyFrontRowShimo++;
            }

            // Put a penality for every empty front row after the second one
            if (emptyFrontRowShimo >= 2)
            {
                CPUscore += (emptyFrontRowShimo - 2) * emptyInnerRowPenalty;
            }
                
            return CPUscore;
        }*/

        // very simplified version (for possible extension it is possible to give different points to nyumba
        // keeping in order to generate different king of cpu ai)
        public float EvaluateGameState()
        {
            float CPUscore = 0.0f;

            float backRowKeteScore = 1.1f;
            float frontRowKeteScore = 1.0f;
            float emptyInnerRowPenalty = 0.1f;
            float nyumbaBonus = 5.0f;

            int i;

            for (i = 1; i <= 8; i++)
            {
                CPUscore += _board[0, i] * backRowKeteScore;
            }

            for (i = 1; i <= 8; i++)
            {
                CPUscore += _board[1, i] * frontRowKeteScore;

                if (_board[1, i] == 0)
                    CPUscore -= emptyInnerRowPenalty;
            }

            if (_nyumba2)
                CPUscore += nyumbaBonus;

            for (i = 1; i <= 8; i++)
            {
                CPUscore -= _board[3, i] * backRowKeteScore;
            }

            for (i = 1; i <= 8; i++)
            {
                CPUscore -= _board[2, i] * frontRowKeteScore;

                if (_board[2, i] == 0)
                    CPUscore += emptyInnerRowPenalty;
            }

            if (_nyumba1)
                CPUscore -= nyumbaBonus;
            
            return CPUscore;
        }

    # endregion    
        
    }

    public class BaoMinimaxElement
    {
        public BaoGameState gameState;

        private float score;

        private bool evaluated;

        public BaoMinimaxElement()
        {
            gameState = null;
            score = 0;
            evaluated = false;
        }

        public BaoMinimaxElement(BaoGameState gs)
        {
            gameState = gs;
            score = 0;
            evaluated = false;
        }

        public BaoMinimaxElement(BaoGameState gs, float s)
        {
            gameState = gs;
            score = s;
            evaluated = true;
        }

        public float Score
        {
            get { return score; }
            set { score = value; evaluated = true; }
        }

        public static bool operator < (BaoMinimaxElement left, BaoMinimaxElement right)
        {
            Debug.Assert(left.evaluated && right.evaluated);

            if( left.score < right.score )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator > (BaoMinimaxElement left, BaoMinimaxElement right)
        {
            Debug.Assert(left.evaluated && right.evaluated);

            if (left.score > right.score)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class ScreenAction
    {
        #region private members

        private ActionType _action;

        private Byte _row;

        private Byte _col;

        private Byte _numOfSeeds;

        #endregion

        #region constructors

        public ScreenAction()
        {
            _action = ActionType.none;
            _row = 0;
            _col = 0;
            _numOfSeeds = 0;
        }

        public ScreenAction(ActionType action, Byte row, Byte col, Byte numOfSeeds)
        {
            _action = action;
            _row = row;
            _col = col;
            _numOfSeeds = numOfSeeds;
        }

        #endregion

        #region properties

        public ActionType Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public Byte Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public Byte Col
        {
            get { return _col; }
            set { _col = value; }
        }

        public Byte Seeds
        {
            get { return _numOfSeeds; }
            set { _numOfSeeds = value; }
        }

        #endregion


    }
}

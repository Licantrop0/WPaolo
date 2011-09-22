using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace BaoUtil
{
    enum Versor
    {
        left,
        right
    }

    public enum WinCondition
    {
        none,
        Player1,
        Player2
    }

    public class BaoGameState
    {
    # region private members

        Byte[,] _board;

        Byte _player;   // player whose move got to the current Gamestate

        BaoMove _move;

    # endregion

    #region costructors

        public BaoGameState()
        {
        }

        public BaoGameState(Byte player)
        {
            // note, player is the the one whose move got to the current Gamestate
            // so if player is Player1, the player to move next is Player2 and viceversa
            _player = player;

            _move = new BaoMove();      // will be set to auto move

            // initialize board
            _board = new byte[4, 10] { { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 }, { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 },  { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 },  { 0, 2, 2, 2, 2, 2, 2, 2, 2, 0 } };
        }

        public BaoGameState DeepCopy()
        {
            BaoGameState copiedObject = new BaoGameState();

            copiedObject._player = _player;
            copiedObject._move = _move.DeepCopy();
            copiedObject._board = new byte[4, 10];
            Array.Copy(_board, copiedObject._board, copiedObject._board.Length);

            return copiedObject;
        }

    #endregion

    #region private methods

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

        private void Capture(ref Byte row, ref Byte col, ref Versor versor)
        {
            int numOfKete = Pick(OppositeRow(row), col);

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

        private void Sow(ref Byte row, ref Byte col, ref Versor versor, int numOfKete)
        {
            while (numOfKete > 0)
            {
                NextShimo(ref row, ref col, ref versor);
                _board[row, col]++;
                
                numOfKete--;
            }
        }

        private int Pick(Byte row, Byte col)
        {
            int numOfKete = _board[row, col];
            _board[row, col] = 0;

            return numOfKete;
        }

        private Byte OppositeRow(Byte row)
        {
            Debug.Assert(row == 1 || row == 2);

            if (row == 1)
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

            for (int i = 1; i <= 8; i++ )
            {
                if (_board[innerRow, i] > 0)
                    return false;
            }

            return true;
        }

    #endregion

    # region properties

        public Byte Player { get { return _player; } set { _player = value; } }

        public BaoMove Move { get { return _move; } }

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

        public void UpdateGameState(BaoMove move, Byte player)
        {
            // does not check for a valid move, check must be done somewhere else, so move MUST be a valid one
            int submoves = 0;

            // set final state of player and move
            _player = player;
            _move = move;

            //Print();

            // now set final state of the board
            // pick all the kete int shimo targetted by move
            Byte row = move.rowBox;
            Byte col = move.columnBox;
            int numOfKete = Pick(row, col);

            // init the initial versor
            Versor versor = move.left ? Versor.left : Versor.right;

            Sow(ref row, ref col, ref versor, numOfKete);
            submoves++;
            //Print();

            // see if I can capture
            if (CanCapture(row, col))
            {
                // turn with captures
                while ((CanCapture(row, col) || CanRelaySowing(row, col)) && submoves < 50)
                {
                    if (CanCapture(row, col))
                    {
                        Capture(ref row, ref col, ref versor);
                        submoves++;
                        //Print();
                    }
                    else if (CanRelaySowing(row, col))
                    {
                        numOfKete = Pick(row, col);
                        Sow(ref row, ref col, ref versor, numOfKete);
                        submoves++;
                        //Print();
                    }
                }
            }
            else
            {
                // turn without captures
                while (CanRelaySowing(row, col) && submoves < 50)
                {
                    numOfKete = Pick(row, col);
                    Sow(ref row, ref col, ref versor, numOfKete);
                    submoves++;
                    //Print();
                }
            }
        }

        public BaoGameState NewGameStateFromExisting(BaoMove move, Byte player)
        {
            BaoGameState newGameState = this.DeepCopy();
            newGameState.UpdateGameState(move, player);
            return newGameState;
        }

        public List<BaoMove> GetPossibleMoves(Byte player)
        {
            bool captureMove = false;
            bool takasaInnerRow = false;
            bool takasaOuterRow = false;

            List<BaoMove> captureMoves = new List<BaoMove>();
            List<BaoMove> takasaInnerRowMoves = new List<BaoMove>();
            List<BaoMove> takasaOuterRowMoves = new List<BaoMove>();

            Debug.Assert(player == 1 || player == 2);

            Byte startRow = (player == 2) ? (Byte)0 : (Byte)2;

            for (Byte row = startRow; row < startRow + 2; row++)
            {
                for (Byte col = 1; col <= 8; col++)
                {
                    int numOfKete = _board[row,col];

                    // go to next shimo if there are not any kete
                    if (numOfKete <= 1)
                    {
                        continue;
                    }

                    // there are more than one kete, possible start for capture move
                    if (numOfKete <= 15)    // this is a necessary condition for a capture move
                    {
                        bool left = false;
                        int i = 0;
                        
                        while(i < 2)
                        {
                            BaoMove currMove = new BaoMove(row, col, left);

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
                                captureMove = true;
                                captureMoves.Add(currMove);
                            }
                            else
                            {
                                if( row == 1 || row == 2 )  // inner rows
                                {
                                    takasaInnerRow = true;
                                    takasaInnerRowMoves.Add(currMove);
                                }
                                else                        // outer rows
                                {
                                    takasaOuterRow = true;
                                    takasaOuterRowMoves.Add(currMove);
                                }
                            }
                            
                            i++;
                            left = !left;
                        }
                    }
                    else
                    {
                        if( row == 1 || row == 2 )  // inner rows
                        {
                            takasaInnerRow = true;
                            takasaInnerRowMoves.Add(new BaoMove(row,col,true));
                            takasaInnerRowMoves.Add(new BaoMove(row,col,false));
                        }
                        else                        // outer rows
                        {
                            takasaOuterRow = true;
                            takasaOuterRowMoves.Add(new BaoMove(row,col,true));
                            takasaOuterRowMoves.Add(new BaoMove(row,col,false));
                        }
                    }
                }
            }

            if (captureMove)
            {
                return captureMoves;
            }
            else if (takasaInnerRow)
            {
                return takasaInnerRowMoves;
            }
            else if (takasaOuterRow)
            {
                return takasaOuterRowMoves;
            }
            else
            {
                return new List<BaoMove>();     // return an enmpty list
            }
           
        }

        public WinCondition WinLoseCondition(Byte currPlayer, List<BaoMove> possibleMoves)
        {
            Debug.Assert(currPlayer == 1 || currPlayer == 2);

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


        public float EvaluateGameState()
        {
            // TODO: riscrivere il codice che fa schifo al cazzo

            float CPUscore = 0.0f;
            int emptyFronRowShimo = 0;

            float[] frontRowKeteLookUp = {0,5,10,14,18,21,24,26,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52};

            int i;

            // every kete in the back row score 5 points
            for (i = 1; i <= 8; i++)
            {
                CPUscore += _board[0, i] * 5;
            }

            // Every kete in the front row score depending on look up
            for (i = 1; i <= 8; i++)
            {
                CPUscore += _board[1, i] * frontRowKeteLookUp[_board[1, i]];

                if (_board[1, i] == 0)
                    emptyFronRowShimo++;
            }

            // Put a penality for every empty front row after the second one
            if (emptyFronRowShimo >= 2)
                CPUscore -= (emptyFronRowShimo - 2) * 5;

            emptyFronRowShimo = 0;

            // same logic for the player, score is the opposite
            // every kete in the back row score 5 points
            for (i = 1; i <= 8; i++)
            {
                CPUscore -= _board[3, i] * 5;
            }

            // Every kete in the front row score depending on look up
            for (i = 1; i <= 8; i++)
            {
                CPUscore -= _board[2, i] * frontRowKeteLookUp[_board[2, i]];

                if (_board[2, i] == 0)
                    emptyFronRowShimo++;
            }

            // Put a penality for every empty front row after the second one
            if (emptyFronRowShimo >= 2)
                CPUscore += (emptyFronRowShimo - 2) * 5;

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
}

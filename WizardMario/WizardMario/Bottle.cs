using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardMario
{
    public enum BottleState
    {
        idle,
        playerControlPillow,
        stop,
        destruction,
        fallingStones
    }

    public struct GridPosition
    {
        public int Row;
        public int Col;

        public GridPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public GridPosition(GridPosition gridPosition)
        {
            Row = gridPosition.Row;
            Col = gridPosition.Col;
        }
    }

    class GridPositionComparer : IEqualityComparer<GridPosition>
    {
        public bool Equals(GridPosition x, GridPosition y)
        {
            return x.Row == y.Row && x.Col == y.Col;
        }

        public int GetHashCode(GridPosition obj)
        {
            return obj.Row.GetHashCode() ^ obj.Col.GetHashCode();
        }
    }


    public class Bottle
    {
        #region private members

        IWizardMarioElement[,] _board = new IWizardMarioElement[17, 8];  // it is possible to rotate in vertical in the very top of the bottle

        Pillow _actualPillow;

        BottleState _state = BottleState.idle;

        List<Stone> _fallingStones = new List<Stone>();

        float _bottleTimer = 0;
        float _fallingTime;
        float _stopTime;
        float _autoFallingTime;

        #endregion

        #region public methods

        public void Initialize(int level)
        {
            // TODO logica incredibile che inizializzi il board a seconda del numero del livello

            // HARD CABLED!
            _board[14, 3] = new Monster(ElementColor.red);
            _board[10, 7] = new Monster(ElementColor.yellow);
            _board[12, 2] = new Monster(ElementColor.blue);
        }

        public void Update(float dt, string action)
        {
            _bottleTimer += dt;

            switch (_state)
            {
                case BottleState.idle:

                    break;

                case BottleState.playerControlPillow:

                    // resolve player input actions
                    if (action == "moveLeft")
                    {
                        if (CheckAndMoveLeft())
                        {
                            UpdateActualPillowPosition();
                        }
                    }
                    else if (action == "moveRight")
                    {
                        if (CheckAndMoveRight())
                        {
                            UpdateActualPillowPosition();
                        }
                    }
                    else if (action == "turnLeft")
                    {
                        if (CheckAndTurnLeft())
                        {
                            UpdateActualPillowPosition();
                        }
                    }
                    else if (action == "turnRight")
                    {
                        if (CheckAndTurnRight())
                        {
                            UpdateActualPillowPosition();
                        }
                    }
                    else if (action == "moveDown")
                    {
                        if (FallForOneStep())
                        {
                            UpdateActualPillowPosition();
                        }
                        else
                        {
                            StuckActualPillowToTheBoard();
                            _state = BottleState.stop;
                            _bottleTimer = 0;
                            break;
                        }
                    }

                    if (_bottleTimer >= _fallingTime)
                    {
                        if (FallForOneStep())
                        {
                            UpdateActualPillowPosition();
                            _bottleTimer -= _fallingTime;
                        }
                        else
                        {
                            _state = BottleState.stop;
                            _bottleTimer = 0;
                            break;
                        }
                    }

                    break;

                case BottleState.stop:
                    // I have to wait a little bit before checking for destructions tetris
                    if (_bottleTimer >= _stopTime)
                    {
                        _bottleTimer = 0;

                        if (CheckTetris())
                        {
                            _state = BottleState.destruction;
                        }
                        else
                        {
                            _state = BottleState.idle;
                        }
                    }

                    break;

                case BottleState.destruction:

                    if (CheckForFallingStones())
                    {
                        _state = BottleState.fallingStones;
                    }

                    break;

                case BottleState.fallingStones:

                    if (_bottleTimer >= _autoFallingTime)
                    {
                        _bottleTimer = 0;



                        if (CheckTetris())
                        {
                            _state = BottleState.destruction;
                        }
                        else
                        {
                            _state = BottleState.idle;
                        }
                    }

                    break;

                default:
                    break;
            }
        }

        private void StuckActualPillowToTheBoard()
        {
            throw new NotImplementedException();
        }

        private bool CheckForFallingStones()
        {
            _fallingStones.Clear();

            for (int i = 1; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (_board[i,j] != null && _board[i,j].GetType().ToString() == "Stone")
                    {
                        Stone currentStone = (Stone)_board[i,j];

                        // current element is a stone
                        // the stone is falling if:
                        // - it is not linked and the element underneath is either void or a falling stone
                        // - it is linked and both the elements underneath it and underneath the linked stone
                        //   are either void or falling stone
                        if ( currentStone.LinkedTo == null )
                        {
                            if (_board[i-1,j] == null || _board[i-1, j].Floating)
                            {
                                currentStone.Floating = true;
                                _fallingStones.Add(currentStone);
                            }
                        }
                        else
                        {
                            Stone linkedStone = currentStone.LinkedTo;
                            
                            // we can distinguish west, south, east and north
                            if (linkedStone.Col == j - 1)       // west
                            {
                                if (linkedStone.Floating)
                                {
                                    currentStone.Floating = true;
                                    _fallingStones.Add(currentStone);
                                }
                            }
                            else if (linkedStone.Row == i - 1)  // south
                            {
                                if (linkedStone.Floating)
                                {
                                    currentStone.Floating = true;
                                    _fallingStones.Add(currentStone);
                                }
                            }
                            else if (linkedStone.Col == j + 1)  // east
                            {
                                if ( (_board[i-1, j] == null || _board[i-1, j].Floating) && 
                                     (_board[i-1, linkedStone.Col] == null || _board[i-1, linkedStone.Col].Floating) )
                                {
                                    currentStone.Floating = true;
                                    _fallingStones.Add(currentStone);
                                }
                            }
                            else if (linkedStone.Row == i + 1)  // north
                            {
                                if (_board[i - 1, j] == null || _board[i - 1, j].Floating)
                                {
                                    currentStone.Floating = true;
                                    _fallingStones.Add(currentStone);
                                }
                            }
                        }
                        
                    }
                }
            }

            return (_fallingStones.Count > 0);
        }

        private bool CheckAndMoveLeft()
        {
            int firstStoneRow = _actualPillow.FirstStone.Row;
            int firstStoneCol = _actualPillow.FirstStone.Col;
            int secondStoneRow = _actualPillow.SecondStone.Row;
            int secondStoneCol = _actualPillow.SecondStone.Col;

            if (firstStoneCol == 0 || secondStoneCol == 0)
            {
                return false;
            }

            if (_board[firstStoneRow, firstStoneCol - 1] != null || _board[secondStoneRow, secondStoneCol - 1] != null)
            {
                return false;
            }

            _actualPillow.FirstStone.Col--;
            _actualPillow.SecondStone.Col--;

            if (_actualPillow.FirstStone.Col == 0 &&
                _actualPillow.SecondStone.Col == 0 &&
                _actualPillow.EmptySpaceVerse == EmptySpaceVerse.left)
            {
                _actualPillow.EmptySpaceVerse = EmptySpaceVerse.right;
            }

            return true;
        }

        private bool CheckAndMoveRight()
        {
            int firstStoneRow = _actualPillow.FirstStone.Row;
            int firstStoneCol = _actualPillow.FirstStone.Col;
            int secondStoneRow = _actualPillow.SecondStone.Row;
            int secondStoneCol = _actualPillow.SecondStone.Col;

            if (firstStoneCol == 7 || secondStoneCol == 7)
            {
                return false;
            }

            if (_board[firstStoneRow, firstStoneCol + 1] != null || _board[secondStoneRow, secondStoneCol + 1] != null)
            {
                return false;
            }

            _actualPillow.FirstStone.Col++;
            _actualPillow.SecondStone.Col++;

            if (_actualPillow.FirstStone.Col == 7 &&
                _actualPillow.SecondStone.Col == 7 &&
                _actualPillow.EmptySpaceVerse == EmptySpaceVerse.right)
            {
                _actualPillow.EmptySpaceVerse = EmptySpaceVerse.left;
            }

            return true;
        }

        private bool CheckAndTurnLeft()
        {
            int firstStoneRow = _actualPillow.FirstStone.Row;
            int firstStoneCol = _actualPillow.FirstStone.Col;
            int secondStoneRow = _actualPillow.SecondStone.Row;
            int secondStoneCol = _actualPillow.SecondStone.Col;

            Stone stoneToTurn, stoneFixed, stoneToTurnAndSlide, stoneToSlide;

            if (_actualPillow.Verse == PillowVerse.orizzontal)
            {
                // I have to check the free space upon the leftmost stone
                if (firstStoneCol < secondStoneCol)
                {
                    stoneToTurn = _actualPillow.SecondStone;
                    stoneFixed = _actualPillow.FirstStone;
                }
                else
                {
                    stoneToTurn = _actualPillow.FirstStone;
                    stoneFixed = _actualPillow.SecondStone;
                }

                if (_board[stoneFixed.Row + 1, stoneFixed.Col] == null)
                {
                    stoneToTurn.Row = stoneToTurn.Row + 1;
                    stoneToTurn.Col = stoneToTurn.Col - 1;
                    _actualPillow.Verse = PillowVerse.vertical;
                    _actualPillow.EmptySpaceVerse = EmptySpaceVerse.right;

                    return true;
                }
            }
            else
            {
                // pillow verse is vertical
                if (_actualPillow.EmptySpaceVerse == EmptySpaceVerse.left)
                {
                    // I have to check the free space left to the beneath stone
                    if (firstStoneRow < secondStoneRow)
                    {
                        stoneFixed = _actualPillow.FirstStone;
                        stoneToTurn = _actualPillow.SecondStone;
                    }
                    else
                    {
                        stoneFixed = _actualPillow.SecondStone;
                        stoneToTurn = _actualPillow.FirstStone;
                    }

                    if (_board[stoneFixed.Row, stoneFixed.Col - 1] == null)
                    {
                        stoneToTurn.Row = stoneToTurn.Row - 1;
                        stoneToTurn.Col = stoneToTurn.Col - 1;
                        _actualPillow.Verse = PillowVerse.orizzontal;
                        _actualPillow.EmptySpaceVerse = EmptySpaceVerse.up;

                        return true;
                    }
                }
                else
                {
                    // in this case the pillow turn and slide to fit its space
                    // I have to check if there is space to the right of the beneath stone
                    if (firstStoneRow < secondStoneRow)
                    {
                        stoneToSlide = _actualPillow.FirstStone;
                        stoneToTurnAndSlide = _actualPillow.SecondStone;
                    }
                    else
                    {
                        stoneToSlide = _actualPillow.SecondStone;
                        stoneToTurnAndSlide = _actualPillow.FirstStone;
                    }

                    if (_board[stoneToSlide.Row, stoneToSlide.Col + 1] == null)
                    {
                        stoneToSlide.Col = stoneToSlide.Col + 1;
                        stoneToTurnAndSlide.Row = stoneToTurnAndSlide.Row - 1;
                        _actualPillow.Verse = PillowVerse.orizzontal;
                        _actualPillow.EmptySpaceVerse = EmptySpaceVerse.up;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckAndTurnRight()
        {
            int firstStoneRow = _actualPillow.FirstStone.Row;
            int firstStoneCol = _actualPillow.FirstStone.Col;
            int secondStoneRow = _actualPillow.SecondStone.Row;
            int secondStoneCol = _actualPillow.SecondStone.Col;

            Stone stoneToTurn, stoneFixed, stoneToTurnAndSlide, stoneToSlide;

            if (_actualPillow.Verse == PillowVerse.orizzontal)
            {
                // I have to check the free space upon the rightmost stone
                if (firstStoneCol > secondStoneCol)
                {
                    stoneToTurn = _actualPillow.SecondStone;
                    stoneFixed = _actualPillow.FirstStone;
                }
                else
                {
                    stoneToTurn = _actualPillow.FirstStone;
                    stoneFixed = _actualPillow.SecondStone;
                }

                if (_board[stoneFixed.Row + 1, stoneFixed.Col] == null)
                {
                    stoneToTurn.Row = stoneToTurn.Row + 1;
                    stoneToTurn.Col = stoneToTurn.Col + 1;
                    _actualPillow.Verse = PillowVerse.vertical;
                    _actualPillow.EmptySpaceVerse = EmptySpaceVerse.left;

                    return true;
                }
            }
            else
            {
                // pillow verse is vertical
                if (_actualPillow.EmptySpaceVerse == EmptySpaceVerse.right)
                {
                    // I have to check the free space right to the beneath stone
                    if (firstStoneRow < secondStoneRow)
                    {
                        stoneFixed = _actualPillow.FirstStone;
                        stoneToTurn = _actualPillow.SecondStone;
                    }
                    else
                    {
                        stoneFixed = _actualPillow.SecondStone;
                        stoneToTurn = _actualPillow.FirstStone;
                    }

                    if (_board[stoneFixed.Row, stoneFixed.Col + 1] == null)
                    {
                        stoneToTurn.Row = stoneToTurn.Row - 1;
                        stoneToTurn.Col = stoneToTurn.Col + 1;
                        _actualPillow.Verse = PillowVerse.orizzontal;
                        _actualPillow.EmptySpaceVerse = EmptySpaceVerse.up;

                        return true;
                    }
                }
                else
                {
                    // in this case the pillow turn and slide to fit its space
                    // I have to check if there is space to the left of the beneath stone
                    if (firstStoneRow < secondStoneRow)
                    {
                        stoneToSlide = _actualPillow.FirstStone;
                        stoneToTurnAndSlide = _actualPillow.SecondStone;
                    }
                    else
                    {
                        stoneToSlide = _actualPillow.SecondStone;
                        stoneToTurnAndSlide = _actualPillow.FirstStone;
                    }

                    if (_board[stoneToSlide.Row, stoneToSlide.Col - 1] == null)
                    {
                        stoneToSlide.Col = stoneToSlide.Col - 1;
                        stoneToTurnAndSlide.Row = stoneToTurnAndSlide.Row - 1;
                        _actualPillow.Verse = PillowVerse.orizzontal;
                        _actualPillow.EmptySpaceVerse = EmptySpaceVerse.up;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool FallForOneStep()
        {
            int firstStoneRow = _actualPillow.FirstStone.Row;
            int firstStoneCol = _actualPillow.FirstStone.Col;
            int secondStoneRow = _actualPillow.SecondStone.Row;
            int secondStoneCol = _actualPillow.SecondStone.Col;

            if (firstStoneRow == 0 || secondStoneRow == 0)
            {
                return false;
            }

            if (_board[firstStoneRow - 1, firstStoneCol] != null || _board[secondStoneRow - 1, secondStoneCol] != null)
            {
                return false;
            }

            _actualPillow.FirstStone.Row--;
            _actualPillow.SecondStone.Row--;

            return true;
        }

        private bool CheckTetris()
        {
            List<GridPosition> toBeDestroyed = new List<GridPosition>();
            int streak;
            ElementColor actualColor;
            bool tetrisHappened = false;

            // first check vertical combo
            for (int j = 0; j < 8; j++)
            {
                actualColor = ElementColor.none;
                streak = 0;

                for (int i = 0; j < 16; i++)
                {
                    CheckTetrisUtil(i, j, ref actualColor, ref streak, ref toBeDestroyed, ref tetrisHappened);
                }
            }

            // now with the same exact strategy I check orizontal combo
            for (int i = 0; i < 16; i++)
            {
                actualColor = ElementColor.none;
                streak = 0;

                for (int j = 0; j < 8; i++)
                {
                    CheckTetrisUtil(i, j, ref actualColor, ref streak, ref toBeDestroyed, ref tetrisHappened);
                }
            }
            
            if (tetrisHappened)
            {
                // delete any duplicate from toBeDestroyed list
                toBeDestroyed = toBeDestroyed.Distinct(new GridPositionComparer()).ToList();

                // destroy elements
                for (int i = 0; i < toBeDestroyed.Count; i++)
                {
                    _board[toBeDestroyed[i].Row, toBeDestroyed[i].Col].Destroy();
                }
            }

            return tetrisHappened;
        }

        private void CheckTetrisUtil(int i, int j, 
                                     ref ElementColor actualColor, ref int streak,
                                     ref List<GridPosition> toBeDestroyed, ref bool tetrisHappened)
        {
            if (_board[i, j] != null && _board[i, j].Color == actualColor)
            {
                // color is equal to the previous one, add to the streak
                streak++;
            }
            else
            {
                // color is not equal or the cell is void, I check if streak is 4 or more so deletion can happen
                if (streak >= 4)
                {
                    tetrisHappened = true;
                    int beginRow = i - streak;

                    for (int row = beginRow; row < i; row++)
                    {
                        toBeDestroyed.Add(new GridPosition(row, j));
                    }
                }

                if (_board[i, j] != null)
                {
                    actualColor = ElementColor.none;
                    streak = 0;
                }
                else
                {
                    actualColor = _board[i, j].Color;
                    streak = 1;
                }
            }
        }

        private void UpdateActualPillowPosition()
        {
            throw new NotImplementedException();
        }

        public void InsertPillow(Pillow pillow)
        {
            _actualPillow = pillow;

            _actualPillow.FirstStone.Row = 16;
            _actualPillow.FirstStone.Col = 3;

            _actualPillow.SecondStone.Row = 16;
            _actualPillow.SecondStone.Col = 4;
        }



        #endregion


        internal void Draw()
        {
            throw new NotImplementedException();
        }
    }
}

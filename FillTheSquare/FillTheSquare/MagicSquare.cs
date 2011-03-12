using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace FillTheSquare
{
    public class MagicSquare
    {
        /// <summary>
        /// array bidimensionale che rappresenta il quadrato magico
        /// </summary>
        private int[,] grid;

        /// <summary>
        /// tiene traccia di tutti gli spostamenti
        /// </summary>
        public Stack<Point> positionHistory;

        /// <summary>
        /// indica la coordinata dell'ultimo numero inserito
        /// </summary>
        /// 

        public int ActualSize { get; private set; }

        public MagicSquare(int size)
        {
            if (size != 5 && size != 10)
                throw new ArgumentException("The square could be only 5x5 or 10x10", "size");

            ActualSize = size;

            grid = new int[ActualSize, ActualSize];
            positionHistory = new Stack<Point>();
        }

        public bool PressButton(Point p)
        {
            if (positionHistory.Count != 0)
            {
                if (p == positionHistory.Peek())    //voglio cancellare l'ultima mossa
                {
                    positionHistory.Pop();
                    grid[(int)(p.X), (int)(p.Y)] = 0;
                    return true;
                }
                else
                {
                    if (positionHistory.Peek().X != -1 && positionHistory.Peek().Y != -1)   //se ho già fatto la prima mossa
                    {
                        if (((p.X == positionHistory.Peek().X - 3) && (p.Y == positionHistory.Peek().Y)) ||
                            ((p.X == positionHistory.Peek().X - 2) && (p.Y == positionHistory.Peek().Y - 2)) ||
                            ((p.X == positionHistory.Peek().X) && (p.Y == positionHistory.Peek().Y - 3)) ||
                            ((p.X == positionHistory.Peek().X + 2) && (p.Y == positionHistory.Peek().Y - 2)) ||
                            ((p.X == positionHistory.Peek().X + 3) && (p.Y == positionHistory.Peek().Y)) ||
                            ((p.X == positionHistory.Peek().X + 2) && (p.Y == positionHistory.Peek().Y + 2)) ||
                            ((p.X == positionHistory.Peek().X) && (p.Y == positionHistory.Peek().Y + 3)) ||
                            ((p.X == positionHistory.Peek().X - 2) && (p.Y == positionHistory.Peek().Y + 2)))
                        {
                            if (positionHistory.Contains(p))
                            {
                                return false;   //casella già occupata
                            }
                        }
                        else
                        {
                            return false;   //non ho rispettato le regole
                        }
                    }
                }
            }
            positionHistory.Push(p);
            grid[(int)(p.X), (int)(p.Y)] = positionHistory.Count;
            return true;
        }

        public void Reset()
        {
            for (int i = 0; i < ActualSize; i++)
            {
                for (int j = 0; j < ActualSize; j++)
                {
                    grid[i, j] = 0;
                }
            }
            positionHistory = new Stack<Point>();
        }

        public int NumberMovesLeft()
        {
            int nAllowedMoves = 0;
            int x = (int)positionHistory.Peek().X;
            int y = (int)positionHistory.Peek().Y;
            if ((x - 3) >= 0)
            {
                if (grid[(int)(positionHistory.Peek().X - 3), (int)(positionHistory.Peek().Y)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            if ((x - 2) >= 0 && (y - 2) >= 0)
            {
                if (grid[(int)(positionHistory.Peek().X - 2), (int)(positionHistory.Peek().Y - 2)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            if ((y - 3) >= 0)
            {
                if (grid[(int)(positionHistory.Peek().X), (int)(positionHistory.Peek().Y - 3)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            if ((x + 2) <= (ActualSize - 1) && (y - 2) >= 0)
            {
                if (grid[(int)(positionHistory.Peek().X + 2), (int)(positionHistory.Peek().Y - 2)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            if ((x + 3) <= (ActualSize - 1))
            {
                if (grid[(int)(positionHistory.Peek().X + 3), (int)(positionHistory.Peek().Y)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            if ((x + 2) <= (ActualSize - 1) && (y + 2) <= (ActualSize - 1))
            {
                if (grid[(int)(positionHistory.Peek().X + 2), (int)(positionHistory.Peek().Y + 2)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            if ((y + 3) <= (ActualSize - 1))
            {
                if (grid[(int)(positionHistory.Peek().X), (int)(positionHistory.Peek().Y + 3)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            if ((x - 2) >= 0 && (y + 2) <= (ActualSize - 1))
            {
                if (grid[(int)(positionHistory.Peek().X - 2), (int)(positionHistory.Peek().Y + 2)] < 1)
                {
                    nAllowedMoves++;
                }
            }
            return nAllowedMoves;
        }
    }
}

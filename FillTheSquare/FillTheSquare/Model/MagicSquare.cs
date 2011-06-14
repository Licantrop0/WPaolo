using System;
using System.Collections.Generic;
using System.Windows;

namespace FillTheSquare
{
    public class MagicSquare
    {
        /// <summary>
        /// array bidimensionale che rappresenta il quadrato magico
        /// </summary>
        private int[,] Grid;

        /// <summary>
        /// tiene traccia di tutti gli spostamenti
        /// </summary>
        public Stack<GridPoint> PositionHistory { get; set; }

        /// <summary>
        /// Dimensione della Griglia
        /// </summary>
        public int Size { get; private set; }

        public bool IsCompleted { get { return PositionHistory.Count == Size * Size; } }
        public bool IsEmpty { get { return PositionHistory.Count == 0; } }

        public MagicSquare(int size, GridPoint[] points)
        {
            if (size != 5 && size != 10)
                throw new ArgumentException("The square could be only 5x5 or 10x10", "size");

            Size = size;

            Grid = new int[Size, Size];
            PositionHistory = new Stack<GridPoint>(points);
        }

        public bool? PressButton(GridPoint p)
        {
            //se ho già fatto la prima mossa
            if (PositionHistory.Count != 0)
            {
                var LastPoint = PositionHistory.Peek();

                //voglio cancellare l'ultima mossa
                if (p == LastPoint)
                {
                    this.Grid[p.X, p.Y] = 0;
                    PositionHistory.Pop();
                    return null;
                }

                //Casella già occupata
                if (PositionHistory.Contains(p))
                    return false;

                bool rules =
                    (p.X == LastPoint.X + 3 && p.Y == LastPoint.Y) ||     //+3,+0
                    (p.X == LastPoint.X && p.Y == LastPoint.Y + 3) ||     //+0,+3
                    (p.X == LastPoint.X - 3 && p.Y == LastPoint.Y) ||     //-3,+0
                    (p.X == LastPoint.X && p.Y == LastPoint.Y - 3) ||     //+0,-3
                    (p.X == LastPoint.X + 2 && p.Y == LastPoint.Y + 2) || //+2,+2
                    (p.X == LastPoint.X - 2 && p.Y == LastPoint.Y - 2) || //-2,-2
                    (p.X == LastPoint.X + 2 && p.Y == LastPoint.Y - 2) || //+2,-2
                    (p.X == LastPoint.X - 2 && p.Y == LastPoint.Y + 2);   //-2,+2

                //non ho rispettato le regole
                if (!rules) return false;
            }

            PositionHistory.Push(p);
            Grid[p.X, p.Y] = PositionHistory.Count;

            return true; //tutto ok
        }

        public void Clear()
        {
            Array.Clear(Grid, 0, Grid.Length);
            PositionHistory.Clear();
        }

        public List<GridPoint> GetAvailableMoves()
        {
            int x = PositionHistory.Peek().X;
            int y = PositionHistory.Peek().Y;
            var AvailablePoints = new List<GridPoint>();

            //+3,+0
            if ((x + 3) <= (Size - 1) && Grid[x + 3, y] == 0)
                AvailablePoints.Add(new GridPoint(x + 3, y));

            //+0,+3
            if ((y + 3) <= (Size - 1) && Grid[x, y + 3] == 0)
                AvailablePoints.Add(new GridPoint(x, y + 3));

            //-3,+0
            if ((x - 3) >= 0 && Grid[x - 3, y] == 0)
                AvailablePoints.Add(new GridPoint(x - 3, y));

            //+0,-3
            if ((y - 3) >= 0 && Grid[x, y - 3] == 0)
                AvailablePoints.Add(new GridPoint(x, y - 3));

            //+2,+2
            if ((x + 2) <= (Size - 1) && (y + 2) <= (Size - 1) && Grid[x + 2, y + 2] == 0)
                AvailablePoints.Add(new GridPoint(x + 2, y + 2));

            //-2,-2
            if ((x - 2) >= 0 && (y - 2) >= 0 && Grid[x - 2, y - 2] == 0)
                AvailablePoints.Add(new GridPoint(x - 2, y - 2));

            //+2,-2
            if ((x + 2) <= (Size - 1) && (y - 2) >= 0 && Grid[x + 2, y - 2] == 0)
                AvailablePoints.Add(new GridPoint(x + 2, y - 2));

            //-2,+2
            if ((x - 2) >= 0 && (y + 2) <= (Size - 1) && Grid[x - 2, y + 2] == 0)
                AvailablePoints.Add(new GridPoint(x - 2, y + 2));

            return AvailablePoints;
        }
    }
}

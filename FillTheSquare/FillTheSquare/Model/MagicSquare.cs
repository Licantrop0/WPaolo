using System;
using System.Collections.Generic;
using System.Windows;

namespace FillTheSquare
{
    public class MagicSquare
    {
        /// <summary>
        /// Rappresentazione della Griglia
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
            PositionHistory = new Stack<GridPoint>(points);
        }

        public bool? PressButton(GridPoint p)
        {
            if (this.IsEmpty == false) //ho già fatto la prima mossa
            {
                var LastPoint = PositionHistory.Peek();

                //voglio cancellare l'ultima mossa
                if (p == LastPoint)
                {
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
            return true; //tutto ok
        }

        public void Clear()
        {
            PositionHistory.Clear();
        }

        public List<GridPoint> GetAvailableMoves()
        {
            var AvailablePoints = new List<GridPoint>();

            if (this.IsEmpty)
                return AvailablePoints;
 
            int x = PositionHistory.Peek().X;
            int y = PositionHistory.Peek().Y;

            GridPoint EvaluatedPoint;
            bool SquareOccupied;

            //+3, +0
            EvaluatedPoint = new GridPoint(x + 3, y);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((x + 3) <= (Size - 1) && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            //+0, +3
            EvaluatedPoint = new GridPoint(x, y + 3);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((y + 3) <= (Size - 1) && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            //-3, +0
            EvaluatedPoint = new GridPoint(x - 3, y);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((x - 3) >= 0 && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            //+0, -3
            EvaluatedPoint = new GridPoint(x, y - 3);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((y - 3) >= 0 && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            //+2, +2
            EvaluatedPoint = new GridPoint(x + 2, y + 2);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((x + 2) <= (Size - 1) && (y + 2) <= (Size - 1) && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            //-2, -2
            EvaluatedPoint = new GridPoint(x - 2, y - 2);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((x - 2) >= 0 && (y - 2) >= 0 && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            //+2, -2
            EvaluatedPoint = new GridPoint(x + 2, y - 2);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((x + 2) <= (Size - 1) && (y - 2) >= 0 && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            //-2, +2
            EvaluatedPoint = new GridPoint(x - 2, y + 2);
            SquareOccupied = PositionHistory.Contains(EvaluatedPoint);
            if ((x - 2) >= 0 && (y + 2) <= (Size - 1) && !SquareOccupied)
                AvailablePoints.Add(EvaluatedPoint);

            return AvailablePoints;
        }
    }
}

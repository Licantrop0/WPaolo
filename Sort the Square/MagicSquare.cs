using System;
using System.Collections.Generic;
using System.Linq;

namespace SortTheSquare
{
    public class MagicSquare
    {
        // array bidimensionale che rappresenta il quadrato magico
        public int[,] Grid;

        /// <summary>
        /// Dimensione della Griglia
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Verifica se il giocatore ha vinto mettendo in ordine tutti i numeri
        /// </summary>
        /// <returns>True se completo, False altrimenti</returns>
        public bool IsCompleted()
        {
            int counter = 1;
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (Grid[j, i] != counter++)
                        return counter == (Size * Size) + 1;

            return true;
        }

        public MagicSquare(int size)
        {
            Size = size;
            Grid = new int[Size, Size];
            InitializeRandomSequence();
        }

        //TODO: se si fa completamente random, potrebbe uscire fuori un quadrato non risolvibie
        private void InitializeRandomSequence()
        {
            Random rnd = new Random();
            var unordered = new Queue<int>(
                Enumerable.Range(0, Size * Size)
                .OrderBy(n => rnd.Next()));

            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Grid[i, j] = unordered.Dequeue();
        }

        private GridPoint? SetN(GridPoint p)
        {
            if (p.Y == 0)
                return null;
            if (Grid[p.X, p.Y - 1] == 0)
            {
                Grid[p.X, p.Y - 1] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return new GridPoint(p.X, p.Y - 1);
            }
            return null;
        }
        private GridPoint? SetS(GridPoint p)
        {
            if (p.Y == (Size - 1))
                return null;
            if (Grid[p.X, p.Y + 1] == 0)
            {
                Grid[p.X, p.Y + 1] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return new GridPoint(p.X, p.Y + 1);
            }
            return null;
        }
        private GridPoint? SetW(GridPoint p)
        {
            if (p.X == 0)
                return null;
            if (Grid[p.X - 1, p.Y] == 0)
            {
                Grid[p.X - 1, p.Y] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return new GridPoint(p.X - 1, p.Y);
            }
            return null;
        }
        private GridPoint? SetE(GridPoint p)
        {
            if (p.X == (Size - 1))
                return null;
            if (Grid[p.X + 1, p.Y] == 0)
            {
                Grid[p.X + 1, p.Y] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return new GridPoint(p.X + 1, p.Y);
            }
            return null;
        }

        public GridPoint? PressButton(GridPoint p)
        {
            return SetN(p) ?? SetS(p) ?? SetW(p) ?? SetE(p);

            //if (Grid[p.X, p.Y] == 0)  //è la casella vuota
            //    return false;
            //if (!SetN(p))
            //    if (!SetS(p))
            //        if (!SetW(p))
            //            if (!SetE(p))
            //                return false;
            //return true;
        }
    }

    public struct GridPoint
    {
        public int X;
        public int Y;

        public GridPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static bool operator ==(GridPoint p1, GridPoint p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(GridPoint p1, GridPoint p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GridPoint)) return false;
            return this == (GridPoint)obj;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace FillTheSquare
{
    public class MagicSquare
    {
        // array bidimensionale che rappresenta il quadrato magico
        public int[,] Grid;

        // Dimensione della Griglia
        public int Size { get; private set; }

        //metodo che verifica se il giocatore ha vinto mettendo in ordine tutti i numeri
        public bool IsCompleted()
        {
            int counter = 1;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Grid[j, i] != counter)
                    {
                        if (i != (Size-1) && j != (Size-1))
                            return false;
                    }
                    counter++;
                }        
            }
            return true;
        }

        public MagicSquare(int size)
        {
            if (size != 5 && size != 10)
                throw new ArgumentException("The square could be only 5x5 or 10x10", "size");

            Size = size;
            Grid = new int[Size, Size];
            InitializeRandomSequence();
        }

        public void InitializeRandomSequence()
        {
            Random rnd = new Random();
            int[] Numbers = new int[Size*Size];
            for(int i = 0; i < Size*Size; i++)
                Numbers[i] = i;

            var unordered = Numbers.OrderBy(n => rnd.Next()).ToArray();

            int counter = 0;
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                {
                    Grid[i, j] = (int)unordered[counter];
                    counter++;
                }
        }

        public bool SetS(GridPoint p)
        {
            if (p.Y == 0)
                return false;
            if (Grid[p.X, p.Y - 1] == 0)
            {
                Grid[p.X, p.Y - 1] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return true;
            }
            return false;
        }
        public bool SetN(GridPoint p)
        {
            if (p.Y == (Size - 1))
                return false;
            if (Grid[p.X, p.Y + 1] == 0)
            {
                Grid[p.X, p.Y + 1] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return true;
            }
            return false;
        }
        public bool SetE(GridPoint p)
        {
            if (p.X == 0)
                return false;
            if (Grid[p.X - 1, p.Y] == 0)
            {
                Grid[p.X - 1, p.Y] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return true;
            }
            return false;
        }
        public bool SetW(GridPoint p)
        {
            if (p.X == (Size - 1))
                return false;
            if (Grid[p.X + 1, p.Y] == 0)
            {
                Grid[p.X + 1, p.Y] = Grid[p.X, p.Y];
                Grid[p.X, p.Y] = 0;
                return true;
            }
            return false;
        }
       

        public bool PressButton(GridPoint p)
        {
            if(Grid[p.X,p.Y] == 0)  //è la casella vuota
                return false;

            if (!SetN(p))
                if (!SetS(p))
                    if (!SetW(p))
                        if (!SetE(p))
                            return false;
            return true;
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

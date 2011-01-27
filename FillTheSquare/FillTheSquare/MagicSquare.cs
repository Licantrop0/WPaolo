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
        private uint[,] grid;

        /// <summary>
        /// tiene traccia di tutti gli spostamenti
        /// </summary>
        private Point[] PositionHistory; //questo è uno Stack<Point>! perchè hai usato gli array?

        /// <summary>
        /// indica la coordinata dell'ultimo numero inserito
        /// </summary>
        public Point LastPosition { get; set; }

        /// <summary>
        /// ultimo numero inserito
        /// </summary>
        public uint LastValue { get; set; }


        public PointValue pv { get; set; }

        public int ActualSize { get; private set; }

        public MagicSquare(int size)
        {
            if (size != 5 && size != 10)
                throw new ArgumentException("The square could be only 5x5 or 10x10", "size");

            ActualSize = size;

            grid = new uint[ActualSize, ActualSize];

            LastValue = 0;
            LastPosition = new Point(-1, -1);
            int histLen = ActualSize ^ 2;
            PositionHistory = new Point[histLen];
            for (int i = 0; i < (histLen); i++)
            {
                PositionHistory[i] = new Point(-1, -1);
            }
        }

        public bool PressButton(Point p)
        {
            if (p == LastPosition)    //voglio cancellare l'ultima mossa
            {
                LastValue--;
                if (LastValue > 0)
                {
                    LastPosition = PositionHistory[LastValue - 1];
                }
                else
                {
                    LastPosition = new Point(-1, -1);
                }
                PositionHistory[LastValue] = new Point(-1, -1);
                grid[(int)(p.X), (int)(p.Y)] = 0;
                return true;
            }
            else
            {
                if (LastPosition.X != -1 && LastPosition.Y != -1)   //se ho già fatto la prima mossa
                {
                    if (((p.X == LastPosition.X - 3) && (p.Y == LastPosition.Y)) ||
                        ((p.X == LastPosition.X - 2) && (p.Y == LastPosition.Y - 2)) ||
                        ((p.X == LastPosition.X) && (p.Y == LastPosition.Y - 3)) ||
                        ((p.X == LastPosition.X + 2) && (p.Y == LastPosition.Y - 2)) ||
                        ((p.X == LastPosition.X + 3) && (p.Y == LastPosition.Y)) ||
                        ((p.X == LastPosition.X + 2) && (p.Y == LastPosition.Y + 2)) ||
                        ((p.X == LastPosition.X) && (p.Y == LastPosition.Y + 3)) ||
                        ((p.X == LastPosition.X - 2) && (p.Y == LastPosition.Y + 2)))
                    {
                        for (int i = 0; i < PositionHistory.Length; i++)
                        {
                            if (p.Equals(PositionHistory[i]))
                            {
                                return false;   //la casella in questione è già occupata
                            }
                        }
                    }
                    else
                    {
                        return false;   //non ho rispettato le regole
                    }
                }
                LastPosition = p;
                PositionHistory[LastValue] = p;
                LastValue++;
                grid[(int)(p.X), (int)(p.Y)] = LastValue;
                return true;
            }
        }
    }


    public class PointValue
    {
        public byte x { get; set; }
        public byte y { get; set; }
        public byte value { get; set; }
    }
}

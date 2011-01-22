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

namespace FillTheSquare
{
    //uso questo enum perchè i quadrati magici per funzionare possono avere solo lati lunghi multipli di cinque
    public enum SquareSize
    {
        Five,
        Ten
    }

    public class MagicSquare
    {
        private uint[,] grid;                           //array bidimensionale che rappresenta il quadrato magico
        private Point[] actualPositionHistory;          //tiene traccia di tutti gli spostamenti
        public Point actualPosition {get; set;}        //indica la coordinata dell'ultimo numero inserito
        public uint actualValue {get; set;}             //ultimo numero inserito

        public MagicSquare(SquareSize Size)
        {
            int size = 0;
            switch(Size)
            {
                case SquareSize.Ten:
                    size = 10;
                    break;
                default:    //lato di cinque
                    size = 5;
                    break;
            }
            grid = new uint[size, size];
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    grid[i,j] = 0;
                }
            }
            actualValue = 0;
            actualPosition = new Point(-1, -1);
            int histLen = size*size;
            actualPositionHistory = new Point[histLen];
            for(int i = 0; i < (histLen); i++)
            {
                actualPositionHistory[i] = new Point(-1,-1);
            }
        }

        public bool PressButton(Point p)
        {
            if (p == actualPosition)    //voglio cancellare l'ultima mossa
            {
                actualValue--;
                if (actualValue > 0)
                {
                    actualPosition = actualPositionHistory[actualValue - 1];
                }
                else
                {
                    actualPosition = new Point(-1, -1);
                }
                actualPositionHistory[actualValue] = new Point(-1, -1);
                grid[(int)(p.X), (int)(p.Y)] = 0;
                return true;
            }
            else
            {
                if (actualPosition.X != -1 && actualPosition.Y != -1)   //se ho già fatto la prima mossa
                {
                    if (((p.X == actualPosition.X - 3) && (p.Y == actualPosition.Y)) ||
                        ((p.X == actualPosition.X - 2) && (p.Y == actualPosition.Y - 2)) ||
                        ((p.X == actualPosition.X) && (p.Y == actualPosition.Y - 3)) ||
                        ((p.X == actualPosition.X + 2) && (p.Y == actualPosition.Y - 2)) ||
                        ((p.X == actualPosition.X + 3) && (p.Y == actualPosition.Y)) ||
                        ((p.X == actualPosition.X + 2) && (p.Y == actualPosition.Y + 2)) ||
                        ((p.X == actualPosition.X) && (p.Y == actualPosition.Y + 3)) ||
                        ((p.X == actualPosition.X - 2) && (p.Y == actualPosition.Y + 2)))
                    {
                        for (int i = 0; i < actualPositionHistory.Length; i++)
                        {
                            if (p.Equals(actualPositionHistory[i]))
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
                actualPosition = p;
                actualPositionHistory[actualValue] = p;
                actualValue++;
                grid[(int)(p.X), (int)(p.Y)] = actualValue;
                return true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardMario
{
    public enum PillowVerse
    {
        orizzontal,
        vertical
    }

    public enum EmptySpaceVerse
    {
        up,
        left,
        right
    }

    public class Pillow
    {
        #region private members

        Stone _first;

        Stone _second;

        PillowVerse _verse;

        EmptySpaceVerse _emptySpaceVerse;

        #endregion

        #region constructor

        public Pillow(ElementColor leftColor, ElementColor rightColor)
        {
            // stone are created out of the board, put into incoming pillows, then they are put on board
            _first = new Stone(leftColor);
            _second = new Stone(rightColor);

            // link the stone together
            _first.LinkedTo = _second;
            _second.LinkedTo = _first;

            _verse = PillowVerse.orizzontal;
            _emptySpaceVerse = EmptySpaceVerse.up;
        }

        #endregion

        #region properties

        public Stone FirstStone 
        {
            get { return _first; }
            set { _first = value; }
        }

        public Stone SecondStone
        {
            get { return _second; }
            set { _second = value; }
        }

        public PillowVerse Verse
        {
            get { return _verse;  }
            set { _verse = value; }
        }

        public EmptySpaceVerse EmptySpaceVerse
        {
            get { return _emptySpaceVerse; }
            set { _emptySpaceVerse = value; }
        }

        #endregion
    }
}

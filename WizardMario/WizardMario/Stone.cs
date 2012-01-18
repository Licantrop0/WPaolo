using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardMario
{
    public enum ElementColor
    {
        none,
        red,
        yellow,
        blue
    }

    public class Stone : IWizardMarioElement
    {
        #region private members

        public static Sprite[] SpritePrototypes = new Sprite[3];

        Sprite _stoneSprite;

        ElementColor _color;

        Stone _linkedTo;

        bool _floating = false;

        // maybe row and column aren't necessary...
        int _row;

        int _col;

        #endregion

        #region constructor

        public Stone(ElementColor color)
        {
            _color = color;

            _stoneSprite = SpritePrototypes[(int)color].Clone();
        }

        #endregion

        #region properties

        public Stone LinkedTo 
        {
            get { return _linkedTo; }
            set { _linkedTo = value; }
        }

        public int Row 
        {
            get { return _row; }
            set { _row = value; }
        }

        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }

        #endregion

        #region interface implementation

        public ElementColor Color
        {
            get { return _color; }
        }

        public bool Floating
        {
            get { return _floating; }

            set { _floating = value; }
        }
       
        public void Destroy()
        {
            // disrupt link between stones

        }

        #endregion
    }
}

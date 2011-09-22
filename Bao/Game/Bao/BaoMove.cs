using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bao
{
    public class BaoMove
    {
    # region public members

        public Byte rowBox;
        public Byte columnBox;
        public bool left;
        public bool playNyumba;
        public bool nyumbaTax;

    # endregion

    # region costructors

        public BaoMove()
        {
            rowBox = 0;
            columnBox = 0;
            left = false;
            playNyumba = false;
            nyumbaTax = false;
        }

        public BaoMove(Byte rB, Byte cB, bool d, bool pn = false, bool nt = false)
        {
            rowBox = rB;
            columnBox = cB;
            left = d;
            playNyumba = pn;
            nyumbaTax = nt;
        }

        public BaoMove DeepCopy()
        {
            return new BaoMove(rowBox, columnBox, left, playNyumba);
        }

    # endregion

    #region public methods
        
        public bool Equal(BaoMove rightWise)
        {
            if (ShimoEqual(rightWise) && left == rightWise.left)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ShimoEqual(BaoMove rightWise)
        {
            if (rowBox == rightWise.rowBox && columnBox == rightWise.columnBox)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

    #endregion
    }
}

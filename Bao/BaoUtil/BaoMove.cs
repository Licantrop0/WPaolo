using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaoUtil
{
    public class BaoMove
    {
    # region public members

        public Byte rowBox;
        public Byte columnBox;
        public bool left;

    # endregion

    # region costructors

        public BaoMove()
        {
            rowBox = 10;
            columnBox = 10;
            left = false;
        }

        public BaoMove(Byte rB, Byte cB, bool c)
        {
            Set(rB, cB, c);
        }

        public BaoMove DeepCopy()
        {
            return new BaoMove(rowBox, columnBox, left);
        }

    # endregion

    #region public methods

        public void Set(Byte rB, Byte cB, bool c)
        {
            rowBox = rB;
            columnBox = cB;
            left = c;
        }

        public static bool operator == (BaoMove leftWise, BaoMove rightWise)
        {
            if (leftWise.rowBox == rightWise.rowBox &&
                leftWise.columnBox == rightWise.columnBox &&
                    leftWise.left == rightWise.left)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator != (BaoMove leftWise, BaoMove rightWise)
        {
            if (leftWise.rowBox != rightWise.rowBox ||
                leftWise.columnBox != rightWise.columnBox ||
                    leftWise.left != rightWise.left)
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

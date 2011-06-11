using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public interface ITouchable
    {
        bool Touched(Vector2 fingerPosition);
        bool Touchable();
    }
}

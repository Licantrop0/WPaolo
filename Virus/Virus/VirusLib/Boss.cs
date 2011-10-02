using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VirusLib
{
    public abstract class Boss : TimingBehaviouralBody
    {
        public bool SpecialMoveHit { get; set; }

        public abstract bool Died { get; }

        public abstract void HandleUserTouch(Vector2 touchPoint, ref int enemiesHit);

        public Boss(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public abstract class Boss : TimingBehaviouralBody
    {
        public bool SpecialMoveHit { get; set; }

        public abstract void HandleUserTouch(Vector2 touchPoint, ref int enemiesHit);

        public Boss(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {

        }
    }
}

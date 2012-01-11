using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public abstract class Boss : TimingBehaviouralBody
    {
        public abstract bool Died { get; }

        public abstract void HandleUserTap(Vector2 touchPoint, ref bool hit);

        public int WorthPoints { get; set; }

        protected Virus _virus;  // reference to virus

        public Boss(DynamicSystem dynamicSystem, Sprite sprite, Shape shape, Virus virus)
            :base(dynamicSystem, sprite, shape)
        {
            _virus = virus;
        }
    }
}

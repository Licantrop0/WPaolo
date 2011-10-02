using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirusLib
{
    public abstract class Enemy : TimingBehaviouralBody
    {
        public abstract bool Moving { get; }
        public abstract bool Died { get; }

        public Enemy(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {

        }
    }
}

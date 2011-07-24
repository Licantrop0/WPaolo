using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Kiwi
{
    public class CircularSprite : Sprite, ITouchable
    {
        protected bool _touchable;
        protected float _radius;        // va incapsulato in una classe per gestire le collisioni
        protected float _touchRadius;

        public CircularSprite(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations)
        {
            _radius = radius;
            _touchRadius = touchRadius;
        }

        public float Radius
        { get { return _radius; } set { _radius = value; } }

        // ITouchable implementation
        public bool Touched(Vector2 fingerPosition)
        {
            return (Touchable() && Vector2.Distance(Position, fingerPosition) < _touchRadius );
        }

        public bool Touchable()
        {
            return _touchable;
        }
    }
}

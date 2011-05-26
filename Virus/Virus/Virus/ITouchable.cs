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

    /*public interface ICircularTouchShape
    {
        private float _touchRadius;
        Vector2 _origin;
        
        public float TouchRadius
        { get { return _touchRadius; } set { _touchRadius = value; } }

        public Vector2 Origin
        { get { return _origin; } set { _origin = Origin} }

        ICircularTouchShape(Vector2 origin, float touchRadius)
        {
            _origin = origin;
            _touchRadius = touchRadius;
        }

        public bool Touched(Vector2 fingerPosition)
        {
            return (Vector2.Distance(_origin, fingerPosition) <= _touchRadius);
        }
    }*/
}

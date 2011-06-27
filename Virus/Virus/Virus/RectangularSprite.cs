using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class RectangularSprite : Sprite, ITouchable
    {
        protected bool _touchable;
        protected float _width;        // vanno incapsulate in una classe per gestire le collisioni
        protected float _height;       // vanno incapsulate in una classe per gestire le collisioni
        protected float _touchWidth;
        protected float _touchHeight;

        public RectangularSprite(Dictionary<string, Animation> animations, float width, float height, float touchWidth, float touchHeight)
            : base(animations)
        {
            _width = width;
            _height = height;
            _touchWidth = touchWidth;
            _touchHeight = touchHeight;
        }

        public float Width
        { get { return _width; } set { _width = value; } }

        public float Height
        { get { return _height; } set { _height = value; } }

        // ITouchable implementation
        public bool Touched(Vector2 fingerPosition)
        {
            return (Touchable() && 
                ( 
                fingerPosition.Y >= Position.Y - _touchHeight / 2 &&
                fingerPosition.Y <= Position.Y + _touchHeight / 2 &&
                fingerPosition.X >= Position.X - _touchWidth  / 2 &&
                fingerPosition.X <= Position.X + _touchWidth  / 2
                ) );
        }

        public bool Touchable()
        {
            return _touchable;
        }
    }
}

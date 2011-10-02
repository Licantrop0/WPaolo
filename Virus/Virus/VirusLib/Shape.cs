using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VirusLib
{
    public enum ShapeType
    {
        circular,
        rectangular
    }

    public abstract class Shape
    {
        public abstract bool Touched(Vector2 shapePosition, float shapeAngle, Vector2 fingerPosition);
        public abstract Vector2 GetShapeSize();
        public abstract ShapeType GetShapeType();
    }

    public class CircularShape : Shape
    {
        private float _radius;
        private float _touchRadius;

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        protected float TouchRadius
        {
            get { return _touchRadius; }
            set { _touchRadius = value; }
        }
        
        public CircularShape(float radius, float touchRadius)
        {
            _radius = radius;
            _touchRadius = touchRadius;
        }

        public override bool Touched(Vector2 shapePosition, float shapeAngle, Vector2 fingerPosition)
        {
            return (Vector2.Distance(shapePosition, fingerPosition) < _touchRadius);
        }

        public override Vector2 GetShapeSize()
        {
            return new Vector2(_radius, _radius);
        }

        public override ShapeType GetShapeType()
        {
            return ShapeType.circular;
        }
    }

    public class RectangularShape : Shape
    {
        private float _width;        // vanno incapsulate in una classe per gestire le collisioni
        private float _height;       // vanno incapsulate in una classe per gestire le collisioni
        private float _touchWidth;
        private float _touchHeight;

        protected float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        protected float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        protected float TouchWidth
        {
            get { return _touchWidth; }
            set { _touchWidth = value; }
        }

        protected float TouchHeight
        {
            get { return _touchHeight; }
            set { _touchHeight = value; }
        }

        public RectangularShape(float width, float height, float touchWidth, float touchHeight)
        {
            _width = width;
            _height = height;
            _touchWidth = touchWidth;
            _touchHeight = touchHeight;
        }

        public override bool Touched(Vector2 shapePosition, float shapeAngle, Vector2 fingerPosition)
        {
            // per adesso lo calcolo con angolo nullo
            return
                (
                fingerPosition.Y >= shapePosition.Y - TouchHeight / 2 &&
                fingerPosition.Y <= shapePosition.Y + TouchHeight / 2 &&
                fingerPosition.X >= shapePosition.X - TouchWidth / 2 &&
                fingerPosition.X <= shapePosition.X + TouchWidth / 2
                );

            /*Vector2 joining = fingerPosition - shapePosition;

            // precalculating variables
            float sinTheta = (float)Math.Sin(shapeAngle);
            float cosTheta = (float)Math.Cos(shapeAngle);

            // rectangle width vector
            Vector2 rectangleWidthVector = new Vector2(cosTheta * Width, sinTheta * Height);

            // rectangle height vector
            Vector2 rectangleHeightVector = new Vector2(- sinTheta * Height, cosTheta * Height);
            
            // joining component along rectangle width must be less than width*/
        }

        public override Vector2 GetShapeSize()
        {
            return new Vector2(_width, _height);
        }

        public override ShapeType GetShapeType()
        {
            return ShapeType.rectangular;
        }
    }
}

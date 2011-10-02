using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Virus
{
    public abstract class Body : ITouchable
    {
        #region injected private members

        // physics
        private DynamicSystem _dynamicSystem;

        // graphics 
        private Sprite _sprite;

        // shape (contacts with other shapes and with touch)
        private Shape _shape;

        #endregion

        #region private members

        // sizing
        private Vector2 _originalSize;
        private Vector2 _size;
        private Vector2 _resizingSpeed = Vector2.Zero;

        // body life time
        private float _lifeTime;

        // body events
        private Queue<BodyEvent> _bodyEventQueque = new Queue<BodyEvent>();

        #endregion

        #region protected members

        // elapsed time
        protected float _elapsedTime;

        // current body input
        protected BodyEvent _actBodyEvent;

        #endregion

        #region properties

        // injected dependencies
        public Shape Shape
        {
            get { return _shape; }
        }

        protected Sprite Sprite
        {
            get { return _sprite; }
        }

        protected DynamicSystem DynamicSystem
        {
            get { return _dynamicSystem; }
        }

        #endregion

        public Body(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
        {
            _dynamicSystem = dynamicSystem;
            _sprite = sprite;
            _shape = shape;

            _size = _originalSize = _shape.GetShapeSize();
        }
        
        public float LifeTime
        { 
            get { return _lifeTime; }
        }

        public Vector2 ResizingSpeed
        {
            get { return _resizingSpeed; }
            set { _resizingSpeed = value; }
        }

        public Vector2 Position
        { 
            get { return _dynamicSystem.Position; }
            set {
                    _dynamicSystem.Position = value;
                    _sprite.Position = value;
                }
        }

        public float Angle
        {
            get { return _dynamicSystem.Angle; }
            set { 
                  _dynamicSystem.Angle = value;
                  _sprite.Angle = value;
                }
        }

        public Vector2 Speed
        { 
            get { return _dynamicSystem.Speed; }
            set { _dynamicSystem.Speed = value; }
        }

        public float AngularSpeed
        {
            get { return _dynamicSystem.AngularSpeed; }
            set { _dynamicSystem.AngularSpeed = value; }
        }

        public void Rotate()
        {
            _dynamicSystem.Rotate(_elapsedTime);
            _sprite.Angle = _dynamicSystem.Angle;
        }

        public void Move()
        {
            _dynamicSystem.Move(_elapsedTime);
            _sprite.Position = _dynamicSystem.Position;
            _sprite.Angle = _dynamicSystem.Angle;
        }

        public void Traslate()
        {
            _dynamicSystem.Traslate(_elapsedTime);
            _sprite.Position = _dynamicSystem.Position;
        }

        public void Resize()
        {
            _size = _size + _resizingSpeed * _elapsedTime;

            Vector2 scale = _size / _originalSize;

            if (scale.X < 0)
                scale.X = 0;
            if (scale.Y < 0)
                scale.Y = 0;

            _sprite.Scale = scale;
        }

        protected void Bounce(Vector2 normal)
        {
            Speed = Vector2.Reflect(Speed, normal);
        }

        public void AddBodyEvent(BodyEvent evnt)
        {
            _bodyEventQueque.Enqueue(evnt);
        }

        public virtual void Update(GameTime gameTime)
        {
            _elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _sprite.Tick(_elapsedTime);
            _actBodyEvent = _bodyEventQueque.Count > 0 ? _bodyEventQueque.Dequeue() : null;
            _lifeTime += _elapsedTime;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch);
        }

        protected void Animate()
        {
            _sprite.Animate();
        }

        public bool Touched(Vector2 fingerPosition)
        {
            return Touchable && _shape.Touched(Position, Angle, fingerPosition);
        }

        public bool Touchable { get; set; }
    }
}

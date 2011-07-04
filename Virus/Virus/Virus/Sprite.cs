using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Virus
{
    public class SpriteEvent
    {
        public SpriteEventCode Code { get; set;}
        public Object[] Params { get; set; }

        public SpriteEvent(SpriteEventCode code)
        {
            Code = code;
        }

        public SpriteEvent(SpriteEventCode code, Object[] p)
        {
            Code = code;
            Params = p;
        }
    }

    public abstract class Sprite
    {
        // graphics
        private Dictionary<string, Animation> _animations;
        private string _currentAnimation;
        private Queue<SpriteEvent> _spriteEventQueque = new Queue<SpriteEvent>();
        protected float _elapsedTime;
        protected SpriteEvent _actSpriteEvent;
        protected float _lifeTime;

        // from animations
        private Color _tint;
        private float _angle;
        private float _scale;

        protected float _rotationSpeed;
        protected float _scalingSpeed;

        protected float _fadeSpeed;

        private float _blinkingPeriod;
        private float _blikingTimeElapsed;
        private Color _blinkingTint;

        public float LifeTime
        { get { return _lifeTime; } }

        // from animations properties
        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Color Tint
        {
            get { return _tint; }
            set { _tint = value; }
        }

        public float FramePerSecond
        { set { _animations[_currentAnimation].TimeToUpdate = value != 0 ? (1f / value) : float.PositiveInfinity; } }

        public float RotationSpeed
        { set { _rotationSpeed = value; } }

        public float ScalingSpeed
        { set { _scalingSpeed = value; } }

        public float FadeSpeed
        { set { _fadeSpeed = value; } }

        public float BlinkingFrequency
        { set { _blinkingPeriod = 1f / value; } }

        public Color BlinkingTint
        { set { _blinkingTint = value; } }

        public void Animate()
        {
            _animations[_currentAnimation].Animate(_elapsedTime);
        }

        public void ChangeAnimation(string animationKey)
        {
            _currentAnimation = animationKey;
        }

        public void Rotate()
        {
            _angle = _angle + _rotationSpeed * _elapsedTime;
        }

        public void ChangeDimension()
        {
            _scale = _scale + _scalingSpeed * _elapsedTime;
            if (_scale < 0)
                _scale = 0;
        }

        public void Fade()
        {
            float alpha = _tint.A - _fadeSpeed * _elapsedTime * 255;
            if (alpha < 0)
                alpha = 0;

            _tint.A = (byte)alpha;
        }

        public void Blink()
        {
            _blikingTimeElapsed += _elapsedTime;

            if (_blikingTimeElapsed > _blinkingPeriod)
            {
                _blikingTimeElapsed -= _blinkingPeriod;
                _tint = _tint == Color.White ? _blinkingTint : Color.White;
            }
        }

        protected void Bounce(Vector2 normal)
        {
            Speed = Vector2.Reflect(Speed, normal);
        }

        // physics
        protected PhysicalKinematicPoint _physicalPoint; 

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _currentAnimation = _animations.Keys.First();
            _tint = Color.White;
            Scale = 1;

            InitializePhysics();

            if (_physicalPoint == null)
            {
                throw new Exception("Porca troia, ma inizializza sta cazzo di fisica, no?");
            }
        }

        public Vector2 Position
        { get { return _physicalPoint.Position; } set { _physicalPoint.Position = value; } }

        public Vector2 Speed
        { get { return _physicalPoint.Speed; } set { _physicalPoint.Speed = value; } }

        public void Move()
        {
            _physicalPoint.Move(_elapsedTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _animations[_currentAnimation].Draw(spriteBatch, this);
        }

        public void AddSpriteEvent(SpriteEvent evnt)
        {
            _spriteEventQueque.Enqueue(evnt);
        }

        public virtual void Update(GameTime gameTime)
        {
            _elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _actSpriteEvent = _spriteEventQueque.Count > 0 ? _spriteEventQueque.Dequeue() : null;
            _lifeTime += _elapsedTime;
        }

        protected virtual void InitializePhysics()
        {
            
        }
    }
}
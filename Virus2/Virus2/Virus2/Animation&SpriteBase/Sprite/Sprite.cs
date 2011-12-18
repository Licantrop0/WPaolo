using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Virus
{
    public class Sprite
    {
        #region private members

        // Animation container and proxy
        private Dictionary<string, Animation> _animations;
        private Animation _currentAnimation;
        protected float _elapsedTime;

        // sprite batch draw parameters (the others are embedded in properties)
        private Color _tint;

        // fading speed
        protected float _fadeSpeed;

        #endregion

        #region properties

        // sprite batch draw parameters
        public Vector2 Position { get; set; }

        public float Angle { get; set; }

        public Vector2 Scale { get; set; }

        public Color Tint
        {
            get { return _tint; }
            set { _tint = value; }
        }

        // fade speed
        public float FadeSpeed
        {
            get { return _fadeSpeed; }
            set { _fadeSpeed = value; }
        }

        // animation parameters
        public float FramePerSecond
        {
            get { return 1 / _currentAnimation.TimeToUpdate; }
            set { _currentAnimation.TimeToUpdate = value != 0 ? (1f / value) : float.PositiveInfinity; }
        }

        public int AnimationFrames
        {
            get { return _currentAnimation.FramesNum; }
        }

        public bool AnimationVerse
        {
            get { return !_currentAnimation.Reverse; }
            set { _currentAnimation.Reverse = !value; }
        }

        #endregion

        #region constructors

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _currentAnimation = _animations[_animations.Keys.First()];
            _tint = Color.White;
            Scale = Vector2.One;
        }

        public Sprite Clone()
        {
            Dictionary<string, Animation> animationDictionary = new Dictionary<string,Animation>();
            foreach (var animation in _animations)
            {
                animationDictionary.Add(animation.Key, animation.Value);
            }

            return new Sprite(animationDictionary);
        }

        #endregion

        #region public methods

        public void ChangeAnimation(string animationKey)
        {
            _currentAnimation.Reset();
            _currentAnimation = _animations[animationKey];
            _currentAnimation.Reset();
        }

        public void DelayAnimation()
        {
            _currentAnimation.Delay();
        }

        public void ReverseAnimation()
        {
            if (!_currentAnimation.Reverse)
            {
                _currentAnimation.Reverse = true;
            }
            else
            {
                _currentAnimation.Reverse = false;
            }
        }

        public void Fade()
        {
            float alpha = _tint.A - _fadeSpeed * _elapsedTime * 255;
            if (alpha < 0)
                alpha = 0;

            _tint.A = (byte)alpha;
        }

        public bool AnimationFinished()
        {
            return _currentAnimation.Finished;
        }

        public bool IsAnimationBegin()
        {
            if (_currentAnimation.FrameIndex == 0)
                return true;
            else
                return false;
        }

        public int FrameIndex()
        {
            return _currentAnimation.FrameIndex;
        }

        public virtual void Tick(float elapsedTime)
        {
            _elapsedTime = elapsedTime;
        }

        public void Animate()
        {
            _currentAnimation.Animate(_elapsedTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _currentAnimation.Draw(spriteBatch, this);
        }

        #endregion
    }
}
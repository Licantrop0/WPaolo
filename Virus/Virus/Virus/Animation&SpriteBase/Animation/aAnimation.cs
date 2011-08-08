using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    // i metodi e le proprietà pubbliche dovranno diventare interne nel momento in cui la classe animation
    // verrà incapsulata in un assembly con la classe Sprite.
    // tali proprietà e metodi infatti dovranno essere accessibili solo alle classi Animaton e Sprite

    public abstract class Animation
    {
        #region private members

        // state variables
        private float _timeElapsed;
        private float _timeToUpdate;

        #endregion

        #region protected members

        // texture source and frames rectangles
        protected Texture2D _sourceTexture;
        protected Rectangle[] _rectangles;

        // frames size 
        protected int _frameWidth;
        protected int _frameHeight;

        // origin offset
        protected Vector2 _origin;

        // state variables
        protected int _frameIndex = 0;
        protected bool _looping;
        protected bool _reverse = false;

        // number of frames
        protected int _frames;

        #endregion

        #region properties

        public float TimeToUpdate
        {
            get { return _timeToUpdate; }
            set { _timeToUpdate = value; }
        }

        public int FramesNum
        {
            get { return _frames; }
        }

        public int FrameIndex
        {
            get { return _frameIndex; }
        }

        public bool Finished
        {
            get
            {
                return !_looping && ((!_reverse && _frameIndex == _rectangles.Length - 1) || (_reverse && _frameIndex == 0));
            }
        }

        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }

        #endregion

        #region constructors

        public Animation(int frames, bool looping)
        {
            _frames = frames;
            _looping = looping;
        }

        #endregion

        #region virtual and abstract methods

        virtual protected void SetSourceTexture()
        {
        }

        virtual public void Reset()
        {
            _frameIndex = 0;
            _timeElapsed = 0;
        }

        #endregion

        #region public methods

        public void Delay()
        {
            _timeElapsed = 0;
        }

        public void Animate(float dt)
        {
            _timeElapsed += dt;

            if (_timeElapsed > _timeToUpdate)
            {
                _timeElapsed -= _timeToUpdate;

                if (!_reverse)
                {
                    if (_frameIndex < _rectangles.Length - 1)
                    {
                        _frameIndex++;
                    }
                    else if (_looping)
                    {
                        _frameIndex = 0;
                    }
                }
                else
                {
                    if (_frameIndex > 0)
                    {
                        _frameIndex--;
                    }
                    else if (_looping)
                    {
                        _frameIndex = _rectangles.Length - 1;
                    }
                }
            }

            SetSourceTexture();
        }

        public void Draw(SpriteBatch spriteBatch, Sprite sprite)
        {
            spriteBatch.Draw(_sourceTexture, sprite.Position, _rectangles[_frameIndex], sprite.Tint, sprite.Angle, _origin, sprite.Scale, SpriteEffects.None, 0f);
        }

        #endregion  
    }
}

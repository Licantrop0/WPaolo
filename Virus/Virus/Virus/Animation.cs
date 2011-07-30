using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public abstract class Animation
    {
        protected Texture2D _sourceTexture;
        protected Rectangle[] _rectangles;
        protected int _frameIndex = 0;

        protected int _frameWidth; 
        protected int _frameHeight;

        protected Vector2 _origin;

        private float _timeElapsed;
        private float _timeToUpdate;
        protected bool _looping;
        protected bool _reverse = false;
        protected int _frames;

        public float TimeToUpdate
        { set { _timeToUpdate = value; } }

        public int FramesNum
        { get { return _frames; } }

        public bool Reverse { get { return _reverse; } set { _reverse = value; } }

        public bool Finished
        {
            get
            {
                return !_looping && ((!_reverse && _frameIndex == _rectangles.Length - 1) || (_reverse && _frameIndex == 0));
            }
        }

        public Animation(int frames, bool looping)
        {
            _frames = frames;
            _looping = looping;
        }
       
        public void Draw(SpriteBatch spriteBatch, Sprite sprite)
        {
            spriteBatch.Draw(_sourceTexture, sprite.Position, _rectangles[_frameIndex], sprite.Tint, sprite.Angle, _origin, sprite.Scale, SpriteEffects.None, 0f);
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

        virtual protected void SetSourceTexture()
        {
        }
    }

    public class SpriteSheetAnimation : Animation
    {
        public SpriteSheetAnimation(int frames, bool looping, Texture2D sourceTexture) 
            : base(frames, looping)
        {
            _sourceTexture = sourceTexture;

            _frameWidth = _sourceTexture.Width / frames;
            _frameHeight = _sourceTexture.Height;

            _rectangles = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
            {
                _rectangles[i] = new Rectangle(i * _frameWidth, 0, _frameWidth, _frameHeight);
            }

            _origin = new Vector2(_frameWidth / 2, _frameHeight / 2);
        }
    }

    public class ScreenAnimation : Animation
    {
        Texture2D[] _textureArray;
        bool _isPortrait;

        public ScreenAnimation(int frames, bool looping, bool isPortrait, Texture2D[] textureArray)
            : base(frames, looping)
        {
            _textureArray = textureArray;
            _sourceTexture = _textureArray[0];
            
            _isPortrait = isPortrait;

            if (_isPortrait)
            {
                _frameWidth = 480;
                _frameHeight = 800;
            }
            else
            {
                _frameWidth = 800;
                _frameHeight = 480;
            }

            int horizontalPeriod = _isPortrait ? 4 : 2;
            int verticalPeriod = _isPortrait ? 2 : 4; 

            _rectangles = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
            {
                _rectangles[i] = new Rectangle(
                    (i % horizontalPeriod) * _frameWidth,
                    (i % verticalPeriod) * _frameHeight,
                    _frameWidth, _frameHeight);
            }

            _origin = new Vector2(_frameWidth / 2, _frameHeight / 2);
        }

        protected override void SetSourceTexture()
        {
            _sourceTexture = _textureArray[(int)Math.Floor(_frameIndex / 8)];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kiwi
{
    public class Animation
    {
        private Texture2D _texture;
        private Rectangle[] _rectangles;
        private int _frameIndex = 0;

        private int _frameWidth { get; set; }
        private int _frameHeight { get; set; }

        private Vector2 _origin;

        private float _timeElapsed;
        private float _timeToUpdate;
        private bool _looping;
        private bool _reverse = false;

        public float TimeToUpdate
        { set { _timeToUpdate = value; } }

        public int NFrames
        { get { return _rectangles.Length; } }

        public bool Reverse { get { return _reverse; } set { _reverse = value; } }

        public bool Finished
        {
            get
            {
                return !_looping && ((!_reverse && _frameIndex == _rectangles.Length - 1) || (_reverse && _frameIndex == 0));
            }
        }

        public Animation(Texture2D texture, int frames, bool looping)
        {
            _texture = texture;

            _frameWidth = _texture.Width / frames;
            _frameHeight = _texture.Height;

            _rectangles = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
            {
                _rectangles[i] = new Rectangle(i * _frameWidth, 0, _frameWidth, _frameHeight);
            }

            _origin = new Vector2(_frameWidth / 2, _frameHeight / 2);
            _looping = looping;
        }

        //TODO va fatto meglio
        public Animation Clone()
        {
            return new Animation(_texture, _rectangles.Length, _looping);
        }

        public void Draw(SpriteBatch spriteBatch, Sprite sprite)
        {
            spriteBatch.Draw(_texture, sprite.Position, _rectangles[_frameIndex], sprite.Tint, sprite.Angle, _origin, sprite.Scale, SpriteEffects.None, 0f);
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
        }
    }

    /*public class ScreenAnimation : Animation
    {
        // class is now designed for vertical screen

    }*/
}

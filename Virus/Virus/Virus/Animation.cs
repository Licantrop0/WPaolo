using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
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

        public float TimeToUpdate
        { set { _timeToUpdate = value; } }


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

                if (_frameIndex < _rectangles.Length - 1)
                {
                    _frameIndex++;
                }
                else if (_looping)
                {
                    _frameIndex = 0;
                }
            }
        }

    }
}

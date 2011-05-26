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

        private Vector2 _position;
        private Color _color;
        private Vector2 _origin;
        private float _rotation;
        private float _scale;
        private SpriteEffects _spriteEffects;

        private float _timeElapsed;
        private float _timeToUpdate;
        private bool _looping;

        private float _rotationSpeed;
        private float _scalingSpeed;

        // fading attributes
        private float _fadeSpeed;

        public Vector2 Position
        { set { _position = value; } }

        public float FramePerSecond
        { set { _timeToUpdate = value != 0 ? (1f / value) : float.PositiveInfinity; } }

        public float RotationSpeed
        { set { _rotationSpeed = value; } }

        public float ScalingSpeed
        { set { _scalingSpeed = value; } }

        public float FadeSpeed
        { set { _fadeSpeed = value; } }

        public Animation(Texture2D texture, int frames)
        {
            _texture = texture;

            _frameWidth = _texture.Width / frames;
            _frameHeight = _texture.Height;

            _rectangles = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
            {
                _rectangles[i] = new Rectangle(i * _frameWidth, 0, _frameWidth, _frameHeight);
            }

            _color = new Color(1f,1f,1f,1f);
            _origin = new Vector2(_frameWidth / 2, _frameHeight / 2);
            _rotation = 0f;
            _scale = 1f;
            _spriteEffects = SpriteEffects.None;

            _looping = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, _rectangles[_frameIndex], _color, _rotation, _origin, _scale, _spriteEffects, 0f);
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

        public void Rotate(float dt)
        {
            _rotation = _rotation + _rotationSpeed * dt;
        }

        public void ChangeDimension(float dt)
        {
            _scale = _scale + _scalingSpeed * dt;
            if (_scale < 0)
                _scale = 0;
        }

        public void Fade(float dt)
        {
            float alpha = _color.A - _fadeSpeed * dt * 255;
            if (alpha < 0)
                alpha = 0;

            _color.A = (byte)alpha;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public abstract class SpriteManager
    {
        public Texture2D _texture;  //PS TEMP!
        protected Rectangle[] _rectangles;
        protected int _frameIndex = 0;

        public int  RectangleWidth { get; set; }
        public int  RectangleHeight { get; set; }

        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public SpriteManager(Texture2D texture, int frames)
        {
            _texture = texture;

            RectangleWidth = _texture.Width / frames;
            RectangleHeight = _texture.Height;

            _rectangles = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
            {
                _rectangles[i] = new Rectangle(i * RectangleWidth, 0, RectangleWidth, RectangleHeight);
            }

            Color = Color.White;
            Rotation = 0f;
            Scale = 1f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, _rectangles[_frameIndex], Color, Rotation, Origin, Scale, SpriteEffect, 0f);
        }
    }

    public class SpriteAnimation : SpriteManager
    {
        private float _timeElapsed;
        private float _timeToUpdate = 0.05f;

        public bool IsLooping { get; set; }
        public float FramesPerSecond
        {
            get { return 1f / _timeToUpdate; }
            set { _timeToUpdate = (1f / value); }
        }

        public void Update(GameTime gameTime)
        {
            _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Rotation += (float)(Math.PI / 16);   //PS temp!

            if (_timeElapsed > _timeToUpdate)
            {
                _timeElapsed -= _timeToUpdate;

                if (_frameIndex < _rectangles.Length - 1)
                {
                    _frameIndex++;
                }
                else if (IsLooping)
                {
                    _frameIndex = 0;
                }
            }
        }

        public SpriteAnimation(Texture2D texture, int frames)
            : base(texture, frames)
        {
            IsLooping = false;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Virus
{
    class MovingBackground
    {
        Texture2D _backgroundTexture;
        bool _goingUp;
        float _cursor;
        float _totalLength;
        float _speed;
        Rectangle _actualWindow;

        public float Speed { get {return _speed;} set {_speed = value;} }

        public MovingBackground(Texture2D texture, bool goUp)
        {
            Debug.Assert(texture.Height > 480);

            _backgroundTexture = texture;
            _goingUp = goUp;
            _totalLength = _backgroundTexture.Height;
            _speed = 0;

            _cursor = _goingUp ? _totalLength - 800f : 0;

            _actualWindow = new Rectangle(1, (int)Math.Round(_cursor), 480, 800);
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _cursor = _cursor + _speed * dt;
            _actualWindow.Y = (int)Math.Round(_cursor);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, Vector2.One, _actualWindow, Color.White, 0, Vector2.Zero, Vector2.One, new SpriteEffects(), 0f);
        }
    }
}

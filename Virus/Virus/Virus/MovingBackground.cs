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
        Texture2D[] _backgroundTextureArray;
        //bool _goingUp;
        float _cursor;
        //int _arrayIndex;
        //float _totalLength;
        float _speed;
        int _frameHeight;

        public float Speed { get {return _speed;} set {_speed = value;} }

        public MovingBackground(Texture2D[] textureArray/*, bool goUp*/)
        {
            //Debug.Assert(texture.Height > 480);

            _backgroundTextureArray = textureArray;
            //_goingUp = goUp;
            //_totalLength = _backgroundTexture.Height;
            _speed = 0;

            //_cursor = _goingUp ? _totalLength - 800f : 0;
            _frameHeight = _backgroundTextureArray[0].Height;
            _cursor = _frameHeight * _backgroundTextureArray.Length - 800;

            //_actualWindow = new Rectangle(1, (int)Math.Round(_cursor), 480, 800);
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _cursor = _cursor - _speed * dt;
            //_actualWindow.Y = (int)Math.Round(_cursor);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_cursor < 0)
                _cursor = 0;

            // determino la texture a cui appartiene il cursore
            int frameIndexTop = (int)(_cursor / _frameHeight);

            // determino la texture a cui appartiene il punto Bottom della finestra
            int frameIndexBot = (int)((_cursor + 800) / _frameHeight);

            int top = (int)Math.Round(_cursor) % _frameHeight;

            // se i due punti appartengono allo stesso frame faccio un unico disegno
            if (frameIndexTop == frameIndexBot)
            {
                Rectangle window = new Rectangle(1, top, 480, 800);
                spriteBatch.Draw(_backgroundTextureArray[frameIndexTop], Vector2.One, window, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            }
            else
            // altrimenti devo spezzare in due disegni
            {
                Rectangle topWindow = new Rectangle(1, top, 480, _frameHeight - top);
                spriteBatch.Draw(_backgroundTextureArray[frameIndexTop], Vector2.One, topWindow, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

                Rectangle botWindow = new Rectangle(1, 1, 480, 800 - (_frameHeight - top));
                spriteBatch.Draw(_backgroundTextureArray[frameIndexBot], new Vector2(1, _frameHeight - top + 1), botWindow, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f); 
            }

            //spriteBatch.Draw(_backgroundTexture, Vector2.One, _actualWindow, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }
    }
}

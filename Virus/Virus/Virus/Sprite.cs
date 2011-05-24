using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public abstract class Sprite : IPhysicalPoint
    {
        // graphics
        private Dictionary<int, Animation> _animations;
        private Vector2 _framePosition;

        public void Move()
        {
            
        }


    }




        public void Move(GameTime gameTime)
        {
            // force is [Kg*px / sec2], Dt is [sec]
            Vector2 a = _forces / _mass;     // [px / sec2]
            _speed    = _speed    + a      * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousPosition = _position;
            _position = _position + _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            UpdateTexturePosition();
            _spriteAnimation.UpdateFrameAnimation(gameTime);
        }

        private void UpdateTexturePosition()
        {
            //_spriteAnimation.Position = new Vector2(_position.X - _spriteAnimation.RectangleWidth / 2, _position.Y - _spriteAnimation.RectangleHeight / 2);
            _spriteAnimation.Position = new Vector2(_position.X , _position.Y);
        }

    }
    }
}

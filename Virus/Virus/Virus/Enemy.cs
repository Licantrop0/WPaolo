using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Virus
{
    public class SimpleEnemy
    {
        // game general status
        int    _hitPoints = 1;
        float _lifeSeconds = 0;

        // geometrical and physycal properties
        float _radius;       // [px]
        float _mass;         // [Kg]
        float _damping;      // [Kg / (sec*px)]
  
        // motion state variables
        private Vector2 _position;     // [px, px]
        private Vector2 _speed;        // [px/s px/s]
        private Vector2 _previousPosition;  // [px, px] (used for collision filter)
        

        // forzanti
        private Vector2 _forces;

        // graphics
        SpriteAnimation _spriteAnimation;
        /*private Texture2D _texture2D;*/
        private Vector2   _texturePosition;

        // properties
        public SpriteAnimation SpriteAnimation { get { return _spriteAnimation; } set { _spriteAnimation = value; } }

        public float Radius { get { return _radius; } set { _radius = value; } }

        public float Mass { get { return _mass; } set { _mass = value; } }

        public float Damping { get { return _damping; } set { _damping = value; } }

        public Vector2 Position { get { return _position; } set { _position = value; _previousPosition = value; UpdateTexturePosition(); } }

        public Vector2 PreviousPosition { get { return _previousPosition; } }

        public Vector2 Speed { get { return _speed; } set { _speed = value; } }

        public Vector2 TexturePosition { get { return _texturePosition; } set { _texturePosition = value; } }

        //public Texture2D Texture { get { return _texture2D; } set { _texture2D = value; } }

        // constructor
        public SimpleEnemy(SpriteAnimation spriteAnimation, float radius)
        {
            //Debug.Assert(texture.Width == texture.Height);

            _spriteAnimation = spriteAnimation;

            _radius = radius;
            _mass = 1;
        }

        public void ResetForces()
        {
            _forces = Vector2.Zero;
        }

        public void AddForce(Vector2 f)
        {
            _forces += f;
        }

        public void Move(GameTime gameTime)
        {
            // force is [Kg*px / sec2], Dt is [sec]
            Vector2 a = _forces / _mass;     // [px / sec2]
            _speed    = _speed    + a      * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousPosition = _position;
            _position = _position + _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            UpdateTexturePosition();
            _spriteAnimation.Update(gameTime);
        }

        private void UpdateTexturePosition()
        {
            _spriteAnimation.Position = new Vector2(_position.X - _spriteAnimation.RectangleWidth / 2, _position.Y - _spriteAnimation.RectangleHeight / 2);
        }

    }
    
}

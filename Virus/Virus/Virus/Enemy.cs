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

        // forzanti
        private Vector2 _forces;

        // graphics
        private Texture2D _texture2D;
        private Vector2   _texturePosition;

        // properties
        public float Radius { get { return _radius; } set { _radius = value; } }

        public float Mass { get { return _mass; } set { _mass = value; } }

        public float Damping { get { return _damping; } set { _damping = value; } }

        public Vector2 Position { get { return _position; } set { _position = value; UpdateTexturePosition(); } }

        public Vector2 Speed { get { return _speed; } set { _speed = value; } }

        public Vector2 TexturePosition { get { return _texturePosition; } set { _texturePosition = value; } }

        public Texture2D Texture { get { return _texture2D; } set { _texture2D = value; } }

        // constructor
        public SimpleEnemy(Texture2D texture)
        {
            Debug.Assert(texture.Width == texture.Height);

            _texture2D = texture;
            _radius    = texture.Height / 2;
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

        public void Move(float dt)
        {
            // force is [Kg*px / sec2], Dt is [sec]
            Vector2 a = _forces / _mass;     // [px / sec2]
            _speed    = _speed    + a      * dt;
           _position  = _position + _speed * dt;
            
            UpdateTexturePosition();
        }

        private void UpdateTexturePosition()
        {
            _texturePosition.X = _position.X - _radius;
            _texturePosition.Y = _position.Y - _radius;
        }

    }
    
}

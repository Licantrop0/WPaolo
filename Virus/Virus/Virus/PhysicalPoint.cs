using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class PhysicalPoint
    {
        protected Vector2 _position;           // [px, px]
        protected Vector2 _speed;              // [px/s px/s]

        public Vector2 Position
        { get { return _position; } set { _position = value; } }

        public Vector2 Speed
        { get { return _speed; } set { _speed = value; } }

        public PhysicalPoint()
        {

        }

        public PhysicalPoint(Vector2 position)
        {
            _position = position;
        }

        // interface

        public virtual void Move(float dt)
        {
            _position = _position + _speed * dt;
        }

    }

    public class MassSystem :  PhysicalPoint
    {
        float _mass;                // [Kg]
        Vector2 _force;             // [Kg * px/s^2 Kg * px/s^2]

        public Vector2 ResultantForce
        { set { _force = value; } }

        public MassSystem(Vector2 position, float mass) : base(position)
        {
            _mass = mass;
        }

        public override void Move(float dt)
        {
            // dt is [s]
            Vector2 acceleration = _force / _mass;     // [px / s^2]
            _speed = _speed + acceleration * dt;
            _position = _position + _speed * dt;
        }
    }

}
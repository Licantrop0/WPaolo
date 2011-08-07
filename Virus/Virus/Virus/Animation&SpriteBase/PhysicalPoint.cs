using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class PhysicalKinematicPoint
    {
        protected Vector2 _position = Vector2.Zero;           // [px, px]
        protected Vector2 _speed = Vector2.Zero;              // [px/s px/s]

        public Vector2 Position
        { get { return _position; } set { _position = value; } }

        public Vector2 Speed
        { get { return _speed; } set { _speed = value; } }

        public PhysicalKinematicPoint()
        {

        }

        public PhysicalKinematicPoint(Vector2 position)
        {
            _position = position;
        }

        // interface

        public virtual void Move(float dt)
        {
            _position = _position + _speed * dt;
        }

    }

    public class PhysicalMassSystemPoint :  PhysicalKinematicPoint
    {
        float _mass = 1;                    // [Kg]
        Vector2 _force = Vector2.Zero;      // [Kg * px/s^2 Kg * px/s^2]

        public Vector2 ResultantForce
        { set { _force = value; } }

        public PhysicalMassSystemPoint()
        {

        }

        public PhysicalMassSystemPoint(Vector2 position, float mass) : base(position)
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

    public class PhysicalKinematicSpiral : PhysicalKinematicPoint
    {
        Vector2 _center;
        bool _clockwise;
        float _angle;
        float _speedModulus;

        Vector2 _auxRotational1;
        Vector2 _auxRotational2;

        public float Angle
        {
            set
            {
                _angle = value;
                _auxRotational1 = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
                _auxRotational2 = new Vector2(-(float)Math.Sin(_angle), (float)Math.Cos(_angle));
            }
        }

        public Vector2 Center
        { set { _center = value; } }

        public float SpeedModulus
        { set { _speedModulus = value; } }

        public bool Clockwise
        { set { _clockwise = value; } }

        public override void Move(float dt)
        {
            // get radial vector
            Vector2 r = Position - _center;

            // get tangential vector
            Vector2 t = new Vector2(-r.Y, r.X);

            // rotate tangential vector to generate tangential-spiral vector
            Vector2 ts = new Vector2();
            ts.X = Vector2.Dot(_auxRotational1, t);
            ts.Y = Vector2.Dot(_auxRotational2, t);

            if (!_clockwise)
                ts = Vector2.Negate(ts);

            // normalizing tangential spiral vector we obtain speed
            _speed = Vector2.Normalize(ts) * _speedModulus;

            // integrate speed to obtain position
            _position = _position + _speed * dt;
        }
    }

}

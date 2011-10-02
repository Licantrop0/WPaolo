using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public abstract class DynamicSystem
    {
        protected Vector2 _position = Vector2.Zero;     // [px, px]
        protected Vector2 _speed = Vector2.Zero;        // [px/s px/s]
        private float _angle = 0;                       // [rad]
        private float _angularSpeed = 0;                // [rad/s]

        public Vector2 Position
        { 
          get { return _position; }
          set { _position = value; }
        }

        public Vector2 Speed
        { 
            get { return _speed; }
            set { _speed = value; }
        }

        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        public float AngularSpeed
        {
            get { return _angularSpeed; }
            set { _angularSpeed = value; }
        }        

        public DynamicSystem()
        {

        }

        public abstract void Move(float dt);
        public abstract void Traslate(float dt);
        public abstract void Rotate(float dt);
    }

    public class IntegratorDynamicSystem : DynamicSystem
    {
        public IntegratorDynamicSystem()
        {

        }

        // interface

        public override void Move(float dt)
        {
            Traslate(dt);
            Rotate(dt);
        }

        public override void Traslate(float dt)
        {
            Position = Position + Speed * dt;
        }

        public override void Rotate(float dt)
        {
            Angle = Angle + AngularSpeed * dt;
        }
    }

    public class MassDoubleIntegratorDynamicSystem :  IntegratorDynamicSystem
    {
        float _mass = 1;                    // [Kg]
        float _momentum = 1;                // ???

        Vector2 _force = Vector2.Zero;      // [Kg * px/s^2 Kg * px/s^2]
        float _torque = 0;                  // [N * px]

        public void SetResultantForce(Vector2 force)
        {
            _force = force;
        }

        public void SetTorque(float torque)
        {
            _torque = torque;
        }

        public MassDoubleIntegratorDynamicSystem()
        {

        }
        
        public override void Traslate(float dt)
        {
            Vector2 acceleration = _force / _mass;     // [px / s^2]
            Speed = Speed + acceleration * dt;
            Position = Position + Speed * dt;
        }

        public override void Rotate(float dt)
        {
            float angularAcceleration = _torque / _momentum;
            AngularSpeed = AngularSpeed + angularAcceleration * dt;
            Angle = Angle + AngularSpeed * dt;
        }
    }

    public class PhysicalKinematicSpiral : IntegratorDynamicSystem
    {
        Vector2 _center;
        bool _clockwise;
        float _angle;
        float _speedModulus;

        Vector2 _auxRotational1;
        Vector2 _auxRotational2;

        public void SetSpiralParameters(float displacementAngle, Vector2 center, float speedModulus, bool clockwise)
        {
            _angle = displacementAngle;
            _auxRotational1 = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
            _auxRotational2 = new Vector2(-(float)Math.Sin(_angle), (float)Math.Cos(_angle));

            _center = center;
            _speedModulus = speedModulus;
            _clockwise = clockwise;
        }

        public override void Traslate(float dt)
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

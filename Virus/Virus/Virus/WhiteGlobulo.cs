using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public enum WhiteGlobuloState
    {
        moving,
        fading,
        falling,
        stopped,
        died
    }

    public class WhiteGlobulo : CircularSprite
    {
        public WhiteGlobulo(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations, radius, touchRadius)
        {
            _touchable = true;

            _currentAnimation = "main";
            _animations["main"].FramePerSecond = 6;
            _state = WhiteGlobuloState.moving;

            InitializePhysics();
        }

        protected virtual void InitializePhysics()
        {
             _physicalPoint = new PhysicalMassSystemPoint();           
        }

        private WhiteGlobuloState _state;
        private float _utilityTimer;

        public WhiteGlobuloState State
        { get { return _state; } }

        protected virtual void SetForce()
        {
            ((PhysicalMassSystemPoint)_physicalPoint).ResultantForce = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            // it sets _elapsedTime and _actSpriteEvent
            base.Update(gameTime);

            switch (_state)
            {
                case WhiteGlobuloState.moving:

                    if (_actSpriteEvent == null)
                    {
                        SetForce();
                        Move(_elapsedTime);
                        _animations["main"].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        _state = WhiteGlobuloState.fading;
                        _touchable = false;
                        Speed = Vector2.Normalize(Speed) * 45;
                        _animations["main"].FadeSpeed = 0.6f; //1.0f;
                        _utilityTimer = 0;
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.fingerHit)
                    {
                        _state = WhiteGlobuloState.falling;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        _animations["main"].ScalingSpeed = -1f;
                        if((int)Position.X % 2 == 0)
                            _animations["main"].RotationSpeed =  2 * (float)Math.PI;
                        else
                            _animations["main"].RotationSpeed = -2 * (float)Math.PI;
                        _utilityTimer = 0;
                    }

                    break;

                case WhiteGlobuloState.fading:

                    _animations["main"].Fade(_elapsedTime);
                    Move(_elapsedTime);
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 10)
                    {
                        _state = WhiteGlobuloState.died;
                    }

                    break;

                case WhiteGlobuloState.falling:

                    _animations["main"].ChangeDimension(_elapsedTime);
                    _animations["main"].Rotate(_elapsedTime);
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = WhiteGlobuloState.died;
                    }

                    break;

                case WhiteGlobuloState.stopped:

                    break;

                case WhiteGlobuloState.died:

                    break;

                default:
                    break;
            }
        }
    }

    public class AcceleratedWhiteGlobulo : WhiteGlobulo
    {
        public AcceleratedWhiteGlobulo(Dictionary<string, Animation> animations, float radius, float touchRadius)
            : base(animations, radius, touchRadius)
        {
            
        }

        protected override void SetForce()
        {
            ((PhysicalMassSystemPoint)_physicalPoint).ResultantForce = Vector2.Normalize(new Vector2(240, 400) - Position) * 150;
        }
    }

    public class OrbitalWhiteGlobulo : WhiteGlobulo
    {
        public OrbitalWhiteGlobulo(Dictionary<string, Animation> animations, float radius, float touchRadius)
            : base(animations, radius, touchRadius)
        {

        }

        protected override void InitializePhysics()
        {
            _physicalPoint = new PhysicalKinematicSpiral();
        }
 
        public void SetSpiralParameters(Vector2 center, float angle, bool clockwise, float speedModulus)
        {
            ((PhysicalKinematicSpiral)_physicalPoint).Center = center;
            ((PhysicalKinematicSpiral)_physicalPoint).Angle = angle;
            ((PhysicalKinematicSpiral)_physicalPoint).Clockwise = clockwise;
            ((PhysicalKinematicSpiral)_physicalPoint).SpeedModulus = speedModulus;
        }

        protected override void SetForce()
        {
            
        }
    }
}

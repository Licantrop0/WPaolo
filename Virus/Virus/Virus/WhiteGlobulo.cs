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
        private WhiteGlobuloState _state;
        private float _utilityTimer;

        public WhiteGlobuloState State
        { get { return _state; } }

        public WhiteGlobulo(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations, radius, touchRadius)
        {
            _touchable = true;

            FramePerSecond = 6;
            _state = WhiteGlobuloState.moving;
        }

        protected override void InitializePhysics()
        {
             _physicalPoint = new PhysicalMassSystemPoint();           
        }

        protected virtual void SetForce()
        {
            ((PhysicalMassSystemPoint)_physicalPoint).ResultantForce = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_state)
            {
                case WhiteGlobuloState.moving:

                    if (_actSpriteEvent == null || _actSpriteEvent.Code == SpriteEventCode.borderCollision)
                    {
                        SetForce();
                        Move();
                        Animate();
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        _state = WhiteGlobuloState.fading;
                        _touchable = false;
                        Speed = Vector2.Normalize(Speed) * 45;
                        FadeSpeed = 0.6f;
                        _utilityTimer = 0;
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.fingerHit)
                    {
                        _state = WhiteGlobuloState.falling;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        ScalingSpeed = -1.125f;
                        if((int)Position.X % 2 == 0)
                            RotationSpeed =  2.25f * (float)Math.PI;
                        else
                            RotationSpeed = -2.25f * (float)Math.PI;
                        _utilityTimer = 0;
                    }

                    break;

                case WhiteGlobuloState.fading:

                    Fade();
                    Move();
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 10)
                    {
                        _state = WhiteGlobuloState.died;
                    }

                    break;

                case WhiteGlobuloState.falling:

                    ChangeDimension();
                    Rotate();
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

    public class BouncingWhiteGlobulo : WhiteGlobulo
    {
        public BouncingWhiteGlobulo(Dictionary<string, Animation> animations, float radius, float touchRadius)
            : base(animations, radius, touchRadius)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.borderCollision)
            {
                Bounce((Vector2)_actSpriteEvent.Params[0]);
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

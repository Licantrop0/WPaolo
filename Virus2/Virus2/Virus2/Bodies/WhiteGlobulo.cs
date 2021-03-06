using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public enum GlobuloState
    {
        moving,
        fading,
        falling,
        stopped,
        died,
        faded
    }

    public class WhiteGlobulo : Enemy
    {
        private float _utilityTimer;

        protected GlobuloState _state;

        public WhiteGlobulo(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {
            Touchable = true;

            Sprite.FramePerSecond = 6;
            _state = GlobuloState.moving;
        }

        protected virtual void HowManyPoints()
        {
            WorthPoints = 5;
        }

        protected virtual void SetForce()
        {
            ((MassDoubleIntegratorDynamicSystem)DynamicSystem).SetResultantForce(Vector2.Zero);
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            switch (_state)
            {
                case GlobuloState.moving:

                    if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.virusGlobuloCollision)
                    {
                        _state = GlobuloState.fading;
                        Touchable = false;
                        Speed = Vector2.Zero;
                        Sprite.FadeSpeed = 0.6f;
                        _utilityTimer = 0;
                    }
                    else if (_actBodyEvent != null && (_actBodyEvent.Code == BodyEventCode.tap || _actBodyEvent.Code == BodyEventCode.bombHit))
                    {
                        _state = GlobuloState.falling;
                        Touchable = false;
                        Speed = Vector2.Zero;
                        ResizingSpeed = Vector2.One * -29f;
                        if ((int)Position.X % 2 == 0)
                            AngularSpeed = 2.25f * (float)Math.PI;
                        else
                            AngularSpeed = -2.25f * (float)Math.PI;
                        _utilityTimer = 0;
                    }
                    else
                    {
                        SetForce();
                        Traslate();
                        Animate();
                    }

                    break;

                case GlobuloState.fading:

                    Sprite.Fade();
                    Traslate();
                    _utilityTimer += elapsedTime;
                    if (_utilityTimer > 3)
                    {
                        _state = GlobuloState.faded;
                    }

                    break;

                case GlobuloState.falling:

                    Resize();
                    Rotate();
                    _utilityTimer += elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = GlobuloState.died;
                        HowManyPoints();
                    }

                    break;

                case GlobuloState.died:

                    break;

                default:
                    break;
            }
        }

        public override bool Moving
        {
            get
            {
                return (_state == GlobuloState.moving);
            }
        }

        public override bool Died
        {
            get
            {
                return (_state == GlobuloState.died);
            }
        }

        public override bool ToBeCleared
        {
            get
            {
                return (_state == GlobuloState.died || _state == GlobuloState.faded);
            }
        }
    }

    public class BulletWhiteGlobulo : WhiteGlobulo
    {
        public BulletWhiteGlobulo(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {

        }

        protected override void HowManyPoints()
        {
            WorthPoints = 0;
        }
    }

    public class BouncingWhiteGlobulo : WhiteGlobulo
    {
        public BouncingWhiteGlobulo(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            : base(dynamicSystem, sprite, shape)
        {
            
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.borderCollision)
            {
                Bounce((Vector2)_actBodyEvent.Params[0]);
            }    
        }
    }

    public class AcceleratedWhiteGlobulo : WhiteGlobulo
    {
        public AcceleratedWhiteGlobulo(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            : base(dynamicSystem, sprite, shape)
        {
            
        }

        protected override void SetForce()
        {
            ((MassDoubleIntegratorDynamicSystem)DynamicSystem).SetResultantForce(Vector2.Normalize(new Vector2(240, 400) - Position) * 150);
        }
    }

    public class OrbitalWhiteGlobulo : WhiteGlobulo
    {
        public OrbitalWhiteGlobulo(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            : base(dynamicSystem, sprite, shape)
        {

        }
 
        public void SetSpiralParameters(Vector2 center, float angle, bool clockwise, float speedModulus)
        {
            ((PhysicalKinematicSpiral)DynamicSystem).SetSpiralParameters(angle, center, speedModulus, clockwise);
        }

        protected override void SetForce()
        {
            
        }
    }
}

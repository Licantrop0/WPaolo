using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public enum BonusState
    {
        moving,
        fading,
        falling,
        toBeCleared
    }

    public class GoToVirusBonus : CircularSprite
    {
        BonusState _state;
        float _utilityTimer;

        public BonusState State
        {
            get { return _state; }
        }

        public GoToVirusBonus(Dictionary<string, Animation> animations, float radius, float touchRadius)
            : base(animations, radius, touchRadius)
        {
             _touchable = true;

            _currentAnimation = "main";
            _animations["main"].FramePerSecond = 0;
            _state = BonusState.moving;

            InitializePhysics();
        }

        private void InitializePhysics()
        {
            _physicalPoint = new PhysicalMassSystemPoint();
        }

        public override void Update(GameTime gameTime)
        {
            // it sets _elapsedTime and _actSpriteEvent
            base.Update(gameTime);

            switch (_state)
            {
                case BonusState.moving:

                    if (_actSpriteEvent == null)
                    {
                        Move(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        _state = BonusState.fading;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        _animations["main"].FadeSpeed = 1.0f;
                        _utilityTimer = 0;
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.fingerHit)
                    {
                        _state = BonusState.falling;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        _animations["main"].ScalingSpeed = -1f;
                        if ((int)Position.X % 2 == 0)
                            _animations["main"].RotationSpeed = 2 * (float)Math.PI;
                        else
                            _animations["main"].RotationSpeed = -2 * (float)Math.PI;
                        _utilityTimer = 0;
                    }

                    break;

                case BonusState.fading:

                    _animations["main"].Fade(_elapsedTime);
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = BonusState.toBeCleared;
                    }

                    break;

                case BonusState.falling:

                    _animations["main"].ChangeDimension(_elapsedTime);
                    _animations["main"].Rotate(_elapsedTime);
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = BonusState.toBeCleared;
                    }

                    break;

                case BonusState.toBeCleared:

                    break;

                default:
                    break;
            }
        }
    }
}

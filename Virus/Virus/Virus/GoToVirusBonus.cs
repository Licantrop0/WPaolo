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

    public enum BonusType
    {
        oneUp,
        bomb,
        ammo,
        bombAmmo
    }

    public class GoToVirusBonus : CircularSprite
    {
        BonusState _state;
        float _utilityTimer;
        BonusType _type;

        public BonusState State
        {
            get { return _state; }
        }

        public BonusType Type
        {
            get { return _type; }
        }

        public GoToVirusBonus(Dictionary<string, Animation> animations, float radius, float touchRadius, BonusType type)
            : base(animations, radius, touchRadius)
        {
             _touchable = true;

            ChangeAnimation("main");
            FramePerSecond = 0;
            RotationSpeed = 1f;
            _state = BonusState.moving;
            _type = type;
        }

        protected override void InitializePhysics()
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

                    if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        _state = BonusState.fading;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        FadeSpeed = 1.0f;
                        _utilityTimer = 0;
                    }
                    else if (_actSpriteEvent != null && (_actSpriteEvent.Code == SpriteEventCode.fingerHit || _actSpriteEvent.Code == SpriteEventCode.bombHit))
                    {
                        _state = BonusState.falling;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        ScalingSpeed = Vector2.One * (-1f);
                        if ((int)Position.X % 2 == 0)
                            RotationSpeed = 2 * (float)Math.PI;
                        else
                            RotationSpeed = -2 * (float)Math.PI;
                        _utilityTimer = 0;
                    }
                    else
                    {
                        Move();
                        Rotate();
                    }

                    break;

                case BonusState.fading:

                    Fade();
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = BonusState.toBeCleared;
                    }

                    break;

                case BonusState.falling:

                    ChangeDimension();
                    Rotate();
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

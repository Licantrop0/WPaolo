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
        bombAmmo,
        points10
    }

    public class GoToVirusBonus : TimingBehaviouralBody
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

        public GoToVirusBonus(DynamicSystem dynamicSystem, Sprite sprite, Shape shape, BonusType type)
            : base(dynamicSystem, sprite, shape)
        {
            Touchable = true;

            Sprite.ChangeAnimation("main");
            Sprite.FramePerSecond = 0;
            AngularSpeed = 1f;
            _state = BonusState.moving;
            _type = type;
        }

        public override void Update(float elapsedTime)
        {
            // it sets _elapsedTime and _actSpriteEvent
            base.Update(elapsedTime);

            switch (_state)
            {
                case BonusState.moving:

                    if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.virusBonusCollision)
                    {
                        _state = BonusState.fading;
                        Touchable = false;
                        Speed = Vector2.Zero;
                        Sprite.FadeSpeed = 1.0f;
                        _utilityTimer = 0;
                    }
                    else if (_actBodyEvent != null && (_actBodyEvent.Code == BodyEventCode.fingerHit || _actBodyEvent.Code == BodyEventCode.bombHit))
                    {
                        _state = BonusState.falling;
                        Touchable = false;
                        Speed = Vector2.Zero;
                        ResizingSpeed = Vector2.One * (-29f);
                        if ((int)Position.X % 2 == 0)
                            AngularSpeed = 2 * (float)Math.PI;
                        else
                            AngularSpeed = -2 * (float)Math.PI;
                        _utilityTimer = 0;
                    }
                    else
                    {
                        Traslate();
                        Rotate();
                    }

                    break;

                case BonusState.fading:

                    Sprite.Fade();
                    _utilityTimer += elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = BonusState.toBeCleared;
                    }

                    break;

                case BonusState.falling:

                    Resize();
                    Rotate();
                    _utilityTimer += elapsedTime;
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

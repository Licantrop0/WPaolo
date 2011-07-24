using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Kiwi
{
    public enum KiwiState
    {
        sad,
        shocked,
        happy,
        died
    }

    public class Kiwi : CircularSprite
    {
        private KiwiState _state;
        private float _utilityTimer;

        public int Ammo { get; set; }
        public int Bombs { get; set; }
        public int Lifes { get; set; }

        public KiwiState State
        { get { return _state; } set { _state = value; } }

        public Kiwi(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations, radius, touchRadius)
        {
            _touchable = true;
            FramePerSecond = 4f;
            Position = new Vector2(240, 400);
            Angle = -(float)Math.PI / 2;
            _state = KiwiState.sad;
            Ammo = 80;
            Bombs = 2;
            Lifes = 3;
        }

        protected override void InitializePhysics()
        {
            _physicalPoint = new PhysicalKinematicPoint();
        }

        private void TransitionToShockedState()
        {
            _utilityTimer = 0;

            Lifes--;
            //Angle = (float)(_actSpriteEvent.Params[0]);
            BlinkingFrequency = 30;
            BlinkingTint = Color.Transparent;
            Animate();
            Blink();
            State = KiwiState.shocked;
        }

        private void TransitionToHappyState()
        {
            _utilityTimer = 0;

            switch ((BonusType)_actSpriteEvent.Params[0])
            {
                case BonusType.oneUp:
                    Lifes++;
                    break;
                case BonusType.bomb:
                    Bombs++;
                    break;
                case BonusType.ammo:
                    Ammo += 50;
                    break;
                case BonusType.bombAmmo:
                    Bombs++;
                    Ammo += 5;
                    break;
                default:
                    break;
            }

            Tint = Color.Red;
            Animate();
            State = KiwiState.happy;
        }

        private void TransitionToTranquilState()
        {
            ChangeAnimation("main");
            Tint = Color.White;
            //Angle = 0; 
            State = KiwiState.sad;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            switch (_state)
            {
                case KiwiState.sad:

                    if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        TransitionToShockedState();        
                    }
                    else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        TransitionToHappyState();
                    }
                    else
                    {
                        Animate();
                    }

                    break;

                case KiwiState.shocked:

                    _utilityTimer += _elapsedTime;

                    if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        TransitionToShockedState();
                    }
                    else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        TransitionToHappyState();
                        _state = KiwiState.happy;
                    }
                    else
                    {
                        Animate();
                        Blink();
                    }

                    if (_utilityTimer > 1.5)
                    {
                        TransitionToTranquilState();
                    }

                    break;

                case KiwiState.happy:

                    _utilityTimer += _elapsedTime;
                    
                    if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        TransitionToShockedState();
                        _state = KiwiState.shocked;
                    }
                    else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        TransitionToHappyState();
                    }
                    else
                    {
                        Animate();
                    }

                    if (_utilityTimer > 1.0)
                    {
                        TransitionToTranquilState();
                        _state = KiwiState.sad;
                    }

                    break;

                case KiwiState.died:
                    break;

                default:
                    break;
            }

            if (Lifes <= 0)
            {
                Ammo = 0;
                _state = KiwiState.died;
            }
                
        }
    }
}

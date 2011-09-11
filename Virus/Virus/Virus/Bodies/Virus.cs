using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public enum ViruState
    {
        tranquil,
        leaving,
        //shocked,
        //happy,
        died
    }

    public class Virus : TimingBehaviouralBody
    {
        private ViruState _state;

        public int Ammo { get; set; }
        public int Bombs { get; set; }
        public int Lifes { get; set; }

        public ViruState State
        { get { return _state; } set { _state = value; } }

        public Virus(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {
            Touchable = true;
            Sprite.FramePerSecond = 4f;
            Position = new Vector2(240, 400);
            Speed = Vector2.Zero;
            _state = ViruState.tranquil;
            Ammo = 80;
            Bombs = 2;
            Lifes = 3;
        }

        public override void Update(GameTime elapsedTime)
        {
            base.Update(elapsedTime);

            switch (_state)
            {
                case ViruState.tranquil:

                    Animate();

                    if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.virusGlobuloCollision)
                    {
                        Lifes--;
                        Angle = (float)(_actBodyEvent.Params[0]);
                        SoundManager.Play("virus-hit");

                        StartBlinking(1.5f, 30, Color.Transparent);
                    }
                    else if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.virusBonusCollision)
                    {
                        SoundManager.Play("powerup");

                        switch ((BonusType)_actBodyEvent.Params[0])
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
                    }
                    else if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.go)
                    {
                        Speed = new Vector2(0, -100);
                        _state = ViruState.leaving;
                    }

                    break;

                case ViruState.leaving:
                    Traslate();
                    Animate();
                    break;

                case ViruState.died:
                    break;

                default:
                    break;
            }

            if (Lifes <= 0)
            {
                Ammo = 0;
                _state = ViruState.died;
            }

        }

        /*public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            switch (_state)
            {
                case ViruState.tranquil:

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

                case ViruState.shocked:

                    _utilityTimer += _elapsedTime;

                    if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        TransitionToShockedState();
                    }
                    else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        TransitionToHappyState();
                        _state = ViruState.happy;
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

                case ViruState.happy:

                    _utilityTimer += _elapsedTime;
                    
                    if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        TransitionToShockedState();
                        _state = ViruState.shocked;
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
                        _state = ViruState.tranquil;
                    }

                    break;

                case ViruState.died:
                    break;

                default:
                    break;
            }

            if (Lifes <= 0)
            {
                Ammo = 0;
                _state = ViruState.died;
            }
                
        }*/

        /*private void TransitionToShockedState()
        {
            _utilityTimer = 0;

            Lifes--;
            Angle = (float)(_actSpriteEvent.Params[0]);
            BlinkingFrequency = 30;
            BlinkingTint = Color.Transparent;
            Animate();
            Blink();
            State = ViruState.shocked;
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
            State = ViruState.happy;
        }

        private void TransitionToTranquilState()
        {
            ChangeAnimation("main");
            Tint = Color.White;
            Angle = 0;
            State = ViruState.tranquil;
        }*/

        


    }
}

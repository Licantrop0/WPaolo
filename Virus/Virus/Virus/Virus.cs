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
        shocked,
        happy,
        died
    }

    public class Virus : CircularSprite
    {
        private ViruState _state;
        private float _utilityTimer;

        public int Score { get; set; }
        public int Bombs { get; set; }

        public ViruState State
        { get { return _state; } set { _state = value; } }

        public Virus(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations, radius, touchRadius)
        {
            _touchable = true;
            _physicalPoint = new PhysicalKinematicPoint();
            _currentAnimation = "main";
            _animations["main"].FramePerSecond = 4f;
            Position = new Vector2(240, 400);
            _state = ViruState.tranquil;
            Score = 500;
            Bombs = 5;
        }

        private void HandleGlobuloCollision()
        {
            Score -= 50;
            // TODO CAMBIA ANIMAZIONE
            float angle = (float)(_actSpriteEvent.Params[0]);
            _animations[_currentAnimation].Color = Color.Blue;
            _animations[_currentAnimation].Angle = angle;
            _animations[_currentAnimation].Animate(_elapsedTime);
            _utilityTimer = 0;
        }

        private void HandleBonusCollision()
        {
            Bombs++;
            // TODO CAMBIA ANIMAZIONE
            _animations[_currentAnimation].Color = Color.Red;
            _animations[_currentAnimation].Animate(_elapsedTime);
            _utilityTimer = 0;
        }

        private void GoBackToTranquilState()
        {
            _currentAnimation = "main";
            _animations[_currentAnimation].Color = Color.White;
            _animations[_currentAnimation].Angle = 0;  //supefluo quando ci sarà l'animazione dedicata allo stato "shocked"
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            switch (_state)
            {
                case ViruState.tranquil:

                    if (_actSpriteEvent == null)
                    {
                        _animations[_currentAnimation].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        HandleGlobuloCollision();
                        _state = ViruState.shocked;
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        HandleBonusCollision();
                        _state = ViruState.happy;
                    }

                    break;

                case ViruState.shocked:

                    _utilityTimer += _elapsedTime;

                    if (_actSpriteEvent == null)
                    {
                        _animations[_currentAnimation].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        HandleGlobuloCollision();
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        HandleBonusCollision();
                        _state = ViruState.happy;
                    }

                    if (_utilityTimer > 1.0)
                    {
                        GoBackToTranquilState();
                        _state = ViruState.tranquil;
                    }

                    break;

                case ViruState.happy:

                    _utilityTimer += _elapsedTime;

                    if (_actSpriteEvent == null)
                    {
                        _animations[_currentAnimation].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        HandleGlobuloCollision();
                        _state = ViruState.shocked;
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        HandleBonusCollision();
                    }

                    if (_utilityTimer > 1.0)
                    {
                        GoBackToTranquilState();
                        _state = ViruState.tranquil;
                    }

                    break;

                case ViruState.died:
                    break;

                default:
                    break;
            }

            if (Score < 0)
            {
                Score = 0;
                _state = ViruState.died;
            }
                
        }
    }
}

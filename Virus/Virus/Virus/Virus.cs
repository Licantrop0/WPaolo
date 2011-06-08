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
        public int Lifes { get; set; }

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
            Lifes = 5;
        }

        private void TransitionToShockedState()
        {
            Lifes--;
            float angle = (float)(_actSpriteEvent.Params[0]);
            _animations[_currentAnimation].BlinkingFrequency = 30;
            _animations[_currentAnimation].BlinkingTint = Color.Transparent;
            _animations[_currentAnimation].Angle = angle;
            _animations[_currentAnimation].Animate(_elapsedTime);
            _animations[_currentAnimation].Blink(_elapsedTime);
            _state = ViruState.shocked;
            _utilityTimer = 0;
        }

        private void TransitionToHappyState()
        {
            Bombs++;    // PS temp
            _animations[_currentAnimation].Color = Color.Red;
            _animations[_currentAnimation].Animate(_elapsedTime);
            _state = ViruState.happy;
            _utilityTimer = 0;
        }

        private void TransitionToTranquilState()
        {
            _currentAnimation = "main";
            _animations[_currentAnimation].Color = Color.White;
            _animations[_currentAnimation].Angle = 0; 
            _state = ViruState.tranquil;
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
                        TransitionToShockedState();
                        
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        TransitionToHappyState();
                    }

                    break;

                case ViruState.shocked:

                    _utilityTimer += _elapsedTime;

                    if (_actSpriteEvent == null)
                    {
                        _animations[_currentAnimation].Animate(_elapsedTime);
                        _animations[_currentAnimation].Blink(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        TransitionToShockedState();
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        TransitionToHappyState();
                        _state = ViruState.happy;
                    }

                    if (_utilityTimer > 1.5)
                    {
                        TransitionToTranquilState();
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
                        TransitionToShockedState();
                        _state = ViruState.shocked;
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusBonusCollision)
                    {
                        TransitionToHappyState();
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
                Score = 0;
                _state = ViruState.died;
            }
                
        }
    }
}

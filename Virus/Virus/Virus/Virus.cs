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
        died
    }

    public class Virus : CircularSprite
    {
        private ViruState _state;
        private float _utilityTimer;

        public int Score { get; set; }

        public ViruState State
        { get { return _state; } set { _state = value; } }

        public Virus(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations, radius, touchRadius)
        {
            _physicalPoint = new PhysicalPoint();
            _currentAnimation = "main";
            _animations["main"].FramePerSecond = 4f;
            Position = new Vector2(240, 400);
            _state = ViruState.tranquil;
            Score = 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_state)
            {
                case ViruState.tranquil:

                    if (_actSpriteEvent == null)
                    {
                        _animations["main"].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        Score -= 20;
                        float angle = (float)(_actSpriteEvent.Params[0]);
                        // TODO CAMBIA ANIMAZIONE
                        _animations["main"].Angle = angle;
                        _animations["main"].Animate(_elapsedTime);
                        _state = ViruState.shocked;
                        _utilityTimer = 0;
                    }

                    break;

                case ViruState.shocked:

                    _utilityTimer += _elapsedTime;

                    if (_actSpriteEvent == null)
                    {
                        _animations["main"].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        Score -= 20;

                        float angle = (float)(_actSpriteEvent.Params[0]);
                        _animations["main"].Angle = angle;
                        _animations["main"].Animate(_elapsedTime);
                        _state = ViruState.shocked;
                        _utilityTimer = 0;
                    }

                    if (_utilityTimer > 0.5)
                    {
                        _state = ViruState.tranquil;
                    }

                    if (Score <= 0)
                    {
                        _state = ViruState.died;
                    }

                    break;

                case ViruState.died:
                    break;

                default:
                    break;
            }
        }
    }
}

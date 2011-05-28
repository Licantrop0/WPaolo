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
        rotatingScaling,    // temp debug!
        stopped,
        died
    }

    public class WhiteGlobulo : CircularSprite
    {
        public WhiteGlobulo(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations, radius, touchRadius)
        {
            _touchable = true;

            _physicalPoint = new PhysicalPoint();
            _currentAnimation = "main";
            _animations["main"].FramePerSecond = 6;
            _state = WhiteGlobuloState.moving;
        }

        private WhiteGlobuloState _state;
        private float _utilityTimer;

        public WhiteGlobuloState State
        { get { return _state; } }

        public override void Update(GameTime gameTime)
        {
            // it sets _elapsedTime and _actSpriteEvent
            base.Update(gameTime);

            switch (_state)
            {
                case WhiteGlobuloState.moving:

                    if (_actSpriteEvent == null)
                    {
                        Move(_elapsedTime);
                        _animations["main"].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.virusGlobuloCollision)
                    {
                        _state = WhiteGlobuloState.fading;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        _animations["main"].FadeSpeed = 1.0f;
                        _utilityTimer = 0;
                    }
                    else if (_actSpriteEvent.Code == SpriteEventCode.fingerHit)
                    {
                        _state = WhiteGlobuloState.rotatingScaling;
                        _touchable = false;
                        Speed = Vector2.Zero;
                        _animations["main"].ScalingSpeed = -1f;
                        if((int)Position.X % 2 == 0)
                            _animations["main"].RotationSpeed =  2 * (float)Math.PI;
                        else
                            _animations["main"].RotationSpeed = -2 * (float)Math.PI;
                        _utilityTimer = 0;
                    }

                    break;

                case WhiteGlobuloState.fading:

                    _animations["main"].Fade(_elapsedTime);
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = WhiteGlobuloState.died;
                    }

                    break;

                case WhiteGlobuloState.rotatingScaling:

                    _animations["main"].ChangeDimension(_elapsedTime);
                    _animations["main"].Rotate(_elapsedTime);
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
}

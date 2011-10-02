using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VirusLib.Sprites
{
    public class LateralMouth : Mouth
    {
        #region constants

        const float DELTA_SPACE = 80;
        const float DELTA_ANGLE = (float)Math.PI * (100f / 180f);

        #endregion

        # region private parameters

        // fixed
        int _globulosToSpit = 4;
        
        #endregion

        #region private memebers state

        Vector2 _initialPosition;
        Vector2 _leavingSpeed;
        float _spitAngle;
        float _initialAngle;
        int _spittedGlobulos;

        #endregion

        #region constructors

        public LateralMouth(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            : base(dynamicSystem, sprite, shape)
        {
            _hitPoints = 3;
            _state = MouthState.idle;
            _globulosSpeed = 250;
            _openingTime = 0.3f;
            _mouthOpenTime = 0.3f;
            Position = new Vector2(-100, -100);
        }

        #endregion

        #region update and auxiliary methods

        protected override void FireGlobulo(TimeSpan t)
        {
            Vector2 speedVersor = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            Vector2 position = Position + 30 * speedVersor;
            GameManager.ScheduleEvent(new GameEvent(t, GameEventType.createMouthBullet, MonsterFactory,
                new Object[] { position, speedVersor * _globulosSpeed }));
        }

        public override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            // handle death transition
            if (_hitPoints <= 0)
            {
                if (Freezed)
                {
                    EndFreeze();
                }
                // state in which mouth is closed
                if (_state == MouthState.approching || _state == MouthState.leaving ||
                    _state == MouthState.preRotating || _state == MouthState.rotating || _state == MouthState.postRotating)
                {
                    Sprite.ChangeAnimation("death");
                    Sprite.FramePerSecond = 10;
                    _state = MouthState.dying;
                    SoundManager.Play("small-mouth-death");
                }
                // state in which mouth is open or partially opened
                else if (_state == MouthState.closing || _state == MouthState.opening ||
                    _state == MouthState.mouthOpenAfter || _state == MouthState.mouthOpenBefore)
                {
                    Sprite.AnimationVerse = false;
                    Sprite.FramePerSecond = Sprite.AnimationFrames / _openingTime;
                    _state = MouthState.dyingMouthClosing;
                }
            }
            // hanlde hit
            else if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.fingerHit)
            {
                _hitPoints--;
                StartBlinking(0.3f, 30, Color.Transparent);
            }
            // handle bomb
            else if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.bombHit)
            {
                StartFreeze(2);
            }

            // handle standard state machine
            switch (_state)
            {
                case MouthState.idle:

                    if (_actBodyEvent != null && _actBodyEvent.Code == BodyEventCode.awake)
                    {
                        // assign leaving speed
                        _leavingSpeed = -Speed;

                        _initialPosition = Position;
                        Angle = (float)Math.Atan2(Speed.Y, Speed.X);
                        _initialAngle = Angle;

                        // initialize and change state
                        _spittedGlobulos = 0;
                        Sprite.ChangeAnimation("opening");
                        Sprite.FramePerSecond = 0;

                        Touchable = true;
                        _state = MouthState.approching;
                    }
                    break;

                case MouthState.approching:

                    Traslate();

                    if (Vector2.Distance(Position, _initialPosition) >= DELTA_SPACE)
                    {
                        _state = MouthState.preRotating;
                    }

                    break;

                case MouthState.preRotating:

                    Rotate();

                    if (Math.Abs(_initialAngle - Angle) >= DELTA_ANGLE / 2)
                    {
                        // reverse rotation asped for rotating state
                        AngularSpeed = -AngularSpeed;

                        // initialize variables for first mouth opening
                        Sprite.FramePerSecond = Sprite.AnimationFrames / _openingTime;
                        _state = MouthState.opening;

                        SoundManager.Play("small-mouth-opens");
                    }

                    break;

                case MouthState.opening:

                    Animate();

                    if (Sprite.AnimationFinished())
                    {
                        Sprite.AnimationVerse = false;
                        Sprite.FramePerSecond = 0;
                        ResetAndStartTimer(_mouthOpenTime / 2);
                        _state = MouthState.mouthOpenBefore;
                    }

                    break;

                case MouthState.mouthOpenBefore:

                    if (Exceeded())
                    {
                        ResetAndStartTimer(_mouthOpenTime / 2);
                        _spittedGlobulos++;
                        _spitAngle = Angle;
                        FireGlobulo(gameTime);
                        _state = MouthState.mouthOpenAfter;
                    }

                    break;

                case MouthState.mouthOpenAfter:

                    if (Exceeded())
                    {
                        Sprite.FramePerSecond = Sprite.AnimationFrames / _openingTime;
                        _state = MouthState.closing;
                    }

                    break;

                case MouthState.closing:

                    Animate();

                    if (Sprite.AnimationFinished())
                    {
                        Sprite.AnimationVerse = true;

                        if (_spittedGlobulos == _globulosToSpit)
                        {
                            AngularSpeed = -AngularSpeed;
                            _state = MouthState.postRotating;
                        }
                        else
                        {
                            _state = MouthState.rotating;
                        }
                    }

                    break;

                case MouthState.rotating:

                    Rotate();

                    if (Math.Abs(Angle - _spitAngle) >= DELTA_ANGLE / (_globulosToSpit - 1))
                    {
                        Sprite.FramePerSecond = Sprite.AnimationFrames / _openingTime;
                        _state = MouthState.opening;

                        SoundManager.Play("small-mouth-opens");
                    }

                    break;

                case MouthState.postRotating:

                    if (Math.Abs(Angle - _initialAngle) < 0.01)
                    {
                        Speed = _leavingSpeed;
                        _state = MouthState.leaving;
                    }
                    else
                    {
                        Rotate();
                    }

                    break;

                case MouthState.leaving:

                    if (Math.Abs(Vector2.Distance(Position, _initialPosition)) < 3)
                    {
                        Touchable = false;
                        _state = MouthState.idle;
                    }
                    else
                    {
                        Traslate();
                    }

                    break;

                case MouthState.dyingMouthClosing:

                    Animate();

                    if (Sprite.AnimationFinished())
                    {
                        Sprite.ChangeAnimation("death");
                        Sprite.FramePerSecond = 10;
                        _state = MouthState.dying;
                        SoundManager.Play("small-mouth-death");
                    }

                    break;

                case MouthState.dying:

                    Animate();

                    if (Sprite.AnimationFinished())
                    {
                        ResetAndStartTimer(1);
                        Sprite.FadeSpeed = 1f;
                        Touchable = false;
                        _state = MouthState.fading;
                    }

                    break;

                case MouthState.fading:

                    Sprite.Fade();

                    if (Exceeded())
                    {
                        _state = MouthState.died;
                    }

                    break;

                case MouthState.died:
                    break;

                default:
                    break;
            }
        }

        #endregion    
    }
}

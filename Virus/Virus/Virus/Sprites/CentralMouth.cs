using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus.Sprites
{
    public class CentralMouth : Mouth
    {
        #region private members parameters

        // fixed
        float _closingTime = 0.3f;
        float _idlePeriodMin = 0.5f;
        float _idlePeriodMax = 1.5f;

        // to be initialized properly (they depend on left flag)
        bool _left;
        float _shootingAngleMin;
        float _shootingAngleMax;
        Vector2 _deltaPosition;
        
        #endregion

        #region private members others

        // reference to boss lung to be solidal in position
        BossLung _bossLung;
        
        // random variable
        Random _dice; 

        #endregion

        #region constructors

        public CentralMouth(Dictionary<string, Animation> animations, BossLung bossLung, bool left, float radius, float touchRadius)
            : base(animations, radius, touchRadius)
        {
            _hitPoints = 20;
            _state = MouthState.idle;
            _left = left;
            _bossLung = bossLung;
            _openingTime = 0.5f;
            _mouthOpenTime = 0.1f;
            _globulosSpeed = 150;
            Position = new Vector2(-100, -100);

            if (_left)
            {
                _deltaPosition = new Vector2(-172, 48);
                _shootingAngleMax = -1.75f * (float)Math.PI;
                _shootingAngleMin = -1.5f * (float)Math.PI;
                _dice = new Random(DateTime.Now.Millisecond);
            }
            else
            {
                _deltaPosition = new Vector2(192, 40);
                _shootingAngleMax = -1.5f * (float)Math.PI;
                _shootingAngleMin = -1.25f * (float)Math.PI;
                _dice = new Random(DateTime.Now.Minute);
            }

            RestartTimer((float)_dice.RandomDouble(_idlePeriodMin, _idlePeriodMax));
        }

        #endregion

        #region update and auxiliary methods

        protected override void FireGlobulo(TimeSpan t)
        {
            float shootingAngle = (float)_dice.RandomDouble(_shootingAngleMin, _shootingAngleMax);

            Vector2 speedVersor = new Vector2((float)Math.Cos(shootingAngle), (float)Math.Sin(shootingAngle));
            Vector2 position = Position + 0 * speedVersor;
            GameManager.ScheduleEvent(new GameEvent(t, GameEventType.createMouthBullet, MonsterFactory,
                new Object[] { position, speedVersor * _globulosSpeed }));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // set position
            Position = _bossLung.Position + _deltaPosition;

            // handle death transition
            if (_hitPoints <= 0)
            {
                if (Freezed)
                {
                    EndFreeze();
                }
                // state in which mouth is closed
                if (_state == MouthState.idle)
                {
                    ChangeAnimation("death");
                    FramePerSecond = 10;
                    _state = MouthState.dying;
                }
                // state in which mouth is open or partially opened
                else if (_state == MouthState.opening ||
                    _state == MouthState.mouthOpenAfter || _state == MouthState.mouthOpenBefore)
                {
                    SetAnimationVerse(false);
                    FramePerSecond = AnimationFrames / _openingTime;
                    _state = MouthState.dyingMouthClosing;
                }
                else if (_state == MouthState.closing)
                {
                    _state = MouthState.dyingMouthClosing;
                }
            }
            // hanlde hit
            else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.fingerHit)
            {
                _hitPoints--;
                StartBlinking(0.3f, 30, Color.Transparent);
            }
            // handle bomb
            else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.bombHit)
            {
                StartFreeze(2);
            }

            // handle standard state machine
            switch (_state)
            {
                case MouthState.idle:

                    if (Exceeded())
                    {
                        ChangeAnimation("opening");
                        FramePerSecond = AnimationFrames / _openingTime;
                        _state = MouthState.opening;
                    }

                    break;

                case MouthState.opening:

                    Animate();

                    if (AnimationFinished())
                    {
                        FramePerSecond = 0;
                        RestartTimer(_mouthOpenTime);
                        _state = MouthState.mouthOpenBefore;
                    }

                    break;

                case MouthState.mouthOpenBefore:

                    if (Exceeded())
                    {
                        RestartTimer(_mouthOpenTime);
                        FireGlobulo(gameTime.TotalGameTime);
                        _state = MouthState.mouthOpenAfter;
                    }

                    break;

                case MouthState.mouthOpenAfter:

                    if (Exceeded())
                    {
                        ChangeAnimation("closing");
                        FramePerSecond = AnimationFrames / _closingTime;
                        _state = MouthState.closing;
                    }

                    break;

                case MouthState.closing:

                    Animate();

                    if (AnimationFinished())
                    {
                        RestartTimer((float)_dice.RandomDouble(_idlePeriodMin, _idlePeriodMax));
                        _state = MouthState.idle;
                    }

                    break;

                case MouthState.dyingMouthClosing:

                    Animate();

                    if (AnimationFinished())
                    {
                        ChangeAnimation("death");
                        FramePerSecond = 10;
                        _state = MouthState.dying;
                    }

                    break;

                case MouthState.dying:

                    Animate();

                    if (AnimationFinished())
                    {
                        _touchable = false;
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

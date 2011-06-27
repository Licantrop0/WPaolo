using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public enum MouthState
    {
        idle,
        approching,
        preRotating,
        rotatingAndFiring,
        postRotating,
        leaving,
        died
    }

    public class Mouth : CircularSprite
    {
        const float DELTA_SPACE = 45;
        const float DELTA_ANGLE = (float)Math.PI * (150 / 180);

        int _hitPoints;
        MouthState _state;
        Vector2 _leavingSpeed;

        float _utilityTimer;
        float _approchingTime;
        float _rotatingTime;

        float _firingPeriod;
        float _firingTimer;

        public MouthState State
        { get { return _state; } }

        public Mouth(Dictionary<string, Animation> animations, float radius, float touchRadius)
            : base(animations, radius, touchRadius)
        {
            _hitPoints = 10;
            _state = MouthState.idle;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_state)
            {
                case MouthState.idle:

                    if (_actSpriteEvent.Code == SpriteEventCode.awake)
                    {
                        // assign leaving speed
                        _leavingSpeed = -Speed;
                        _approchingTime = DELTA_SPACE / Speed.Length();
                        _rotatingTime = DELTA_ANGLE / Math.Abs(_rotationSpeed);
                        Angle = (float)Math.Atan2(Speed.Y, Speed.X);

                        _utilityTimer = 0;
                        _state = MouthState.approching;
                    }
                    break;

                case MouthState.approching:

                    _utilityTimer += _elapsedTime;

                    if (_utilityTimer > _approchingTime)
                    {
                        _utilityTimer = 0;
                        _state = MouthState.preRotating;
                    }
                    else
                    {
                        Move();
                    }
                        
                    break;

                case MouthState.preRotating:

                    _utilityTimer += _elapsedTime;

                    if (_utilityTimer > _rotatingTime)
                    {
                        _rotationSpeed = -_rotationSpeed;
                        _utilityTimer = 0;
                        _state = MouthState.rotatingAndFiring;
                    }
                    else
                    {
                        Rotate();
                    }

                    break;

                case MouthState.rotatingAndFiring:
                    break;

                case MouthState.leaving:
                    break;

                case MouthState.died:
                    break;

                default:
                    break;
            }
        }
    }

    public class BossLung : RectangularSprite
    {
        int _hitPoints;

        // mouth structures
        Mouth[] _leftMouths;
        Mouth[] _bottomMouths;
        Mouth[] _rightMouths;

        // mouth handling structures
        float _approchingPeriodMin;
        float _approchingPeriodMax;
        float _approchingSpeed;
        float _rotatingSpeed;

        float _leftTimer;
        float _leftTimeToCall;

        float _bottomTimer;
        float _bottomTimeToCall;

        float _rightTimer;
        float _rightTimeToCall;

        Random _dice  = new Random(DateTime.Now.Millisecond);

        public BossLung(Dictionary<string, Animation> animations, float width, float height, float touchWidth, float touchHeight)
            : base(animations, width, height, touchWidth, touchHeight)
        {
            

        }

        private void AwakeMouth(Mouth[] mouthArray, bool vertical, float fixedPosition, float variablePositionMin, float variablePositionMax, float distance, Vector2 speedVersor)
        {
            // chose randomly the mouth to awake
            int idleMouths = mouthArray.Where(m => m.State == MouthState.idle).Count();

            // if there is not a mouth to pick exit (no mouth awaken)
            if (idleMouths == 0)
                return;

            // get position of the active mouth, if any
            Mouth activeMouth = (Mouth)(mouthArray.Where(m => m.State != MouthState.idle && m.State != MouthState.died).First());
            // VERIFICARE CHE TORNA NULL SE NON CI SONO BOCCHE ATTIVE SENNO' CASCA TUTTO!!!

            // awake a idle mouth picking randomly
            int mouthPickedIndex = _dice.Next(1, idleMouths + 1);

            int i = 1;
            Mouth awakenMouth = null;
            foreach (Mouth m in mouthArray)
            {
                awakenMouth = m;

                if (m.State == MouthState.idle)
                {
                    if (i == mouthPickedIndex)
                    {
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            // set position and speed of chosen mouth and awake it!
            float position, referencePosition, middlePosition;

            // if there is no active mouths, position is taken randomly
            if (activeMouth == null)
            {
                position = (float)_dice.RandomDouble(variablePositionMin, variablePositionMax);
            }
            // else position is at fixed distance from the active mouth, on the emptier side
            else
            {
                if (vertical)
                    referencePosition = activeMouth.Position.Y;
                else
                    referencePosition = activeMouth.Position.X;

                middlePosition = (variablePositionMin + variablePositionMax) / 2;

                if (referencePosition > middlePosition)
                    position = referencePosition - distance;
                else
                    position = referencePosition + distance;
            }

            if (vertical)
                awakenMouth.Position = new Vector2(fixedPosition, position);
            else
                awakenMouth.Position = new Vector2(position, fixedPosition);

            // set linear speed
            awakenMouth.Speed = speedVersor * _approchingSpeed;

            // set rotational speed
            awakenMouth.RotationSpeed = (int)position % 2 == 0 ? _rotatingSpeed : -_rotatingSpeed;

            // awake!
            awakenMouth.AddSpriteEvent(new SpriteEvent(SpriteEventCode.awake));
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // aggiorna le frequenze e velocità delle bocche in maniera inversamente proporzionale agli hit points
            UpdateMouthSpeedParameters();

            // update timers and awake mouth if timer is expired
            _leftTimer += _elapsedTime;
            if (_leftTimer > _leftTimeToCall)
            {
                _leftTimer -= _leftTimeToCall;
                _leftTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
                AwakeMouth(_leftMouths, true, -30, 250, 750, 200, new Vector2(1, 0));
            }

            _bottomTimer += _elapsedTime;
            if (_bottomTimer > _bottomTimeToCall)
            {
                _bottomTimer -= _bottomTimeToCall;
                _bottomTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
                AwakeMouth(_bottomMouths, false, 830, 50, 430, 140, new Vector2(0, -1));
            }

            _rightTimer += _elapsedTime;
            if (_rightTimer > _rightTimeToCall)
            {
                _rightTimer -= _rightTimeToCall;
                _rightTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
                AwakeMouth(_rightMouths, true, 480, 250, 750, 200, new Vector2(-1, 0));
            }
        }

        private void UpdateMouthSpeedParameters()
        {
            throw new NotImplementedException();
        }

    }

    
}

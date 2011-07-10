using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Virus
{
    public enum MouthState
    {
        idle,
        approching,
        preRotating,
        rotating,
        chargingBefore,
        mouthOpenBefore,
        mouthOpenAfter,
        chargingAfter,
        postRotating,
        leaving,
        died
    }

    public class Mouth : CircularSprite
    {
        const float DELTA_SPACE = 50;
        const float DELTA_ANGLE = (float)Math.PI * (120f / 180f);

        static GameEventsManager _gameManager;
        static MonsterFactory _monsterFactory;

        static public GameEventsManager GameManager { set { _gameManager = value; } }
        static public MonsterFactory MonsterFactory { set { _monsterFactory = value; } }

        int _hitPoints;
        MouthState _state;
        Vector2 _leavingSpeed;

        float _timer;

        float _approachingTime;
        float _rotatingTime;
        float _openingTime;
        float _mouthOpenTime;

        int _globulosToSpit = 4;
        int _spittedGlobulos;
        float _globulosSpeed = 150;

        public float OpeningTime { set { _openingTime = value; } }
        public float MouthOpenTime { set { _mouthOpenTime = value; } }
        public float GlobulosSpeed { set { _globulosSpeed = value; } }
        public int HitPoints { set { _hitPoints = value; } }

        public MouthState State
        { get { return _state; } }

        public Mouth(Dictionary<string, Animation> animations, float radius, float touchRadius)
            : base(animations, radius, touchRadius)
        {
            _hitPoints = 10;
            _state = MouthState.idle;
            Position = new Vector2(-100, -100);
        }

        protected override void InitializePhysics()
        {
            _physicalPoint = new PhysicalMassSystemPoint();
        }

        private void FireGlobulo(TimeSpan t)
        {
            Vector2 speedVersor = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            Vector2 position = Position + 20 * speedVersor;
            _gameManager.ScheduleEvent(new GameEvent(t, GameEventType.createBouncingEnemy, _monsterFactory,
                new Object[] { position, speedVersor * _globulosSpeed }));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_state)
            {
                case MouthState.idle:

                    if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.awake)
                    {
                        // assign leaving speed
                        _leavingSpeed = -Speed;

                        // calculating approaching and rotating time
                        _approachingTime = DELTA_SPACE / Speed.Length();
                        _rotatingTime = (DELTA_ANGLE) / Math.Abs(_rotationSpeed);

                        Angle = (float)Math.Atan2(Speed.Y, Speed.X);

                        // initialize and change state
                        _spittedGlobulos = 0;
                        ChangeAnimation("opening");
                        FramePerSecond = 0;

                        _timer = 0;
                        _state = MouthState.approching;
                    }
                    break;

                case MouthState.approching:

                    _timer += _elapsedTime;

                    if (_timer > _approachingTime)
                    {
                        _timer = 0;
                        _state = MouthState.preRotating;
                    }
                    else
                    {
                        Move();
                    }
                        
                    break;

                case MouthState.preRotating:

                    _timer += _elapsedTime;

                    if (_timer > _rotatingTime / 2)
                    {
                        // reverse rotation asped for rotating state
                        _rotationSpeed = -_rotationSpeed;

                        // initialize variables for first mouth opening
                        _timer = 0;
                        FramePerSecond = AnimationFrames / _openingTime ;
                        _state = MouthState.chargingBefore;
                    }
                    else
                    {
                        Rotate();
                    }

                    break;

                case MouthState.chargingBefore:

                    Animate();

                    if (AnimationFinished())
                    {
                        _timer = 0;
                        ReverseAnimation();
                        FramePerSecond = 0;

                        _state = MouthState.mouthOpenBefore;
                    }

                    break;

                case MouthState.mouthOpenBefore:

                    _timer += _elapsedTime;

                    if (_timer >= _openingTime / 2)
                    {
                        _timer = 0;
                        _spittedGlobulos++;
                        FireGlobulo(gameTime.TotalGameTime);
                        _state = MouthState.mouthOpenAfter;
                    }

                    break;

                case MouthState.mouthOpenAfter:

                    _timer += _elapsedTime;

                    if (_timer >= _openingTime / 2)
                    {
                        FramePerSecond = AnimationFrames / _openingTime;
                        _state = MouthState.chargingAfter;
                    }

                    break;

                case MouthState.chargingAfter:

                    Animate();

                    if (AnimationFinished())
                    {
                        ReverseAnimation();
                        _timer = 0;

                        if (_spittedGlobulos == _globulosToSpit)
                        {
                            _rotationSpeed = -_rotationSpeed;
                            _state = MouthState.postRotating;
                        }
                        else
                        {
                            _state = MouthState.rotating;
                        }
                    }

                    break;

                case MouthState.rotating:

                    _timer += _elapsedTime;

                    float chargeTime = _rotatingTime / (_globulosToSpit - 1);

                    Rotate();

                    if (_timer > chargeTime)
                    {
                        //_timer -= chargeTime;
                        _timer = 0;

                        FramePerSecond = AnimationFrames / _openingTime;
                        _state = MouthState.chargingBefore;
                    }

                    break;

                case MouthState.postRotating:

                    _timer += _elapsedTime;

                    if (_timer > _rotatingTime / 2)
                    {
                        Speed = _leavingSpeed;
                        _timer = 0;
                        _state = MouthState.leaving;
                    }
                    else
                    {
                        Rotate();
                    }

                    break;

                case MouthState.leaving:

                    _timer += _elapsedTime;

                    if (_timer > _approachingTime)
                    {
                        _timer = 0;
                        _state = MouthState.idle;
                    }
                    else
                    {
                        Move();
                    }
                        
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
        List<Mouth> _idleLeftMouths;
        List<Mouth> _idleBottomMouths;
        List<Mouth> _idleRightMouths;

        List<Mouth> _activeLeftMouths = new List<Mouth>();
        List<Mouth> _activeBottomMouths = new List<Mouth>();
        List<Mouth> _activeRightMouths = new List<Mouth>();

        /*Mouth[] _leftMouths;
        Mouth[] _bottomMouths;
        Mouth[] _rightMouths;*/

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

        public BossLung(Dictionary<string, Animation> animations, float width, float height, float touchWidth, float touchHeight, 
            Dictionary<string, Animation> mouthAnimations, GameEventsManager gm, MonsterFactory mf)
            : base(animations, width, height, touchWidth, touchHeight)
        {
            // initialize mouths
            Mouth.GameManager = gm;
            Mouth.MonsterFactory = mf;

            int i = 0;

            Animation opening = mouthAnimations["opening"];
            Animation death = mouthAnimations["death"];
            Dictionary<string, Animation> aux;

            _idleLeftMouths = new List<Mouth>();
            for (i = 0; i < 5; i++)
            {
                 aux = new Dictionary<string, Animation>();
                 aux.Add("opening", opening.Clone());
                 aux.Add("death", death.Clone());

                _idleLeftMouths.Add(new Mouth(aux, 40, 40));
            }
                
            _idleBottomMouths = new List<Mouth>();
            for (i = 0; i < 5; i++)
            {
                aux = new Dictionary<string, Animation>();
                aux.Add("opening", opening.Clone());
                aux.Add("death", death.Clone());

                _idleBottomMouths.Add(new Mouth(aux, 40, 40));
            }
                
            _idleRightMouths = new List<Mouth>(); ;
            for (i = 0; i < 5; i++)
            {
                aux = new Dictionary<string, Animation>();
                aux.Add("opening", opening.Clone());
                aux.Add("death", death.Clone());

                _idleRightMouths.Add(new Mouth(aux, 40, 40));
            }
                
            _leftTimeToCall = 3;
            _bottomTimeToCall = 6;
            _rightTimeToCall = 9;
        }

        protected override void InitializePhysics()
        {
            _physicalPoint = new PhysicalMassSystemPoint();
        }
         
        private void AwakeMouth(List<Mouth> idleMouthsList, List<Mouth> activeMouthsList, bool vertical, float fixedPosition, float variablePositionMin, float variablePositionMax, float distance, Vector2 speedVersor)
        {
            // chose randomly the mouth to awake
            int idleMouths = idleMouthsList.Count();

            // if there is not a mouth to pick exit (no mouth awaken)
            if (idleMouths == 0)
                return;

            // get position of the active mouth, if any
            Mouth activeMouth = activeMouthsList.Count > 0 ? activeMouthsList[0] : null;

            // retrieve mouth to awake picking randomly
            int mouthPickedIndex = _dice.Next(0, idleMouths);
            Mouth awakenMouth = idleMouthsList[mouthPickedIndex];
            
            // bring awaken mouth to active mouth list
            activeMouthsList.Add(awakenMouth);
            idleMouthsList.RemoveAt(mouthPickedIndex);

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

            // set opening mouth time and mout open time!
            awakenMouth.OpeningTime = 0.3f;
            awakenMouth.MouthOpenTime = 0.15f;
            
            // awake!
            awakenMouth.AddSpriteEvent(new SpriteEvent(SpriteEventCode.awake));
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // aggiorna le frequenze e velocità delle bocche in maniera inversamente proporzionale agli hit points
            UpdateMouthSpeedParameters();

            //update timers and awake mouth if timer is expired
            _leftTimer += _elapsedTime;
            if (_leftTimer > _leftTimeToCall)
            {
                _leftTimer -= _leftTimeToCall;
                _leftTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
                AwakeMouth(_idleLeftMouths, _activeLeftMouths, true, -20, 250, 750, 200, new Vector2(1, 0));
            }

            _bottomTimer += _elapsedTime;
            if (_bottomTimer > _bottomTimeToCall)
            {
                _bottomTimer -= _bottomTimeToCall;
                _bottomTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
                AwakeMouth(_idleBottomMouths, _activeBottomMouths, false, 820, 50, 430, 140, new Vector2(0, -1));
            }

            _rightTimer += _elapsedTime;
            if (_rightTimer > _rightTimeToCall)
            {
                _rightTimer -= _rightTimeToCall;
                _rightTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
                AwakeMouth(_idleRightMouths, _activeRightMouths, true, 500, 250, 750, 200, new Vector2(-1, 0));
            }

            // call update on every active mouth!
            _activeLeftMouths.ForEach(m => m.Update(gameTime));
            _activeBottomMouths.ForEach(m => m.Update(gameTime));
            _activeRightMouths.ForEach(m => m.Update(gameTime));

            // idle mouths in active list are brought back to idle queues
            BringBackIdleMouthsToIdleQueque(_activeLeftMouths, _idleLeftMouths);
            BringBackIdleMouthsToIdleQueque(_activeBottomMouths, _idleBottomMouths);
            BringBackIdleMouthsToIdleQueque(_activeRightMouths, _idleRightMouths);

            // remove died mouths
            RemoveDiedMouths(_activeLeftMouths);
            RemoveDiedMouths(_activeBottomMouths);
            RemoveDiedMouths(_activeRightMouths);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _activeLeftMouths.ForEach(m => m.Draw(spriteBatch));
            _activeBottomMouths.ForEach(m => m.Draw(spriteBatch));
            _activeRightMouths.ForEach(m => m.Draw(spriteBatch));
        }

        private void RemoveDiedMouths(List<Mouth> activeMouths)
        {
            for (int i = 0; i < activeMouths.Count; i++)
            {
                if (activeMouths[i].State == MouthState.died)
                {
                    activeMouths.RemoveAt(i);
                    i--;
                }
            }
        }

        private void BringBackIdleMouthsToIdleQueque(List<Mouth> activeMouths, List<Mouth> idleMouths)
        {
            for (int i = 0; i < activeMouths.Count; i++)
            {
                if (activeMouths[i].State == MouthState.idle)
                {
                    idleMouths.Add(activeMouths[i]);
                    activeMouths.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateMouthSpeedParameters()
        {
            _approchingPeriodMin = 10;
            _approchingPeriodMax = 15;

            _approchingSpeed = 60;
            _rotatingSpeed = 1;
        }

    }

    
}

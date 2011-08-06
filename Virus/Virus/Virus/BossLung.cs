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
		opening,
		mouthOpenBefore,
		mouthOpenAfter,
		closing,
		postRotating,
		leaving,
		dyingMouthClosing,
		dying,
		fading,
		died
	}

	public class CentralMouth : CircularSprite
	{
		float _hitPoints;
		MouthState _state;
		BossLung _bossLung;  // reference to boss lung
		bool _left;

		public CentralMouth(Dictionary<string, Animation> animations, BossLung bossLung, bool left, float radius, float touchRadius)
			: base(animations, radius, touchRadius)
		{
			_hitPoints = 3;
			_state = MouthState.idle;
			_left = left;
			Position = new Vector2(-100, -100);
		}

		protected override void InitializePhysics()
		{
			_physicalPoint = new PhysicalMassSystemPoint();
		}


	}

	public class LateralMouth : CircularSprite
	{
		const float DELTA_SPACE = 60;
		const float DELTA_ANGLE = (float)Math.PI * (100f / 180f);

		static GameEventsManager _gameManager;
		static MonsterFactory _monsterFactory;

		static public GameEventsManager GameManager { set { _gameManager = value; } }
		static public MonsterFactory MonsterFactory { set { _monsterFactory = value; } }

		int _hitPoints;
		MouthState _state;
		Vector2 _leavingSpeed;

		// resi trasparenti dalla classe genitrice TimingBehaviouralSprite
		/*bool _blinking;
		float _blinkingTimer;
		bool _freezed;
		float _freezingTimer;

		float _timer;*/

		Vector2 _initialPosition;
		float _spitAngle;
		float _initialAngle;

		float _openingTime;
		float _mouthOpenTime;

		int _globulosToSpit = 4;
		int _spittedGlobulos;
		float _globulosSpeed = 250;

		public float OpeningTime { set { _openingTime = value; } }
		public float MouthOpenTime { set { _mouthOpenTime = value; } }
		public float GlobulosSpeed { set { _globulosSpeed = value; } }
		public int HitPoints { set { _hitPoints = value; } }

		public MouthState State
		{ get { return _state; } }

		public LateralMouth(Dictionary<string, Animation> animations, float radius, float touchRadius)
			: base(animations, radius, touchRadius)
		{
			_hitPoints = 3;
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
			Vector2 position = Position + 30 * speedVersor;
			_gameManager.ScheduleEvent(new GameEvent(t, GameEventType.createMouthBullet, _monsterFactory,
				new Object[] { position, speedVersor * _globulosSpeed }));
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			// handle death transition
			if (_hitPoints <= 0)
			{
				// state in which mouth is closed
				if (_state == MouthState.approching || _state == MouthState.leaving ||
					_state == MouthState.preRotating || _state == MouthState.rotating || _state == MouthState.postRotating)
				{
					ChangeAnimation("death");
					FramePerSecond = 10;
					_state = MouthState.dying;
				}
				// state in which mouth is open or partially opened
				else if (_state == MouthState.closing || _state == MouthState.opening ||
					_state == MouthState.mouthOpenAfter || _state == MouthState.mouthOpenBefore)
				{
					SetAnimationVerse(false);
					FramePerSecond = AnimationFrames / _openingTime;
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
				Freeze(2);
			}

			// handle standard state machine
			switch (_state)
			{
				case MouthState.idle:

					if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.awake)
					{
						// assign leaving speed
						_leavingSpeed = -Speed;

						_initialPosition = Position;
						Angle = (float)Math.Atan2(Speed.Y, Speed.X);
						_initialAngle = Angle;

						// initialize and change state
						_spittedGlobulos = 0;
						ChangeAnimation("opening");
						FramePerSecond = 0;

						ResetTimer();       // !!!!
						_touchable = true;
						_state = MouthState.approching;
					}
					break;

				case MouthState.approching:

					if (Vector2.Distance(Position, _initialPosition) >= DELTA_SPACE)
					{
						_state = MouthState.preRotating;
					}
					else
					{
						Move();
					}

					break;

				case MouthState.preRotating:

					if (Math.Abs(_initialAngle - Angle) >= DELTA_ANGLE / 2)
					{
						// reverse rotation asped for rotating state
						_rotationSpeed = -_rotationSpeed;

						// initialize variables for first mouth opening
						FramePerSecond = AnimationFrames / _openingTime;
						_state = MouthState.opening;
					}
					else
					{
					   Rotate();
					}

					break;

				case MouthState.opening:

					Animate();

					if (AnimationFinished())
					{
						SetAnimationVerse(false);
						FramePerSecond = 0;
						StartTimer(_mouthOpenTime / 2);
						_state = MouthState.mouthOpenBefore;
					}

					break;

				case MouthState.mouthOpenBefore:

					if (Exceeded())
					{
						RestartTimer(_mouthOpenTime / 2);
						_spittedGlobulos++;
						_spitAngle = Angle;
						FireGlobulo(gameTime.TotalGameTime);
						_state = MouthState.mouthOpenAfter;
					}

					break;

				case MouthState.mouthOpenAfter:

					if (Exceeded())
					{
						FramePerSecond = AnimationFrames / _openingTime;
						_state = MouthState.closing;
					}

					break;

				case MouthState.closing:

					Animate();

					if (AnimationFinished())
					{
						SetAnimationVerse(true);

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

					Rotate();

					if (Math.Abs(Angle - _spitAngle) >= DELTA_ANGLE / (_globulosToSpit - 1))
					{
						FramePerSecond = AnimationFrames / _openingTime;
						_state = MouthState.opening;
					}

					break;

				case MouthState.postRotating:

					if (Math.Abs(Angle - _initialAngle) < 0.01)     // si potrebbe usare il verso della velocità angolare per fare un controllo meno rischioso
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
						_touchable = false;
						_state = MouthState.idle;
					}
					else
					{
						Move();
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
						RestartTimer(2);
						FadeSpeed = 0.5f;
						_touchable = false;
						_state = MouthState.fading;
					}

					break;

				case MouthState.fading:

					if (Exceeded())
					{
						Fade();
					}
					else
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

		/*public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			// hanlde blinking
			if (_blinking)
			{
				_blinkingTimer += _elapsedTime;
				if (_blinkingTimer < 0.2f)
				{
					Blink();
				}
				else
				{
					Tint = Color.White;
					_blinkingTimer = 0;
					_blinking = false;
				}
			}

			// hanlde freezing
			if (_freezed)
			{
				_freezingTimer += _elapsedTime;
				if (_freezingTimer > 2)
				{
					_freezingTimer = 0;
					_freezed = false;
				}
			}

			// handle death transition
			if (_hitPoints <= 0)
			{
				// state in which mouth is closed
				if (_state == MouthState.approching || _state == MouthState.leaving ||
					_state == MouthState.preRotating || _state == MouthState.rotating || _state == MouthState.postRotating)
				{
					ChangeAnimation("death");
					FramePerSecond = 10;
					_state = MouthState.dying;
				}
				// state in which mouth is open or partially opened
				else if (_state == MouthState.closing || _state == MouthState.opening ||
					_state == MouthState.mouthOpenAfter || _state == MouthState.mouthOpenBefore)
				{
					SetAnimationVerse(false);
					FramePerSecond = AnimationFrames / _openingTime;
					_state = MouthState.dyingMouthClosing;
				}
			}
			// hanlde hit
			else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.fingerHit)
			{
				_hitPoints--;
				BlinkingFrequency = 30;
				BlinkingTint = Color.Transparent;
				_blinking = true;
			}
			// handle bomb
			else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.bombHit)
			{
				_freezed = true;
				_freezingTimer = 0;
			}

			// handle standard state machine
			switch (_state)
			{
				case MouthState.idle:

					if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.awake)
					{
						// assign leaving speed
						_leavingSpeed = -Speed;

						_initialPosition = Position;
						Angle = (float)Math.Atan2(Speed.Y, Speed.X);
						_initialAngle = Angle;

						// initialize and change state
						_spittedGlobulos = 0;
						ChangeAnimation("opening");
						FramePerSecond = 0;

						_timer = 0;
						_touchable = true;
						_state = MouthState.approching;
					}
					break;

				case MouthState.approching:

					if (Vector2.Distance(Position, _initialPosition) >= DELTA_SPACE)
					{
						_state = MouthState.preRotating;
					}
					else
					{
						if (!_freezed)
							Move();
					}
						
					break;

				case MouthState.preRotating:

					if (Math.Abs(_initialAngle - Angle) >= DELTA_ANGLE / 2)
					{
						// reverse rotation asped for rotating state
						_rotationSpeed = -_rotationSpeed;

						// initialize variables for first mouth opening
						FramePerSecond = AnimationFrames / _openingTime ;
						_state = MouthState.opening;
					}
					else
					{
						if (!_freezed)
							Rotate();
					}

					break;

				case MouthState.opening:

					if (!_freezed)
						Animate();

					if (AnimationFinished())
					{
						SetAnimationVerse(false);
						FramePerSecond = 0;
						_state = MouthState.mouthOpenBefore;
					}

					break;

				case MouthState.mouthOpenBefore:

					if (!_freezed)
						_timer += _elapsedTime;

					if (_timer >= _mouthOpenTime / 2)
					{
						_timer = 0;
						_spittedGlobulos++;
						_spitAngle = Angle;
						FireGlobulo(gameTime.TotalGameTime);
						_state = MouthState.mouthOpenAfter;
					}

					break;

				case MouthState.mouthOpenAfter:

					if (!_freezed)
						_timer += _elapsedTime;

					if (_timer >= _mouthOpenTime / 2)
					{
						FramePerSecond = AnimationFrames / _openingTime;
						_state = MouthState.closing;
					}

					break;

				case MouthState.closing:

					if (!_freezed)
						Animate();

					if (AnimationFinished())
					{
						SetAnimationVerse(true);
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

					if (!_freezed)
						Rotate();

					if (Math.Abs(Angle - _spitAngle) >= DELTA_ANGLE / (_globulosToSpit - 1))
					{
						FramePerSecond = AnimationFrames / _openingTime;
						_state = MouthState.opening;
					}

					break;

				case MouthState.postRotating:

					if (Math.Abs(Angle - _initialAngle) < 0.01)     // si potrebbe usare il verso della velocità angolare per fare un controllo meno rischioso
					{
						Speed = _leavingSpeed;
						_state = MouthState.leaving;
					}
					else
					{
						if (!_freezed)
							Rotate();
					}

					break;

				case MouthState.leaving:

					if (Math.Abs(Vector2.Distance(Position, _initialPosition)) < 3)
					{
						_touchable = false;
						_state = MouthState.idle;
					}
					else
					{
						if (!_freezed)
							Move();
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
						_timer = 0;
						FadeSpeed = 0.5f;
						_touchable = false;
						_state = MouthState.fading;
					}

					break;

				case MouthState.fading:

					_timer += _elapsedTime;

					if (_timer < 2)
					{
						Fade();
					}
					else
					{
						_state = MouthState.died;
					}

					break;

				case MouthState.died:
					break;

				default:
					break;
			}
		}*/

	}

	public enum BossLungState
	{
		approaching,
		standing,
		vomiting,
		vomitingback,
		dying,
		died
	}

	public class BossLung : RectangularSprite
	{
		int _hitPoints;
		int _reverseVomitingHitPoints = 0;
		BossLungState _state;

		float _timer = 0;

		int _diedMouthCounter = 0;

		public bool SpecialMoveHit { get; set; }

		bool _freezed;
		float _freezingTimer = 0;
		bool _blinking;
		float _blinkingTimer;

		bool _bSpecialMoveFlag = false;

		AnimationFactory _mouthAnimationFactory;

		// mouth structures
		// lateral mouths
		List<LateralMouth> _idleLeftMouths = new List<LateralMouth>();
		List<LateralMouth> _idleBottomMouths = new List<LateralMouth>();
		List<LateralMouth> _idleRightMouths = new List<LateralMouth>();

		List<LateralMouth> _activeLeftMouths = new List<LateralMouth>();
		List<LateralMouth> _activeBottomMouths = new List<LateralMouth>();
		List<LateralMouth> _activeRightMouths = new List<LateralMouth>();

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

		// central mouths
		List<CentralMouth> _centralMouths = new List<CentralMouth>();

		Random _dice  = new Random(DateTime.Now.Millisecond);

		public BossLung(Dictionary<string,Animation> animations, float width, float height, float touchWidth, float touchHeight, AnimationFactory mouthAnimationFactory, GameEventsManager gm, MonsterFactory mf)
			: base(animations, width, height, touchWidth, touchHeight)
		{
			_touchable = true;

			_state = BossLungState.approaching;

			Position = new Vector2(240, -100);
			Speed = new Vector2(0, 20);

			_mouthAnimationFactory = mouthAnimationFactory;

			// initialize lateral mouths
			LateralMouth.GameManager = gm;
			LateralMouth.MonsterFactory = mf;

			int i = 0;
			int totalQueueMouths = 10;

			// create lateral mouths
			for (i = 0; i < totalQueueMouths; i++)
			{
				 _idleLeftMouths.Add(new LateralMouth(_mouthAnimationFactory.CreateAnimations("Mouth"), 40, 40));
			}
				
			for (i = 0; i < totalQueueMouths; i++)
			{
				_idleBottomMouths.Add(new LateralMouth(_mouthAnimationFactory.CreateAnimations("Mouth"), 40, 40));
			}
				
			for (i = 0; i < totalQueueMouths; i++)
			{
				_idleRightMouths.Add(new LateralMouth(_mouthAnimationFactory.CreateAnimations("Mouth"), 40, 40));
			}
				
			_leftTimeToCall = 7.25f;
			_bottomTimeToCall = 7.25f;
			_rightTimeToCall = 7.25f;

			// initialize central mouths
			// create central mouths
			for (i = 0; i < 2; i++)
			{
				_centralMouths.Add(new CentralMouth(_mouthAnimationFactory.CreateAnimations("CentralMouth"), this, i == 0, 40, 40));
			}
		}

		protected override void InitializePhysics()
		{
			_physicalPoint = new PhysicalMassSystemPoint();
		}
		 
		/*private void AwakeMouth(List<Mouth> idleMouthsList, List<Mouth> activeMouthsList, bool vertical, float fixedPosition, float variablePositionMin, float variablePositionMax, float distance, Vector2 speedVersor)
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
			awakenMouth.MouthOpenTime = 0.3f;
			
			// awake!
			awakenMouth.AddSpriteEvent(new SpriteEvent(SpriteEventCode.awake));
		}*/

		private void AwakeMouth(List<LateralMouth> idleMouthsList, List<LateralMouth> activeMouthsList, bool vertical, float fixedPosition, float variablePositionMin, float variablePositionMax, float distance, Vector2 speedVersor)
		{
			// chose randomly the mouth to awake
			int idleMouths = idleMouthsList.Count();

			// if there is not a mouth to pick exit (no mouth awaken)
			if (idleMouths == 0)
				return;

			// get position of the active mouth, if any
			LateralMouth activeMouth = activeMouthsList.Count > 0 ? activeMouthsList[0] : null;

			// retrieve mouth to awake picking randomly
			int mouthPickedIndex = _dice.Next(0, idleMouths);
			LateralMouth awakenMouth = idleMouthsList[mouthPickedIndex];

			// bring awaken mouth to active mouth list
			activeMouthsList.Add(awakenMouth);
			idleMouthsList.RemoveAt(mouthPickedIndex);

			// set position and speed of chosen mouth and awake it!
			float position;

			// position is always taken randomly
			position = (float)_dice.RandomDouble(variablePositionMin, variablePositionMax);

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
			awakenMouth.MouthOpenTime = 0.3f;

			// awake!
			awakenMouth.AddSpriteEvent(new SpriteEvent(SpriteEventCode.awake));
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			// handle freezed
			if (_freezed)
			{
				_freezingTimer += _elapsedTime;
				if (_freezingTimer > 2.0f)
				{
					_freezingTimer = 0;
					_freezed = false;
				}
			}

			// hanlde blinking
			if (_blinking)
			{
				_blinkingTimer += _elapsedTime;
				if (_blinkingTimer < 0.2f)
				{
					Blink();
				}
				else
				{
					Tint = Color.White;
					_blinkingTimer = 0;
					_blinking = false;
				}
			}


			if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.bombHit)
			{
				_activeLeftMouths.ForEach(m => m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.bombHit)));
				_activeBottomMouths.ForEach(m => m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.bombHit)));
				_activeRightMouths.ForEach(m => m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.bombHit)));
				_freezed = true;
				_freezingTimer = 0;

				// cancella l'animazione vomiting
			}

			HandleLateralMouths(gameTime);
			Animate();

			switch (_state)
			{
				case BossLungState.approaching:
					Move();

					if (Position.Y >= 128)
						_state = BossLungState.standing;
						
					break;

				case BossLungState.standing:

					_timer += _elapsedTime;

					_bSpecialMoveFlag = false;

					if (_timer >= 10 && IsAnimationBegin())
					{
						_timer -= 10f;
						ChangeAnimation("vomit");
						FramePerSecond = 3.5f;
						SetAnimationVerse(true);
						_state = BossLungState.vomiting;
					}

					break;

				case BossLungState.vomiting:

					// condition to reverse vomiting
					if (FrameIndex() <= 5)
					{
						if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.fingerHit)
						{
							DelayAnimation();
							BlinkingFrequency = 30;
							BlinkingTint = Color.Transparent;
							_blinking = true;

							_reverseVomitingHitPoints++;
							if (_reverseVomitingHitPoints >= 3)
							{
								SetAnimationVerse(false);
								FramePerSecond = 3.5f;
								_reverseVomitingHitPoints = 0;
								_state = BossLungState.vomitingback;

								break;
							}
						}
						else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.bombHit)
						{
							SetAnimationVerse(false);
							FramePerSecond = 3.5f;
							_reverseVomitingHitPoints = 0;
							_state = BossLungState.vomitingback;
							break;
						}
					}
				   
					if (FrameIndex() == 5)
					{
						FramePerSecond = 2.0f;
					}
					else if (FrameIndex() == 6)
					{
						FramePerSecond = 10.0f;
					}
					else if (FrameIndex() == 24 && !_bSpecialMoveFlag)
					{
						SpecialMoveHit = true;
						_bSpecialMoveFlag = true;
					}
					else if (AnimationFinished())
					{
						ChangeAnimation("main");
						_reverseVomitingHitPoints = 0;
						FramePerSecond = 3.5f;
						_state = BossLungState.standing;
					}

					break;

				case BossLungState.vomitingback:

					if (AnimationFinished())
					{
						if (_diedMouthCounter < 30)
						{
							ChangeAnimation("main");
							FramePerSecond = 3.5f;
							_state = BossLungState.standing;
						}
						else
						{
							ChangeAnimation("death");
							FramePerSecond = 3.5f;
							_state = BossLungState.dying;
						}
					}

					break;

				case BossLungState.dying:

					if (AnimationFinished())
					{
						_state = BossLungState.died;
					}

					break;

				default:
					break;
			}   
		}

		private void HandleLateralMouths(GameTime gameTime)
		{
			// aggiorna le frequenze e velocità delle bocche in maniera inversamente proporzionale agli hit points
			UpdateMouthSpeedParameters();

			//update timers and awake mouth if timer is expired
			if (!_freezed)
				_leftTimer += _elapsedTime;
			if (_leftTimer > _leftTimeToCall)
			{
				_leftTimer -= _leftTimeToCall;
				_leftTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
				AwakeMouth(_idleLeftMouths, _activeLeftMouths, true, -30, 250, 750, 200, new Vector2(1, 0));
			}

			if (!_freezed)
				_bottomTimer += _elapsedTime;
			if (_bottomTimer > _bottomTimeToCall)
			{
				_bottomTimer -= _bottomTimeToCall;
				_bottomTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
				AwakeMouth(_idleBottomMouths, _activeBottomMouths, false, 830, 50, 430, 140, new Vector2(0, -1));
			}

			if (!_freezed)
				_rightTimer += _elapsedTime;
			if (_rightTimer > _rightTimeToCall)
			{
				_rightTimer -= _rightTimeToCall;
				_rightTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
				AwakeMouth(_idleRightMouths, _activeRightMouths, true, 510, 250, 750, 200, new Vector2(-1, 0));
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

		private void RemoveDiedMouths(List<LateralMouth> activeMouths)
		{
			for (int i = 0; i < activeMouths.Count; i++)
			{
				if (activeMouths[i].State == MouthState.died)
				{
					activeMouths.RemoveAt(i);
					_diedMouthCounter++;
					i--;
				}
			}
		}

		private void BringBackIdleMouthsToIdleQueque(List<LateralMouth> activeMouths, List<LateralMouth> idleMouths)
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
			_approchingPeriodMin = 5.25f;
			_approchingPeriodMax = 5.25f;

			_approchingSpeed = 175;
			_rotatingSpeed = 1.5f;
		}

		// deve diventare un metodo di una interfaccia che i boss ereditano e che ogni boss deve implementare
		public void HandleUserTouch(Vector2 _touchPoint, ref int enemiesHit)
		{
			// contact with main blob
			if (Touched(_touchPoint))
			{
				enemiesHit++;
				this.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit));
			}

			// contact with mouths
			HandleFingerContactWidthMouthQueque(_touchPoint, _activeLeftMouths, ref enemiesHit);
			HandleFingerContactWidthMouthQueque(_touchPoint, _activeBottomMouths, ref enemiesHit);
			HandleFingerContactWidthMouthQueque(_touchPoint, _activeRightMouths, ref enemiesHit);
		}

		private void HandleFingerContactWidthMouthQueque(Vector2 touchPoint, List<LateralMouth> Mouths, ref int enemiesHit)
		{
			foreach (LateralMouth m in Mouths)
			{
				if (m.Touched(touchPoint))
				{
					m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit));
					enemiesHit++;
				}
			}
		}
	}
}

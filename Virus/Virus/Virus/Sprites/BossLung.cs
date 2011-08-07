using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virus.Sprites;

namespace Virus
{
	public enum BossLungState
	{
		approaching,
		standing,
		vomiting,
		vomitingback,
		dying,
		died
	}

	public class BossLung :  RectangularSprite
	{
		#region privare members states

		int _reverseVomitingHitPoints = 0;
		BossLungState _state = BossLungState.approaching;
		int _diedMouthCounter = 0;
		bool _mouthsFreezed = false;
		float _mouthFreezedTimer = 0;
		bool _specialMoveFlag = false;
		bool _lateralMouthCall = false;
		Random _dice = new Random(DateTime.Now.Millisecond);

		#endregion

		#region private members mouths relationships

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
		float _approchingPeriodMin = 6.00f;
		float _approchingPeriodMax = 8.00f;
		float _approchingSpeed = 175;
		float _rotatingSpeed = 1.5f;

		float _lateralMouthsTimer = 0;
		float _lateralMouthsTimeToCall;
		int _mouthsPerQueue = 10;

		// central mouths
		List<CentralMouth> _centralMouths = new List<CentralMouth>();

		#endregion

		#region properties

		public bool SpecialMoveHit { get; set; }

		#endregion

		#region constructors

		public BossLung(Dictionary<string, Animation> animations, float width, float height, float touchWidth, float touchHeight, AnimationFactory mouthAnimationFactory, GameEventsManager gm, MonsterFactory mf)
			: base(animations, width, height, touchWidth, touchHeight)
		{
			_touchable = true;

			Position = new Vector2(240, -100);
			Speed = new Vector2(0, 20);

			_mouthAnimationFactory = mouthAnimationFactory;

			// initialize lateral mouths
			Mouth.GameManager = gm;
			Mouth.MonsterFactory = mf;

			int i = 0;
			// create lateral mouths
			for (i = 0; i < _mouthsPerQueue; i++)
			{
				_idleLeftMouths.Add(new LateralMouth(_mouthAnimationFactory.CreateAnimations("Mouth"), 40, 40));
			}

			for (i = 0; i < _mouthsPerQueue; i++)
			{
				_idleBottomMouths.Add(new LateralMouth(_mouthAnimationFactory.CreateAnimations("Mouth"), 40, 40));
			}

			for (i = 0; i < _mouthsPerQueue; i++)
			{
				_idleRightMouths.Add(new LateralMouth(_mouthAnimationFactory.CreateAnimations("Mouth"), 40, 40));
			}

			// lateral mouths will be called when central mouths are all dead!
			_lateralMouthsTimeToCall = float.PositiveInfinity;

			// initialize central mouths
			// create central mouths
			for (i = 0; i < 2; i++)
			{
				_centralMouths.Add(new CentralMouth(_mouthAnimationFactory.CreateAnimations("CentralMouth"), this, i == 0, 40, 40));
			}
		}

		#endregion

		#region physics initialization

		protected override void InitializePhysics()
		{
			_physicalPoint = new PhysicalMassSystemPoint();
		}

		#endregion

		#region auxilary methods

		private void AwakeLateralMouth(List<LateralMouth> idleMouthsList, List<LateralMouth> activeMouthsList, bool vertical, float fixedPosition, float variablePositionMin, float variablePositionMax, float distance, Vector2 speedVersor)
		{
			// chose randomly the mouth to awake
			int idleMouths = idleMouthsList.Count();

			// if there is not a mouth to pick exit (no mouth awaken)
			if (idleMouths == 0)
				return;

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

			// awake!
			awakenMouth.AddSpriteEvent(new SpriteEvent(SpriteEventCode.awake));
		}

		private void HandleCentralMouths(GameTime gameTime)
		{
			_centralMouths.ForEach(cm => cm.Update(gameTime));
		}

		private void HandleLateralMouths(GameTime gameTime)
		{
			if (_lateralMouthCall && !_mouthsFreezed)
				_lateralMouthsTimer += _elapsedTime;
			if (_lateralMouthsTimer > _lateralMouthsTimeToCall)
			{
				_lateralMouthsTimer -= _lateralMouthsTimeToCall;
				_lateralMouthsTimeToCall = (float)_dice.RandomDouble(_approchingPeriodMin, _approchingPeriodMax);
				AwakeLateralMouth(_idleLeftMouths, _activeLeftMouths, true, -50, 250, 750, 200, new Vector2(1, 0));
				AwakeLateralMouth(_idleBottomMouths, _activeBottomMouths, false, 850, 50, 430, 140, new Vector2(0, -1));
				AwakeLateralMouth(_idleRightMouths, _activeRightMouths, true, 530, 250, 750, 200, new Vector2(-1, 0));
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

		#endregion

		#region interface with user inputs

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

			HandleFingerContactWithCentralMouths(_touchPoint, _centralMouths, ref enemiesHit);
		}

		private void HandleFingerContactWithCentralMouths(Vector2 touchPoint, List<CentralMouth> Mouths, ref int enemiesHit)
		{
			foreach (CentralMouth m in Mouths)
			{
				if (m.Touched(touchPoint))
				{
					m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit));
					enemiesHit++;
				}
			}
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

		#endregion

		#region update

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			// handle mouth freezed timer
			if (_mouthsFreezed)
			{
				_mouthFreezedTimer += _elapsedTime;
				if (_mouthFreezedTimer > 2)
				{
					_mouthFreezedTimer = 0;
					_mouthsFreezed = false;
				}
			}

			if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.bombHit)
			{
				_activeLeftMouths.ForEach(m => m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.bombHit)));
				_activeBottomMouths.ForEach(m => m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.bombHit)));
				_activeRightMouths.ForEach(m => m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.bombHit)));
				_centralMouths.ForEach(m => m.AddSpriteEvent(new SpriteEvent(SpriteEventCode.bombHit)));
				_mouthsFreezed = true;
				_mouthFreezedTimer = 0;
			}

			// if central mouths are died, awake lateral mouths
			if (!_lateralMouthCall && _centralMouths.All(cm => cm.State == MouthState.died))
			{
				_lateralMouthsTimeToCall = 3;
				_lateralMouthCall = true;
			}

			HandleLateralMouths(gameTime);
			HandleCentralMouths(gameTime);

			Animate();

			switch (_state)
			{
				case BossLungState.approaching:

					Move();

					if (Position.Y >= 128)
					{
						_state = BossLungState.standing;
						RestartTimer(13);
					}

					break;

				case BossLungState.standing:

					_specialMoveFlag = false;

					if (Exceeded() && IsAnimationBegin())
					{
						Recycle();
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
							StartBlinking(0.3f, 30, Color.Transparent);
							_reverseVomitingHitPoints++;

							if (_reverseVomitingHitPoints >= 3)
							{
								_reverseVomitingHitPoints = 0;
								SetAnimationVerse(false);
								FramePerSecond = 3.5f;
								_state = BossLungState.vomitingback;

								break;
							}
						}
						else if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.bombHit)
						{
							_reverseVomitingHitPoints = 0;
							SetAnimationVerse(false);
							FramePerSecond = 3.5f;
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
					else if (FrameIndex() == 24 && !_specialMoveFlag)
					{
						SpecialMoveHit = true;
						_specialMoveFlag = true;
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
						if (_diedMouthCounter < 3 * _mouthsPerQueue)
						{
							ChangeAnimation("main");
							FramePerSecond = 3.5f;
							_state = BossLungState.standing;
						}
						else
						{
							_centralMouths.RemoveAt(0);
							_centralMouths.RemoveAt(0);
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

		#endregion

		#region draw

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			_activeLeftMouths.ForEach(lm => lm.Draw(spriteBatch));
			_activeBottomMouths.ForEach(lm => lm.Draw(spriteBatch));
			_activeRightMouths.ForEach(lm => lm.Draw(spriteBatch));

			_centralMouths.ForEach(cm => cm.Draw(spriteBatch));
		}

		#endregion
	}
}

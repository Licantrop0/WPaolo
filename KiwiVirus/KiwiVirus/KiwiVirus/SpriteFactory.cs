using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Kiwi
{
	public class Level1DifficultyPackEnemies
	{
		public TimeSpan SimpleEnemySchedulingTimeIntervalMin { get; set; }
		public TimeSpan SimpleEnemySchedulingTimeIntervalMax { get; set; }

		public TimeSpan SimpleEnemyCreationTimeIntervalMin { get; set; }
		public TimeSpan SimpleEnemyCreationTimeIntervalMax { get; set; }

		public float TimeToReachMin { get; set; }
		public float TimeToReachMax { get; set; }

		public int NumberOfMonstersMin { get; set; }
		public int NumberOfMonstersMax { get; set; }

		public Level1DifficultyPackEnemies (TimeSpan schedTimeMin, TimeSpan schedTimeMax, TimeSpan createMin, TimeSpan createMax,
			float timeToReachMin, float timeToReachMax, int numMonstersMin, int numMonstersMax)
		{
			SimpleEnemySchedulingTimeIntervalMin = schedTimeMin;
			SimpleEnemySchedulingTimeIntervalMax = schedTimeMax;

			SimpleEnemyCreationTimeIntervalMin = createMin;
			SimpleEnemyCreationTimeIntervalMax = createMax;

			TimeToReachMin = timeToReachMin;
			TimeToReachMax = timeToReachMax;

			NumberOfMonstersMin = numMonstersMin;
			NumberOfMonstersMax = numMonstersMax;
		}
	}

	public class SpriteFactory : GameEventHandler
	{
		protected Random _dice = new Random(DateTime.Now.Millisecond);
		protected Vector2 _virusPosition;

		const int DELTA_BORDER = 15;
		const int W = 480;
		const int H = 800;

		public SpriteFactory(GameEventsManager eventManager)
			:base(eventManager)
		{

		}

		// Initializing methods
		public Vector2 VirusPosition
		{ set { _virusPosition = value; } }


		protected double DoubleDiceResult(double min, double max)
		{
			return min + _dice.NextDouble() * (max - min);
		}
		
		protected Vector2 SetSpriteInitialPositionOnScreenBorder()
		{
			int p1 = W      + 2 * DELTA_BORDER;
			int p2 = p1 + H + 2 * DELTA_BORDER;
			int p3 = p2 + W + 2 * DELTA_BORDER;
			int p4 = p3 + H + 2 * DELTA_BORDER;

			int borderPosition = _dice.Next(1, p4 + 1);

			if (borderPosition < p1)
			{
				return new Vector2(borderPosition - DELTA_BORDER, -DELTA_BORDER);
			}
			else if (borderPosition >= p1 && borderPosition < p2)
			{
				return new Vector2(W + DELTA_BORDER, (borderPosition - p1) - DELTA_BORDER);
			}
			else if (borderPosition >= p2 && borderPosition < p3)
			{
				return new Vector2(H + DELTA_BORDER, p3 - borderPosition - DELTA_BORDER);
			}
			else if (borderPosition >= p3)
			{
				return new Vector2(-DELTA_BORDER, p4 - borderPosition - DELTA_BORDER);
			}
			else
			{
				throw new Exception("Exception in MonsterFactory::SetEnemyInitialPositionOnScreenBorder()");
			}
		}

		protected Vector2 SetSpriteInitialPositionOnTopOrBotBorder()
		{
			int p1 = W + 2 * DELTA_BORDER;
			int p2 = p1 + W + 2 * DELTA_BORDER;

			int borderPosition = _dice.Next(1, p2 + 1);

			if (borderPosition < p1)
			{
				return new Vector2(borderPosition - DELTA_BORDER, -DELTA_BORDER);
			}
			else
			{
				return new Vector2(p2 - borderPosition - DELTA_BORDER, H + DELTA_BORDER); ;
			}
		}
	} 

	public class MonsterFactory : SpriteFactory
	{
		List<WhiteGlobulo> _enemies;                // reference to the game enemies list
		List<BossLung> _bossContainer;                   

		// valori cablati
		const float VIRUS_RADIUS = 37;
		float ENEMY_RADIUS = 29;
		float ENEMY_TOUCH_RADIUS = 34;

		// textures
		Texture2D _constantSpeedMonsterTexture;
		Texture2D _acceleratedMonsterTexture;
		Texture2D _orbitalMonsterTexture;
		Texture2D _bossLungTexture;
		Texture2D _bossLungMouthOpeningTexture;
		Texture2D _bossLungMouthDeathTexture;

		// difficulty parameters
		protected TimeSpan _simpleEnemySchedulingTimeIntervalMin;
		protected TimeSpan _simpleEnemySchedulingTimeIntervalMax;

		protected TimeSpan _simpleEnemyCreationTimeIntervalMin;
		protected TimeSpan _simpleEnemyCreationTimeIntervalMax;

		protected float _timeToReachMin;
		protected float _timeToReachMax;

		protected int _numberOfMonstersMin;
		protected int _numberOfMonstersMax;

		public MonsterFactory(GameEventsManager eventManager, List<WhiteGlobulo> enemies,  List<BossLung> bossContainer)
			:base(eventManager)
		{
			_enemies = enemies;
			_bossContainer = bossContainer;
		}

		// Initializing methods
		public Texture2D SimpleEnemyTexture
		{ set { _constantSpeedMonsterTexture = value; } }

		public Texture2D AcceleratedEnemyTexture
		{ set { _acceleratedMonsterTexture = value; } }

		public Texture2D OrbitalMonsterTexture
		{ set { _orbitalMonsterTexture = value; } }

		public Texture2D BossLungTexture
		{ set { _bossLungTexture = value; } }

		public Texture2D BossLungMouthOpeningTexture
		{ set { _bossLungMouthOpeningTexture = value; } }

		public Texture2D BossLungMouthDeathTexture
		{ set { _bossLungMouthDeathTexture = value; } }

		public void SetDifficulty(Level1DifficultyPackEnemies difficulty)
		{
			_simpleEnemySchedulingTimeIntervalMin = difficulty.SimpleEnemySchedulingTimeIntervalMin;
			_simpleEnemySchedulingTimeIntervalMax = difficulty.SimpleEnemySchedulingTimeIntervalMax;

			_simpleEnemyCreationTimeIntervalMin = difficulty.SimpleEnemyCreationTimeIntervalMin;
			_simpleEnemyCreationTimeIntervalMax = difficulty.SimpleEnemyCreationTimeIntervalMax;

			_timeToReachMin = difficulty.TimeToReachMin;
			_timeToReachMax = difficulty.TimeToReachMax;

			_numberOfMonstersMin = difficulty.NumberOfMonstersMin;
			_numberOfMonstersMax = difficulty.NumberOfMonstersMax;
		}

		public override void HandleEvent(GameEvent gameEvent)
		{
			TimeSpan actualTime = gameEvent.GameTimer;

			switch (gameEvent.EventType)
			{
				case GameEventType.undefined:
					break;

				case GameEventType.createSimpleEnemy:
					CreateSimpleEnemy();
					break;

				case GameEventType.createBouncingEnemy:
					CreateBouncingEnemy((Vector2)gameEvent.Params[0], (Vector2)gameEvent.Params[1]);
					break;

				case GameEventType.createAcceleratedEnemy:
					CreateAcceleratedEnemy();
					break;

				case GameEventType.createOrbitalEnemy:
					CreateOrbitalEnemy();
					break;

				case GameEventType.createBossLung:
					CreateBossLung();
					break;

				case GameEventType.scheduleSimpleEnemyCreation:
					ScheduleSimpleEnemyCreation(actualTime);
					break;

				case GameEventType.scheduleAcceleratedEnemyCreation:
					ScheduleAcceleratedEnemyCreation(actualTime);
					break;

				case GameEventType.scheduleOrbitalEnemyCreation:
					ScheduleOrbitalEnemyCreation(actualTime);
					break;

				case GameEventType.ChangeLevel1Difficulty:
					SetDifficulty((Level1DifficultyPackEnemies)gameEvent.Params[0]);
					break;

				default:
					throw new Exception("W Blaze Baley!");
			}
		}

		private void ScheduleSimpleEnemyCreation(TimeSpan actualTime)
		{
			int numOfMonsterToCreate = _dice.Next(_numberOfMonstersMin, _numberOfMonstersMax + 1);
			double deltaT = 0;

			// schedule monster creation
			for (int i = 0; i < numOfMonsterToCreate; i++)
			{
				deltaT = DoubleDiceResult(_simpleEnemyCreationTimeIntervalMin.TotalMilliseconds, _simpleEnemyCreationTimeIntervalMax.TotalMilliseconds);

				_eventsManager.ScheduleEvent(new GameEvent(actualTime + TimeSpan.FromMilliseconds(deltaT), GameEventType.createSimpleEnemy, this));
			}

			// schedule monster creation schedule
			deltaT = DoubleDiceResult(_simpleEnemySchedulingTimeIntervalMin.TotalMilliseconds, _simpleEnemySchedulingTimeIntervalMax.TotalMilliseconds);

			_eventsManager.ScheduleEvent(new GameEvent(actualTime + TimeSpan.FromMilliseconds(deltaT), GameEventType.scheduleSimpleEnemyCreation, this));
		}

		// da rivedere
		private void ScheduleAcceleratedEnemyCreation(TimeSpan actualTime)
		{
			// schedule creation of accelerated monster
			GameEvent ge = new GameEvent(actualTime + TimeSpan.FromSeconds(0.2), GameEventType.createAcceleratedEnemy, this);
			_eventsManager.ScheduleEvent(ge);

			// schedule accelerated monster creation schedule
			double deltaT = (2000 +
				_dice.NextDouble() * (5000 - 2000));
			ge = new GameEvent(actualTime + TimeSpan.FromMilliseconds(deltaT), GameEventType.scheduleAcceleratedEnemyCreation, this);
			_eventsManager.ScheduleEvent(ge);
		}

		// da rivedere
		private void ScheduleOrbitalEnemyCreation(TimeSpan actualTime)
		{
			// temp! schedule creation of orbital monster
			GameEvent ge = new GameEvent(actualTime + TimeSpan.FromSeconds(1.0), GameEventType.createOrbitalEnemy, this);
			_eventsManager.ScheduleEvent(ge);

			// schedule orbital monster creation schedule
			double deltaT = (3000 +
				_dice.NextDouble() * (7000 - 3000));
			ge = new GameEvent(actualTime + TimeSpan.FromMilliseconds(deltaT), GameEventType.scheduleOrbitalEnemyCreation, this);
			_eventsManager.ScheduleEvent(ge);
		}

		private void CreateSimpleEnemy()
		{
			// create simple enemy
			//WhiteGlobulo enemy = new WhiteGlobulo(CreateAnimations(AnimationDB("<nomeSprite>")));

			Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
			Animation mainAnimation = new Animation(_constantSpeedMonsterTexture, 7, true);
			animations.Add("main", mainAnimation);

			WhiteGlobulo enemy = new WhiteGlobulo(animations, ENEMY_RADIUS, ENEMY_TOUCH_RADIUS);
			Vector2 enemyPosition = SetSpriteInitialPositionOnScreenBorder();
			enemy.Position = enemyPosition;

			// extract time to reach
			 float timeToReach = (float)DoubleDiceResult(_timeToReachMin, _timeToReachMax);

			// calculate speed modulus
			float distance = Vector2.Distance(enemyPosition, _virusPosition) - enemy.Radius - VIRUS_RADIUS; 
			float speedModulus = distance / timeToReach;

			// set enemy speed
			enemy.Speed = Vector2.Normalize(_virusPosition - enemy.Position) * speedModulus;

			_enemies.Add(enemy);
		}

		private void CreateBossLung()
		{
			// create boss
			Dictionary<string, Animation> bossAnimations = new Dictionary<string, Animation>();
			Animation bossMainAnimation = new Animation(_bossLungTexture, 1, true);
			bossAnimations.Add("main", bossMainAnimation);

			Dictionary<string, Animation> mouthAnimations = new Dictionary<string, Animation>();
			Animation animation = new Animation(_bossLungMouthOpeningTexture, 8, false);
			mouthAnimations.Add("opening", animation);
			animation = new Animation(_bossLungMouthDeathTexture, 8, false);
			mouthAnimations.Add("death", animation);

			BossLung boss = new BossLung(bossAnimations, 240, 80, 240, 80, mouthAnimations, _eventsManager, this)
			{
				Position = new Vector2(240, 40),
			};

			_bossContainer.Add(boss);
		}

		// da rivedere
		private void CreateAcceleratedEnemy()
		{
			// create accelerated enemy
			Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
			Animation mainAnimation = new Animation(_acceleratedMonsterTexture, 7, true);
			animations.Add("main", mainAnimation);

			AcceleratedWhiteGlobulo enemy = new AcceleratedWhiteGlobulo(animations, 24, 30);
			enemy.Position = SetSpriteInitialPositionOnScreenBorder();

			// set enemy speed
			enemy.Speed = Vector2.Normalize(new Vector2(240, 400) - enemy.Position) * 20f;

			_enemies.Add(enemy);
		}

		// da rivedere
		public void CreateOrbitalEnemy()
		{
			Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
			Animation mainAnimation = new Animation(_orbitalMonsterTexture, 7, true);
			animations.Add("main", mainAnimation);

			OrbitalWhiteGlobulo enemy = new OrbitalWhiteGlobulo(animations, 24, 30);
			Vector2 position = SetSpriteInitialPositionOnScreenBorder();
			enemy.Position = position;

			if ((int)position.X % 2 == 0)
				enemy.SetSpiralParameters(new Vector2(240, 400), -(float)Math.PI / 30, true, 100);
			else
				enemy.SetSpiralParameters(new Vector2(240, 400), (float)Math.PI / 30, false, 100);

			_enemies.Add(enemy);
		}

		// da rivedere
		private void CreateBouncingEnemy(Vector2 position, Vector2 speed)
		{
			// create simple enemy
			Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
			Animation mainAnimation = new Animation(_constantSpeedMonsterTexture, 7, true);
			animations.Add("main", mainAnimation);

			BouncingWhiteGlobulo enemy = new BouncingWhiteGlobulo(animations, ENEMY_RADIUS, ENEMY_TOUCH_RADIUS)
			{
				Position = position,
				Speed = speed,
			};

			_enemies.Add(enemy);
		}
	}

	public class BonusFactory : SpriteFactory
	{
		List<GoToVirusBonus> _bonuses;   // reference to bonus list

		//valori cablati
		const float BONUS_RADIUS = 29;
		const float BONUS_TOUCH_RADIUS = 30;
		const float BONUS_SPEED = 45;

		Texture2D _bombBonusTexture;
		Texture2D _ammoBonusTexture;
		Texture2D _oneUpBonusTexture;
		Texture2D _bombPlusBonusTexture;

		// bomb bonus creation time
		float _bombBonusCreationPeriodMin;
		float _bombBonusCreationPeriodMax;

		public BonusFactory(GameEventsManager eventManager, List<GoToVirusBonus> bonuses)
			:base(eventManager)
		{
			_bonuses = bonuses;
		}

		// initializing methods
		public Texture2D BombBonusTexture
		{
			set { _bombBonusTexture = value; }
		}

		public Texture2D AmmoBonusTexture
		{
			set { _ammoBonusTexture = value; }
		}

		public Texture2D OneUpBonusTexture
		{
			set { _oneUpBonusTexture = value; }
		}

		public Texture2D BombPlusBonusTexture
		{
			set { _bombPlusBonusTexture = value; }
		}

		public void SetBombBonusTimePeriod(float periodMin, float periodMax)
		{
			_bombBonusCreationPeriodMin = periodMin;
			_bombBonusCreationPeriodMax = periodMax;
		}

		public override void HandleEvent(GameEvent gameEvent)
		{
			TimeSpan actualTime = gameEvent.GameTimer;

			switch (gameEvent.EventType)
			{
				case GameEventType.undefined:
					break;

				case GameEventType.createBombBonus:
					CreateBonus(_bombBonusTexture, BonusType.bomb);
					break;

				case GameEventType.createAmmoBonus:
					CreateBonus(_ammoBonusTexture, BonusType.ammo);
					break;

				case GameEventType.createOneUpBonus:
					CreateBonus(_oneUpBonusTexture, BonusType.oneUp);
					break;

				case GameEventType.createBombPlusBonus:
					CreateBonus(_bombPlusBonusTexture, BonusType.bombAmmo);
					break;

				case GameEventType.ScheduleBombBonusCreation:
					ScheduleCreatePeriodicalBonus(actualTime);
					break;

				default:
					throw new Exception("W Blaze Baley!");
			}
		}

		private void CreateBonus(Texture2D bonusTexture, BonusType bonusType)
		{
			// create bonus
			Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
			Animation mainAnimation = new Animation(bonusTexture, 1, true);
			animations.Add("main", mainAnimation);

			GoToVirusBonus bonus = new GoToVirusBonus(animations, BONUS_RADIUS, BONUS_TOUCH_RADIUS, bonusType);
			bonus.Position = SetSpriteInitialPositionOnTopOrBotBorder();

			// set bonus speed
			Vector2 virusPosition = _virusPosition;
			bonus.Speed = Vector2.Normalize(virusPosition - bonus.Position) * BONUS_SPEED;

			_bonuses.Add(bonus);
		}

		private void ScheduleCreatePeriodicalBonus(TimeSpan actualTime)
		{
			CreateBonus(_bombBonusTexture, BonusType.bomb);

			double deltaT = DoubleDiceResult(_bombBonusCreationPeriodMin, _bombBonusCreationPeriodMax);
			_eventsManager.ScheduleEvent(new GameEvent(actualTime + TimeSpan.FromSeconds(deltaT), GameEventType.ScheduleBombBonusCreation, this));
		}
	}
}

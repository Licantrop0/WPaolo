using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace Virus
{
    public abstract class LevelDifficultyPack
    {

    }

    public class Level1DifficultyPackEnemies : LevelDifficultyPack
    {
        public float SimpleEnemySchedulingTimeIntervalMin { get; set; }
        public float SimpleEnemySchedulingTimeIntervalMax { get; set; }

        public float SimpleEnemyCreationTimeIntervalMin { get; set; }
        public float SimpleEnemyCreationTimeIntervalMax { get; set; }

        public float TimeToReachMin { get; set; }
        public float TimeToReachMax { get; set; }

        public int NumberOfMonstersMin { get; set; }
        public int NumberOfMonstersMax { get; set; }

        public Level1DifficultyPackEnemies (float schedTimeMin, float schedTimeMax, float createMin, float createMax,
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

    public class SpriteGenerator : GameEventHandler
    {
        protected Random _dice = new Random(DateTime.Now.Millisecond);
        protected Virus _virus;

        const int DELTA_BORDER = 15;
        const int W = 480;
        const int H = 800;
        
        const int P1 = W  +     2 * DELTA_BORDER;
        const int P2 = P1 + H + 2 * DELTA_BORDER;
        const int P3 = P2 + W + 2 * DELTA_BORDER;
        const int P4 = P3 + H + 2 * DELTA_BORDER;
        protected const int MAX_PERIMETER = P4;

        const int LIMITATION = 200;
        const int EMPTY_SPACE = H - 2 * LIMITATION;

        const int Q1  = W   + 2 * DELTA_BORDER;
        const int Q12 = Q1  + DELTA_BORDER + LIMITATION;
        const int Q2  = Q12 + LIMITATION + DELTA_BORDER;
        const int Q3  = Q2  + W + 2 * DELTA_BORDER;
        const int Q34 = Q3  + DELTA_BORDER + LIMITATION;
        const int Q4  = Q34 + LIMITATION + DELTA_BORDER;
        protected const int MAX_LIMITED_PERIMETER = Q4;

        public SpriteGenerator(GameEventsManager eventManager, Virus virus)
            :base(eventManager)
        {
            _virus = virus;
        }

        protected double DoubleDiceResult(double min, double max)
        {
            return min + _dice.NextDouble() * (max - min);
        }
        
        protected Vector2 SetSpriteInitialPositionOnScreenBorder()
        {
            int borderPosition = _dice.Next(1, P4 + 1);

            return SetSpriteInitialPositionOnScreenBorder(borderPosition);
        }

        protected Vector2 SetSpriteInitialPositionOnScreenBorder(int borderPosition)
        {
            if (borderPosition < P1)
            {
                return new Vector2(borderPosition - DELTA_BORDER, -DELTA_BORDER);
            }
            else if (borderPosition >= P1 && borderPosition < P2)
            {
                return new Vector2(W + DELTA_BORDER, (borderPosition - P1) - DELTA_BORDER);
            }
            else if (borderPosition >= P2 && borderPosition < P3)
            {
                return new Vector2(P3 - borderPosition - DELTA_BORDER, H + DELTA_BORDER);
            }
            else if (borderPosition >= P3)
            {
                return new Vector2(- DELTA_BORDER, P4 - borderPosition - DELTA_BORDER);
            }
            else
            {
                throw new Exception("Exception in MonsterFactory::SetEnemyInitialPositionOnScreenBorder()");
            }
        }

        protected Vector2 SetSpriteInitialPositionOnScreenBorderLimited()
        {
            int limitedBorderPosition = _dice.Next(1, MAX_LIMITED_PERIMETER + 1);

            return SetSpriteInitialPositionOnScreenBorderLimited(limitedBorderPosition);
        }

        protected Vector2 SetSpriteInitialPositionOnScreenBorderLimited(int limitedBorderPosition)
        {
            if (limitedBorderPosition < Q1)
            {
                return new Vector2(limitedBorderPosition - DELTA_BORDER, -DELTA_BORDER);
            }
            else if (limitedBorderPosition >= Q1 && limitedBorderPosition < Q12)
            {
                return new Vector2(W + DELTA_BORDER, (limitedBorderPosition - Q1) - DELTA_BORDER);
            }
            else if (limitedBorderPosition >= Q12 && limitedBorderPosition < Q2)
            {
                return new Vector2(W + DELTA_BORDER, (limitedBorderPosition - Q1) + EMPTY_SPACE - DELTA_BORDER);
            }
            else if (limitedBorderPosition >= Q2 && limitedBorderPosition < Q3)
            {
                return new Vector2(-(limitedBorderPosition - Q2) + W - DELTA_BORDER, H + DELTA_BORDER);
            }
            else if (limitedBorderPosition >= Q3 && limitedBorderPosition < Q34)
            {
                return new Vector2(-DELTA_BORDER, -(limitedBorderPosition - Q3) + H + DELTA_BORDER);
            }
            else if (limitedBorderPosition >= Q34 && limitedBorderPosition < Q4)
            {
                return new Vector2(-DELTA_BORDER, -(limitedBorderPosition - Q34) + LIMITATION);
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

        protected void ClearEvents(GameEventType[] gameEventType)
        {
            _eventsManager.ClearEventsType(gameEventType);
        }
    } 

    public class MonsterGenerator : SpriteGenerator
    {
        List<Enemy> _enemies;                // reference to the game enemies list
        List<Boss> _bossContainer;                   
        
        // configuration
        SpritePrototypeContainer _spritePrototypesDictionary;

        // difficulty parameters
        protected float _simpleEnemySchedulingTimeIntervalMin;
        protected float _simpleEnemySchedulingTimeIntervalMax;

        protected float _simpleEnemyCreationTimeIntervalMin;
        protected float _simpleEnemyCreationTimeIntervalMax;

        protected float _timeToReachMin;
        protected float _timeToReachMax;

        protected int _numberOfMonstersMin;
        protected int _numberOfMonstersMax;

        public MonsterGenerator(GameEventsManager eventManager, Virus virus,
                                List<Enemy> enemies, List<Boss> bossContainer, SpritePrototypeContainer animationFactory)
            :base(eventManager, virus)
        {
            _enemies = enemies;
            _bossContainer = bossContainer;
            _spritePrototypesDictionary = animationFactory;
        }

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

        public override void HandleEvent(GameEventRecord gameEventRecord)
        {
            float actualTime = gameEventRecord.Time;
            GameEvent gameEvent = gameEventRecord.GameEvent;

            switch (gameEvent.EventType)
            {
                case GameEventType.undefined:
                    break;

                case GameEventType.clearEvents:
                    ClearEvents((GameEventType[])gameEvent.Params[0]);
                    break;

                case GameEventType.createSimpleEnemy:
                    CreateSimpleEnemy((Vector2)gameEvent.Params[0], (float)gameEvent.Params[1]);
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

                case GameEventType.scheduleMassEnemyCycleCreation:
                    CreateMassSimpleEnemyComingOneAfterOtherInClockWiseFashion();
                    break;

                case GameEventType.scheduleMassEnemyAllTogetherCreation:
                    CreateMassSimpleEnemyComingTogetherInAllDirections();
                    break;

                case GameEventType.createBossLung:
                    CreateBossLung();
                    break;

                case GameEventType.scheduleSimpleEnemyCreation:
                    ScheduleSimpleEnemyCreation();
                    break;

                case GameEventType.scheduleAcceleratedEnemyCreation:
                    ScheduleAcceleratedEnemyCreation();
                    break;

                case GameEventType.scheduleOrbitalEnemyCreation:
                    ScheduleOrbitalEnemyCreation();
                    break;

                case GameEventType.changeLevel1Difficulty:
                    SetDifficulty((Level1DifficultyPackEnemies)gameEvent.Params[0]);
                    break;

                default:
                    throw new Exception("W Blaze Baley!");
            }
        }

        private void ScheduleSimpleEnemyCreation()
        {
            int numOfMonsterToCreate = _dice.Next(_numberOfMonstersMin, _numberOfMonstersMax + 1);
            double deltaT = 0;

            // schedule next simple enemy creations
            for (int i = 0; i < numOfMonsterToCreate; i++)
            {
                deltaT = DoubleDiceResult(_simpleEnemyCreationTimeIntervalMin, _simpleEnemyCreationTimeIntervalMax);
                Vector2 enemyPosition = SetSpriteInitialPositionOnScreenBorder();
                float timeToReach = (float)DoubleDiceResult(_timeToReachMin, _timeToReachMax);

                GameEvent cse = new GameEvent(GameEventType.createSimpleEnemy, this, new Object[] { enemyPosition, timeToReach });
                _eventsManager.ScheduleEventInTime(cse, (float)deltaT);
            }

            // schedule monster creation schedule
            deltaT = DoubleDiceResult(_simpleEnemySchedulingTimeIntervalMin, _simpleEnemySchedulingTimeIntervalMax);

            _eventsManager.ScheduleEventInTime(new GameEvent(GameEventType.scheduleSimpleEnemyCreation, this), (float)deltaT);
        }

        private void CreateSimpleEnemy(Vector2 enemyPosition, float timeToReach)
        {
            // create simple enemy
            var enemy = new WhiteGlobulo(new MassDoubleIntegratorDynamicSystem(),
                                         _spritePrototypesDictionary.Sprites["WhiteGlobulo"].Clone(),
                                         new CircularShape(Global.GLOBULO_RADIUS, Global.GLOBULO_TOUCH_RADIUS));

            enemy.Position = enemyPosition;

            // calculate speed modulus
            float distance = Vector2.Distance(enemyPosition, _virus.Position);
            float speedModulus = distance / timeToReach;

            // set enemy speed
            enemy.Speed = Vector2.Normalize(_virus.Position - enemy.Position) * speedModulus;

            _enemies.Add(enemy);
        }

        // da rivedere
        private void ScheduleAcceleratedEnemyCreation()
        {
            // schedule creation of accelerated monster
            _eventsManager.ScheduleEventInTime(new GameEvent(GameEventType.createAcceleratedEnemy, this), 0.2f);

            // schedule accelerated monster creation schedule
            double deltaT = DoubleDiceResult(2, 5);
            _eventsManager.ScheduleEventInTime(new GameEvent(GameEventType.scheduleAcceleratedEnemyCreation, this), (float)deltaT);
        }

        // da rivedere
        private void ScheduleOrbitalEnemyCreation()
        {
            // temp! schedule creation of orbital monster
            _eventsManager.ScheduleEventInTime(new GameEvent(GameEventType.createOrbitalEnemy, this), 1);

            // schedule orbital monster creation schedule
            double deltaT = DoubleDiceResult(3, 7);
            _eventsManager.ScheduleEventInTime(new GameEvent(GameEventType.scheduleOrbitalEnemyCreation, this), (float)deltaT);
        }	

        private void CreateMassSimpleEnemyComingOneAfterOtherInClockWiseFashion()
        {
            int startPerimeter = _dice.Next(MAX_PERIMETER);
            int enemiesToCreate = 30;
            float deltaTime = 0.30f;
            float deltaPerimeter = 200;

            float perimeter = startPerimeter;
            float time = 0;
            for (int i = 0; i < enemiesToCreate; i++)
            {
                Vector2 enemyPosition = SetSpriteInitialPositionOnScreenBorder((int)perimeter);
                float timeToReach = 1.75f;

                GameEvent cse = new GameEvent(GameEventType.createSimpleEnemy, this, new Object[] { enemyPosition, timeToReach });
                _eventsManager.ScheduleEventInTime(cse, time);

                time += deltaTime;
                perimeter += deltaPerimeter;
                if (perimeter > MAX_PERIMETER)
                    perimeter -= MAX_PERIMETER;
            }
        }

        private void CreateMassSimpleEnemyComingTogetherInAllDirections()
        {
            int startPerimeter = _dice.Next(MAX_PERIMETER);
            int enemiesToCreate = 12;
            float deltaPerimeter = (float)MAX_PERIMETER / (float)enemiesToCreate;
            float timeToReach = 1.8f;

            float perimeter = startPerimeter;
            for (int i = 0; i < enemiesToCreate; i++)
            {
                Vector2 enemyPosition = SetSpriteInitialPositionOnScreenBorder((int)perimeter);

                CreateSimpleEnemy(enemyPosition, timeToReach);

                perimeter += deltaPerimeter;
                if (perimeter > MAX_PERIMETER)
                    perimeter -= MAX_PERIMETER;
            }
        }

        private void CreateBossLung()
        {
            // create boss
            BossLung boss = new BossLung(new MassDoubleIntegratorDynamicSystem(),
                                         _spritePrototypesDictionary.Sprites["BossLung"].Clone(),
                                         new RectangularShape(240, 178, 240, 178),
                                         _virus,
                                         _spritePrototypesDictionary.Sprites["CentralMouth"],
                                         _spritePrototypesDictionary.Sprites["Mouth"],
                                         _spritePrototypesDictionary.Sprites["WhiteGlobulo"],
                                         _enemies);

            _bossContainer.Add(boss);
        }

        // da rivedere
        private void CreateAcceleratedEnemy()
        {
            /*// create accelerated enemy
            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            Animation mainAnimation = new Animation(_acceleratedMonsterTexture, 7, true);
            animations.Add("main", mainAnimation);

            AcceleratedWhiteGlobulo enemy = new AcceleratedWhiteGlobulo(animations, 24, 30);
            enemy.Position = SetSpriteInitialPositionOnScreenBorder();

            // set enemy speed
            enemy.Speed = Vector2.Normalize(new Vector2(240, 400) - enemy.Position) * 20f;

            _enemies.Add(enemy);*/
        }

        // da rivedere
        public void CreateOrbitalEnemy()
        {
            /*Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            Animation mainAnimation = new Animation(_orbitalMonsterTexture, 7, true);
            animations.Add("main", mainAnimation);

            OrbitalWhiteGlobulo enemy = new OrbitalWhiteGlobulo(animations, 24, 30);
            Vector2 position = SetSpriteInitialPositionOnScreenBorder();
            enemy.Position = position;

            if ((int)position.X % 2 == 0)
                enemy.SetSpiralParameters(new Vector2(240, 400), -(float)Math.PI / 30, true, 100);
            else
                enemy.SetSpiralParameters(new Vector2(240, 400), (float)Math.PI / 30, false, 100);

            _enemies.Add(enemy);*/
        }

        private void CreateBouncingEnemy(Vector2 position, Vector2 speed)
        {
            // create bouncing enemy
            BouncingWhiteGlobulo enemy = new BouncingWhiteGlobulo(new MassDoubleIntegratorDynamicSystem(),
                                                                  _spritePrototypesDictionary.Sprites["WhiteGlobulo"].Clone(),
                                                                  new CircularShape(Global.GLOBULO_RADIUS, Global.GLOBULO_TOUCH_RADIUS))
            {
                Position = position,
                Speed = speed,
            };

            _enemies.Add(enemy);
        }
    }

    public class BonusGenerator : SpriteGenerator
    {
        List<GoToVirusBonus> _bonuses;   // reference to bonus list

        // configuration
        SpritePrototypeContainer _animationFactory;

        //valori cablati
        const float BONUS_RADIUS = 29;
        const float BONUS_TOUCH_RADIUS = 30;

        public float BonusSpeed { get; set; }

        // bomb bonus creation time
        float _bombBonusCreationPeriodMin;
        float _bombBonusCreationPeriodMax;

        public BonusGenerator(GameEventsManager eventManager, Virus virus,
                              List<GoToVirusBonus> bonuses, SpritePrototypeContainer animationFactory)
            :base(eventManager, virus)
        {
            _bonuses = bonuses;
            _animationFactory = animationFactory;
        }

        public void SetBombBonusTimePeriod(float periodMin, float periodMax)
        {
            _bombBonusCreationPeriodMin = periodMin;
            _bombBonusCreationPeriodMax = periodMax;
        }

        public override void HandleEvent(GameEventRecord gameEventRecord)
        {
            //float actualTime = gameEventRecord.Time;
            GameEvent gameEvent = gameEventRecord.GameEvent;

            switch (gameEventRecord.GameEvent.EventType)
            {
                case GameEventType.undefined:
                    break;

                case GameEventType.clearEvents:
                    ClearEvents((GameEventType[])gameEvent.Params[0]);
                    break;

                case GameEventType.createBombBonus:
                    CreateBonus("Bomb", BonusType.bomb);
                    break;

                case GameEventType.createAmmoBonus:
                    CreateBonus("Ammo", BonusType.ammo);
                    break;

                case GameEventType.createOneUpBonus:
                    CreateBonus("OneUp", BonusType.oneUp);
                    break;

                case GameEventType.createBombPlusBonus:
                    CreateBonus("BombPlus", BonusType.bombAmmo);
                    break;

                case GameEventType.create10PointsBonus:
                    CreateBonus("Points10", BonusType.points10);
                    break;

                case GameEventType.scheduleBombBonusCreation:
                    ScheduleCreatePeriodicalBonus();
                    break;

                case GameEventType.changeBonusSpeed:
                    BonusSpeed = (int)gameEvent.Params[0];
                    break;

                case GameEventType.scheduleBonusPointsDistribution:
                    ScheduleCreatePointsBonus((float)gameEvent.Params[0], (float)gameEvent.Params[1]);
                    break;

                default:
                    throw new Exception("W Blaze Baley!");
            }
        }

        private void CreateBonus(string bonusSpriteName, BonusType bonusType)
        {
            // create bonus
            GoToVirusBonus bonus = new GoToVirusBonus(new MassDoubleIntegratorDynamicSystem(),
                                                      _animationFactory.Sprites[bonusSpriteName].Clone(),
                                                      new CircularShape(BONUS_RADIUS, BONUS_TOUCH_RADIUS),
                                                      bonusType);

            bonus.Position = SetSpriteInitialPositionOnScreenBorderLimited();

            // set bonus speed
            bonus.Speed = Vector2.Normalize(_virus.Position - bonus.Position) * BonusSpeed;

            _bonuses.Add(bonus);
        }

        private void ScheduleCreatePeriodicalBonus()
        {
            CreateBonus("Bomb", BonusType.bomb);

            double deltaT = DoubleDiceResult(_bombBonusCreationPeriodMin, _bombBonusCreationPeriodMax);
            _eventsManager.ScheduleEventInTime(new GameEvent(GameEventType.scheduleBombBonusCreation, this), (float)deltaT);
        }

        private void ScheduleCreatePointsBonus(float period, float lasting)
        {
            int nBonusToCreate = (int)Math.Round(lasting / period);

            for (int i = 0; i < nBonusToCreate; i++)
            {
                _eventsManager.ScheduleEventInTime(new GameEvent(GameEventType.create10PointsBonus, this), (float)(_dice.NextDouble() * lasting));
            }
        }

    }
}

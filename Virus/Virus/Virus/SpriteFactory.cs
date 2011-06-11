using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class SpriteFactory : GameEventHandler
    {
        List<WhiteGlobulo> _enemies;     // reference to the game enemies list
        List<GoToVirusBonus> _bonuses;   // reference to bonus list

        Random _dice = new Random(DateTime.Now.Millisecond);

        Texture2D _constantSpeedMonsterTexture;
        Texture2D _acceleratedMonsterTexture;
        Texture2D _orbitalMonsterTexture;
        Texture2D _bombBonusTexture;
        Texture2D _ammoBonusTexture;
        Texture2D _oneUpBonusTexture;
        Texture2D _bombPlusBonusTexture;

        public TimeSpan SchedulingTimeIntervalMin { get; set; }

        public TimeSpan SchedulingTimeIntervalMax { get; set; }

        public TimeSpan CreationTimeIntervalMin { get; set; }

        public TimeSpan CreationTimeIntervalMax { get; set; }

        public float TimeToReachMin { get; set; }

        public float TimeToReachMax { get; set; }

        public int NumberOfMonstersMin { get; set; }

        public int NumberOfMonstersMax { get; set; }

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

                case GameEventType.createAcceleratedEnemy:
                    CreateAcceleratedEnemy();
                    break;

                case GameEventType.createOrbitalEnemy:
                    CreateOrbitalEnemy();
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

                case GameEventType.scheduleSimpleEnemyCreation:
                    ScheduleSimpleEnemyCreation(actualTime);
                    break;

                case GameEventType.scheduleAcceleratedEnemyCreation:
                    ScheduleAcceleratedEnemyCreation(actualTime);
                    break;

                case GameEventType.scheduleOrbitalEnemyCreation:
                    ScheduleOrbitalEnemyCreation(actualTime);
                    break;

                default:
                    throw new Exception("W Blaze Baley!");
            }
        }

        private void ScheduleSimpleEnemyCreation(TimeSpan actualTime)
        {
            int numOfMonsterToCreate = _dice.Next(NumberOfMonstersMin, NumberOfMonstersMax + 1);
            double deltaT = 0;
            GameEvent ge;

            // schedule monster creation
            for (int i = 0; i < numOfMonsterToCreate; i++)
            {
                deltaT = (CreationTimeIntervalMin.TotalMilliseconds +
                    _dice.NextDouble() * (CreationTimeIntervalMax.TotalMilliseconds - CreationTimeIntervalMin.TotalMilliseconds));
                ge = new GameEvent(actualTime + TimeSpan.FromMilliseconds(deltaT), GameEventType.createSimpleEnemy, this);
                _eventsManager.ScheduleEvent(ge);
            }

            // schedule monster creation schedule
            deltaT = (SchedulingTimeIntervalMin.TotalMilliseconds +
                _dice.NextDouble() * (SchedulingTimeIntervalMax.TotalMilliseconds - SchedulingTimeIntervalMin.TotalMilliseconds));
            ge = new GameEvent(actualTime + TimeSpan.FromMilliseconds(deltaT), GameEventType.scheduleSimpleEnemyCreation, this);
            _eventsManager.ScheduleEvent(ge);
        }

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

        private Vector2 SetSpriteInitialPositionOnScreenBorder()
        {
            int deltaBorderY = 30;
            int deltaBorderX = 30;

            int p1 =      480 + 2 * deltaBorderX;
            int p2 = p1 + 800 + 2 * deltaBorderY;
            int p3 = p2 + 480 + 2 * deltaBorderX;
            int p4 = p3 + 800 + 2 * deltaBorderY;

            int borderPosition = _dice.Next(1, p4 + 1);

            if (borderPosition < p1)
            {
                return new Vector2(borderPosition - deltaBorderX, - deltaBorderY);
            }
            else if (borderPosition >= p1 && borderPosition < p2)
            {
                return new Vector2(480 + deltaBorderX, (borderPosition - p1) - deltaBorderY);
            }
            else if (borderPosition >= p2 && borderPosition < p3)
            {
                return new Vector2(800 + deltaBorderY, p3 - borderPosition - deltaBorderX);
            }
            else if (borderPosition >= p3)
            {
                return new Vector2(- deltaBorderX, p4 - borderPosition - deltaBorderY );
            }
            else
            {
                throw new Exception("Exception in MonsterFactory::SetEnemyInitialPositionOnScreenBorder()");
            }
        }

        private Vector2 SetSpriteInitialPositionOnTopOrBotBorder()
        {
            int deltaBorderX = 30;
            int deltaBorderY = 30;

            int p1 = 480 + 2 * deltaBorderX;
            int p2 = p1 + 480 + 2 * deltaBorderX;

            int borderPosition = _dice.Next(1, p2 + 1);

            if (borderPosition < p1)
            {
                return new Vector2(borderPosition - deltaBorderX, -deltaBorderY);
            }
            else
            {
                return new Vector2(p2 - borderPosition - deltaBorderX, 800 + deltaBorderY); ;
            }
        }

        public void CreateAcceleratedEnemy()
        {
            // create accelerated enemy
            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            Animation mainAnimation = new Animation(_acceleratedMonsterTexture, 7, true);
            animations.Add("main", mainAnimation);

            AcceleratedWhiteGlobulo enemy = new AcceleratedWhiteGlobulo(animations, 24, 30);
            enemy.Position =  SetSpriteInitialPositionOnScreenBorder();

            // set enemy speed
            enemy.Speed = Vector2.Normalize(new Vector2(240, 400) - enemy.Position) * 20f;

            _enemies.Add(enemy);
        }

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
                enemy.SetSpiralParameters(new Vector2(240, 400),  (float)Math.PI / 30, false, 100);

            _enemies.Add(enemy);
        }

        private void CreateSimpleEnemy()
        {
            // create simple enemy
            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            Animation mainAnimation = new Animation(_constantSpeedMonsterTexture, 7, true);
            animations.Add("main", mainAnimation);

            WhiteGlobulo enemy = new WhiteGlobulo(animations, 29, 34);
            Vector2 enemyPosition = SetSpriteInitialPositionOnScreenBorder();
            enemy.Position = enemyPosition;

            // extract time to reach
            float timeToReach = (float)(TimeToReachMin + _dice.NextDouble() * (TimeToReachMax - TimeToReachMin));

            // calculate speed modulus
            Vector2 virusPosition = new Vector2(240, 400);
            float distance = Vector2.Distance(enemyPosition, virusPosition) - enemy.Radius - 37;    // 37 è il raggio del virus, va messo come variabile membro della monster factory
            float speedModulus = distance / timeToReach;

            // set enemy speed
            enemy.Speed = Vector2.Normalize(virusPosition - enemy.Position) * speedModulus;

            _enemies.Add(enemy);
        }

        private void CreateBonus(Texture2D bonusTexture, BonusType bonusType)
        {
            // create bonus
            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            Animation mainAnimation = new Animation(bonusTexture, 1, true);
            animations.Add("main", mainAnimation);

            GoToVirusBonus bonus = new GoToVirusBonus(animations, 29, 30, bonusType);
            bonus.Position = SetSpriteInitialPositionOnTopOrBotBorder();

            // set bonus speed
            Vector2 virusPosition = new Vector2(240, 400);
            bonus.Speed = Vector2.Normalize(virusPosition - bonus.Position) * 45;

            _bonuses.Add(bonus);
        }


        public SpriteFactory(GameEventsManager em,
            List<WhiteGlobulo> enemies, List<GoToVirusBonus> bonuses,
            Texture2D monsterTexture,
            Texture2D bombBonusTexture, Texture2D ammoBonusTexture, Texture2D oneUpBonusTexture, Texture2D bombPlusBonusTexture,
            TimeSpan schedTimeIntervalMin, TimeSpan schedTimeIntervalMax,
            TimeSpan createTimeIntervalMin, TimeSpan createTimeIntervalMax,
            float timeToReachMin, float timeToReachMax,
            int numOfMonstersMin, int numOfMonstersMax)
            : base(em)
        {
            _enemies = enemies;
            _bonuses = bonuses;
            _constantSpeedMonsterTexture = monsterTexture;
            _bombBonusTexture = bombBonusTexture;
            _ammoBonusTexture = ammoBonusTexture;
            _oneUpBonusTexture = oneUpBonusTexture;
            _bombPlusBonusTexture = bombPlusBonusTexture;
            SchedulingTimeIntervalMin = schedTimeIntervalMin;
            SchedulingTimeIntervalMax = schedTimeIntervalMax;
            CreationTimeIntervalMin = createTimeIntervalMin;
            CreationTimeIntervalMax = createTimeIntervalMax;
            TimeToReachMin = timeToReachMin;
            TimeToReachMax = timeToReachMax;
            NumberOfMonstersMin = numOfMonstersMin;
            NumberOfMonstersMax = numOfMonstersMax;
        }
    } 
}

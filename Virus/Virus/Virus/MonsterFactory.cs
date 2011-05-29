using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class MonsterFactory : GameEventHandler
    {
        List<WhiteGlobulo> _enemies;     // reference to the game enemies list

        Random _dice = new Random(DateTime.Now.Millisecond);

        Texture2D _constantSpeedMonsterTexture;

        Texture2D _acceleratedMonsterTexture;

        Texture2D _orbitalMonsterTexture;

        public TimeSpan SchedulingTimeIntervalMin { get; set; }

        public TimeSpan SchedulingTimeIntervalMax { get; set; }

        public TimeSpan CreationTimeIntervalMin { get; set; }

        public TimeSpan CreationTimeIntervalMax { get; set; }

        public float MonsterSpeedMin { get; set; }

        public float MonsterSpeedMax { get; set; }

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

        private Vector2 SetEnemyInitialPositionOnScreenBorder()
        {
            // roll the dice for enemy position
            int borderPosition = _dice.Next(1, 2561);

            // set enemy initial position
            if (borderPosition <= 480)
            {
                return new Vector2(borderPosition, 1);
            }
            else if (borderPosition >= 481 && borderPosition <= 1280)
            {
                return  new Vector2(480, borderPosition - 480);
            }
            else if (borderPosition >= 1281 && borderPosition <= 1760)
            {
                return new Vector2(1760 - borderPosition, 800);
            }
            else if (borderPosition >= 1761)
            {
                return new Vector2(1, 2560 - borderPosition);
            }
            else
            {
                throw new Exception("Exception in MonsterFactory::SetEnemyInitialPositionOnScreenBorder()");
            }
        }

        public void CreateAcceleratedEnemy()
        {
            // create accelerated enemy
            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            Animation mainAnimation = new Animation(_acceleratedMonsterTexture, 7);
            animations.Add("main", mainAnimation);

            AcceleratedWhiteGlobulo enemy = new AcceleratedWhiteGlobulo(animations, 24, 30);
            enemy.Position =  SetEnemyInitialPositionOnScreenBorder();

            // set enemy speed
            enemy.Speed = Vector2.Normalize(new Vector2(240, 400) - enemy.Position) * 20f;

            _enemies.Add(enemy);
        }

        public void CreateOrbitalEnemy()
        {
            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
            Animation mainAnimation = new Animation(_orbitalMonsterTexture, 7);
            animations.Add("main", mainAnimation);

            OrbitalWhiteGlobulo enemy = new OrbitalWhiteGlobulo(animations, 24, 30);
            Vector2 position = SetEnemyInitialPositionOnScreenBorder();
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
            Animation mainAnimation = new Animation(_constantSpeedMonsterTexture, 7);
            animations.Add("main", mainAnimation);

            WhiteGlobulo enemy = new WhiteGlobulo(animations, 24, 30);
            enemy.Position = SetEnemyInitialPositionOnScreenBorder();

            // set enemy speed
            Vector2 virusPosition = new Vector2(240, 400);
            enemy.Speed = Vector2.Normalize(virusPosition - enemy.Position) *
                (float)(MonsterSpeedMin + _dice.NextDouble() * (MonsterSpeedMax - MonsterSpeedMin));

            _enemies.Add(enemy);
        }

        public MonsterFactory(GameEventsManager em,
            List<WhiteGlobulo> enemies, Texture2D monsterTexture, Texture2D acceleratedMonsterTexture, Texture2D orbitalMonsterTexture,
            TimeSpan schedTimeIntervalMin, TimeSpan schedTimeIntervalMax,
            TimeSpan createTimeIntervalMin, TimeSpan createTimeIntervalMax,
            float speedMin, float speedMax,
            int numOfMonstersMin, int numOfMonstersMax)
            : base(em)
        {
            _enemies = enemies;
            _constantSpeedMonsterTexture = monsterTexture;
            _acceleratedMonsterTexture = acceleratedMonsterTexture;
            _orbitalMonsterTexture = orbitalMonsterTexture;
            SchedulingTimeIntervalMin = schedTimeIntervalMin;
            SchedulingTimeIntervalMax = schedTimeIntervalMax;
            CreationTimeIntervalMin = createTimeIntervalMin;
            CreationTimeIntervalMax = createTimeIntervalMax;
            MonsterSpeedMin = speedMin;
            MonsterSpeedMax = speedMax;
            NumberOfMonstersMin = numOfMonstersMin;
            NumberOfMonstersMax = numOfMonstersMax;
        }
    } 
}

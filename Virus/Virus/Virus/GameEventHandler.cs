using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class GameEventHandler
    {
        protected Game1 _game;

        protected GameEventsManager _eventsManager;    // reference to the game manager

        public virtual void HandleEvent(GameEvent ge)
        {
            
        }

        public GameEventHandler(GameEventsManager em)
        {
            _eventsManager = em;
        }
    }

    public class MonsterFactory : GameEventHandler
    {
        List<SimpleEnemy> _enemies;     // reference to the game enemies list

        SpriteAnimation _monsterSpriteAnimation;

        Random _dice = new Random(DateTime.Now.Millisecond);

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

                case GameEventType.scheduleSimpleEnemyCreation:
                    ScheduleSimpleEnemyCreation(actualTime);
                    break;

                default:
                    break;
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

        private void CreateSimpleEnemy()
        {
            // create simple enemy
            SpriteAnimation monsterSpriteAnimation = new SpriteAnimation(_monsterSpriteAnimation._texture, 7);
            monsterSpriteAnimation.IsLooping = _monsterSpriteAnimation.IsLooping;
            monsterSpriteAnimation.FramesPerSecond = _monsterSpriteAnimation.FramesPerSecond;
            monsterSpriteAnimation.Origin = new Vector2(_monsterSpriteAnimation.RectangleWidth / 2, _monsterSpriteAnimation.RectangleHeight / 2);
            SimpleEnemy enemy = new SimpleEnemy(monsterSpriteAnimation, 30f);

            // roll the dice for enemy position
            int borderPosition = _dice.Next(1, 2561);

            // set enemy initial position
            if (borderPosition <= 480)
            {
                enemy.Position = new Vector2(borderPosition, 1);
            }
            else if (borderPosition >= 481 && borderPosition <= 1280)
            {
                enemy.Position = new Vector2(480, borderPosition - 480);
            }
            else if (borderPosition >= 1281 && borderPosition <= 1760)
            {
                enemy.Position = new Vector2(1760 - borderPosition, 800);
            }
            else if (borderPosition >= 1761)
            {
                enemy.Position = new Vector2(1, 2560 - borderPosition);
            }
            
            // set enemy speed
            Vector2 virusPosition = new Vector2(240, 400);
            enemy.Speed = Vector2.Normalize(virusPosition - enemy.Position) * 
                (float)(MonsterSpeedMin + _dice.NextDouble() * (MonsterSpeedMax - MonsterSpeedMin));

            _enemies.Add(enemy);
        }

        public MonsterFactory(GameEventsManager em,
            List<SimpleEnemy> enemies, SpriteAnimation spriteAnimation,
            TimeSpan schedTimeIntervalMin, TimeSpan schedTimeIntervalMax,
            TimeSpan createTimeIntervalMin, TimeSpan createTimeIntervalMax,
            float speedMin, float speedMax,
            int numOfMonstersMin, int numOfMonstersMax)
            : base(em)
        {
            _enemies = enemies;
            _monsterSpriteAnimation = spriteAnimation;
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

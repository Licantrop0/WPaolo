using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiwi
{
    public enum GameEventType
    {
        undefined,
        createSimpleEnemy,
        createBouncingEnemy,
        createAcceleratedEnemy,
        createOrbitalEnemy,
        createBossLung,
        createBombBonus,
        createAmmoBonus,
        createOneUpBonus,
        createBombPlusBonus,
        scheduleSimpleEnemyCreation,
        scheduleAcceleratedEnemyCreation,
        scheduleOrbitalEnemyCreation,
        ScheduleBombBonusCreation,
        ChangeLevel1Difficulty
    }

    public class GameEvent
    {
        public TimeSpan GameTimer { get; set; }

        public GameEventType EventType { get; set; }

        public GameEventHandler Subscriber { get; set; }

        public Object[] Params { get; set; }

        public GameEvent(TimeSpan time, GameEventType et, GameEventHandler subscriber)
        {
            GameTimer = time;
            EventType = et;
            Subscriber = subscriber;
        }

        public GameEvent(TimeSpan time, GameEventType et, GameEventHandler subscriber, Object[] parameters)
        {
            GameTimer = time;
            EventType = et;
            Subscriber = subscriber;
            Params = parameters;
        }
    }
}

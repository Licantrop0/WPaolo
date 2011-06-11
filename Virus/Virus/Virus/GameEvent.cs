using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virus
{
    public enum GameEventType
    {
        undefined,
        createSimpleEnemy,
        createAcceleratedEnemy,
        createOrbitalEnemy,
        createBombBonus,
        createAmmoBonus,
        createOneUpBonus,
        scheduleSimpleEnemyCreation,
        scheduleAcceleratedEnemyCreation,
        scheduleOrbitalEnemyCreation
    }

    public class GameEvent
    {
        public TimeSpan GameTimer { get; set; }

        public GameEventType EventType { get; set; }

        public GameEventHandler Subscriber { get; set; }

        public GameEvent(TimeSpan time, GameEventType et, GameEventHandler subscriber)
        {
            GameTimer = time;
            EventType = et;
            Subscriber = subscriber;
        }
    }
}

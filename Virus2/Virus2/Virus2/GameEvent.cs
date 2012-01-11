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
        createBouncingEnemy,
        createAcceleratedEnemy,
        createOrbitalEnemy,
        createBossLung,
        createBombBonus,
        createAmmoBonus,
        createOneUpBonus,
        createBombPlusBonus,
        create10PointsBonus,
        changeBonusSpeed,
        scheduleSimpleEnemyCreation,
        scheduleAcceleratedEnemyCreation,
        scheduleOrbitalEnemyCreation,
        scheduleMassEnemyCycleCreation,
        scheduleMassEnemyAllTogetherCreation,
        scheduleBombBonusCreation,
        scheduleBonusPointsDistribution,
        changeLevel1Difficulty,
        clearEvents
    }

    public class GameEvent
    {
        //public TimeSpan GameTimer { get; set; }

        public GameEventType EventType { get; set; }

        public GameEventHandler Subscriber { get; set; }

        public Object[] Params { get; set; }

        public GameEvent(GameEventType et, GameEventHandler subscriber)
        {
            EventType = et;
            Subscriber = subscriber;
        }

        public GameEvent(GameEventType et, GameEventHandler subscriber, Object[] parameters)
        {
            EventType = et;
            Subscriber = subscriber;
            Params = parameters;
        }
    }
}

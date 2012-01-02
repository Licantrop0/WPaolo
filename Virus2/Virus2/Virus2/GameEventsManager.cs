using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virus
{
    public class GameEventRecord
    {
        public float Time;
        public GameEvent GameEvent;
    }

    public class GameEventsManager
    {
        List<GameEventRecord> _gameEvents = new List<GameEventRecord>();
        public float Timer { get; set; }

        private void InsertIntoList(GameEventRecord ger)
        {
            int count = _gameEvents.Count;

            if (count == 0)
            {
                _gameEvents.Add(ger);
            }
            else
            {
                int i = 0;
                while (i < count && ger.Time > _gameEvents[i].Time)
                {
                    i++;
                }

                if (i != count)
                {
                    _gameEvents.Insert(i, ger);
                }
                else
                {
                    _gameEvents.Add(ger);
                }
            }
        }

        // schedule now
        public void ScheduleEventNow(GameEvent ge)
        {
            GameEventRecord ger = new GameEventRecord()
            {
                Time = Timer,
                GameEvent = ge
            };

            InsertIntoList(ger);
        }

        // schedule at given timer
        public void ScheduleEventAtTime(GameEvent ge, float scheduledEventTime)
        {
            GameEventRecord ger = new GameEventRecord()
            {
                Time = scheduledEventTime,
                GameEvent = ge
            };

            InsertIntoList(ger);
        }

        public void ScheduleEventInTime(GameEvent ge, float seconds)
        {
            GameEventRecord ger = new GameEventRecord()
            {
                Time = Timer + seconds,
                GameEvent = ge
            };

            InsertIntoList(ger);
        }

        public void ManageCurrentEvent()
        {
            if (_gameEvents.Count > 0)
            {
                GameEventRecord curEventRecord = _gameEvents[0];
                if (Timer > curEventRecord.Time)
                {
                    GameEventHandler handler = curEventRecord.GameEvent.Subscriber;
                    _gameEvents.RemoveAt(0);
                    handler.HandleEvent(curEventRecord);
                }
            }
        }

        public void ClearAllEvents()
        {
            _gameEvents.Clear();
        }

        public void ClearEventsType(GameEventType[] eventTypes)
        {
            for (int i = 0; i < _gameEvents.Count; i++)
            {
                if (eventTypes.Contains(_gameEvents[i].GameEvent.EventType))
                {
                    _gameEvents.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

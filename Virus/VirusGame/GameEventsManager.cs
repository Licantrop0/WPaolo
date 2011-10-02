using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virus
{
    public class GameEventsManager
    {
        List<GameEvent> _gameEvents = new List<GameEvent>();

        public void ScheduleEvent(GameEvent ge)
        {
            int count = _gameEvents.Count;

            if (count == 0)
            {
                _gameEvents.Add(ge);
            }
            else
            {
                int i = 0;
                while (i < count && ge.GameTimer > _gameEvents[i].GameTimer)
                {
                    i++;
                }

                if (i != count)
                {
                    _gameEvents.Insert(i, ge);
                }
                else
                {
                    _gameEvents.Add(ge);
                }
            }
        }

        public void ManageCurrentEvent(TimeSpan gameTimer)
        {
            if (_gameEvents.Count > 0)
            {
                GameEvent curEvent = _gameEvents[0];
                if (gameTimer > curEvent.GameTimer)
                {
                    GameEventHandler handler = curEvent.Subscriber;
                    _gameEvents.RemoveAt(0);
                    handler.HandleEvent(curEvent);
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
                if (eventTypes.Contains(_gameEvents[i].EventType))
                {
                    _gameEvents.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

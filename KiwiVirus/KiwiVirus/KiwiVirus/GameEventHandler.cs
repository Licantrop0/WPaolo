using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kiwi
{
    public abstract class GameEventHandler
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
}

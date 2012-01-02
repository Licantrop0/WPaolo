using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public abstract class GameEventHandler
    {
        //protected VirusGame _game;

        protected GameEventsManager _eventsManager;    // reference to the game manager

        public virtual void HandleEvent(GameEventRecord ger)
        {
            
        }

        public GameEventHandler(GameEventsManager em)
        {
            _eventsManager = em;
        }
    }
}

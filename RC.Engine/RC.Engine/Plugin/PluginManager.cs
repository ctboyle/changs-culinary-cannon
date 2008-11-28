using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RC.Engine.Plugin
{
    public class RCPluginManager
    {
        public class GameTimeEventArgs : EventArgs
        {
            public GameTimeEventArgs(GameTime gt) { GameTime = gt; }
            public GameTime GameTime; 
        }

        public event EventHandler<GameTimeEventArgs> UpdateEvent;

        internal void RaiseUpdateEvent(GameTimeEventArgs args)
        {
            if(UpdateEvent != null) UpdateEvent(this, args);
        }
    }
}
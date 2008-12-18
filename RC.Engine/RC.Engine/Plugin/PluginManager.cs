using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RC.Engine.Base;

namespace RC.Engine.Plugin
{
    /// <summary>
    /// Manages new hooks into the engine.
    /// </summary>
    public class RCPluginManager
    {
        /// <summary>
        /// Events arguments for UpdateEvent.
        /// </summary>
        public class GameTimeEventArgs : EventArgs
        {
            public GameTimeEventArgs(GameTime gt) { GameTime = gt; }
            public GameTime GameTime; 
        }

        /// <summary>
        /// Creates a new instance of the plugin manager.
        /// </summary>
        /// <param name="game">The game.</param>
        public RCPluginManager(RCXnaGame game)
        {
            game.Services.AddService(typeof(RCPluginManager), this);
        }

        /// <summary>
        /// The Update Event.
        /// </summary>
        public event EventHandler<GameTimeEventArgs> UpdateEvent;

        internal void RaiseUpdateEvent(GameTimeEventArgs args)
        {
            if(UpdateEvent != null) UpdateEvent(this, args);
        }
    }
}
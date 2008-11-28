using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RC.Engine.ContentManagement;
using RC.Engine.StateManagement;
using System.Diagnostics;

namespace RC.Engine.Base
{
    internal abstract class RCXnaGame : Game
    {
        private GraphicsDeviceManager _deviceMgr = null;

        /// <summary>
        /// I am a contructor that will setup the graphics device manager.
        /// </summary>
        public RCXnaGame()
        {
            _deviceMgr = new GraphicsDeviceManager(this);
        }

        public abstract IRCContentRequester ContentRqst { set; get; }

        public abstract IRCGameStateManager StateMgr { set; get; }

        public abstract RCBasicGame Game { set; get; }

        /// <summary>
        /// I initialize stuff.
        /// </summary>
        protected override void Initialize()
        {
            Debug.Assert(ContentRqst != null);
            Debug.Assert(StateMgr != null);
            Debug.Assert(Game != null);

            Components.Add(ContentRqst);
            Components.Add(StateMgr);
            Game.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// I initialize more stuff.
        /// </summary>
        protected override void BeginRun()
        {
            Debug.Assert(Game != null);

            Game.BeginRun();
            base.BeginRun();
        }
    }
}

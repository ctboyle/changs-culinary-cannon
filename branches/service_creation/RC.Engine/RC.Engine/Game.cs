using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Ninject.Core;
using RC.Engine.StateManagement;
using RC.Engine.ContentManagement;

namespace RC.Engine
{
    /// <summary>
    /// I am the real XNA game instance.  I am hidden to the
    /// user of the actual RC framework.
    /// </summary>
    [Singleton]
    internal class RCGame : Game
    {
        private GraphicsDeviceManager _deviceMgr = null;
        private IRCGameStateManager _stateMgr = null;
        private IRCContentRequester _content = null;
        private RCBasicGame _game = null;

        /// <summary>
        /// I am a contructor that will setup the graphics device manager.
        /// </summary>
        public RCGame()
        {
            _deviceMgr = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// I initialize stuff.
        /// </summary>
        protected override void Initialize()
        {
            Components.Add(_content);
            Components.Add(_stateMgr);
            Game.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// I initialize more stuff.
        /// </summary>
        protected override void BeginRun()
        {
            Game.BeginRun();
            base.BeginRun();
        }

        [Inject]
        public RCBasicGame Game
        {
            get { return _game; }
            set { _game = value; }
        }

        [Inject]
        public IRCContentRequester ContentRqst
        {
            get { return _content; }
            set { _content = value; }
        }

        [Inject]
        public IRCGameStateManager StateMgr
        {
            get { return _stateMgr; }
            set { _stateMgr = value; }
        }
    }
}

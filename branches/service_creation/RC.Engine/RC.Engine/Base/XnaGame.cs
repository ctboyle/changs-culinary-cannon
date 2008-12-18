using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RC.Engine.ContentManagement;
using RC.Engine.StateManagement;
using System.Diagnostics;
using RC.Engine.Cameras;
using RC.Engine.Rendering;
using RC.Engine.Plugin;

namespace RC.Engine.Base
{
    /// <summary>
    /// The base game that all games should be derived.
    /// </summary>
    public class RCXnaGame : Game
    {
        private GraphicsDeviceManager _deviceMgr = null;

        /// <summary>
        /// The graphics device manager for the game.
        /// </summary>
        protected GraphicsDeviceManager DeviceMgr
        {
            get { return _deviceMgr; }
            set { _deviceMgr = value; }
        }

        private RCPluginManager _pluginMgr = null;

        /// <summary>
        /// The plugin manager for the game.
        /// </summary>
        protected RCPluginManager PluginMgr
        {
            get { return _pluginMgr; }
            set { _pluginMgr = value; }
        }
        
        private IRCContentManager _contentMgr = null;

        /// <summary>
        /// The content manager for the game.
        /// </summary>
        protected IRCContentManager ContentMgr
        {
            get { return _contentMgr; }
            set { _contentMgr = value; }
        }

        private IRCCameraManager _cameraMgr = null;

        /// <summary>
        /// The camera manager for the game.
        /// </summary>
        protected IRCCameraManager CameraMgr
        {
            get { return _cameraMgr; }
            set { _cameraMgr = value; }
        }

        private IRCGameStateManager _stateMgr = null;

        /// <summary>
        /// The state manager for the game.
        /// </summary>
        protected IRCGameStateManager StateMgr
        {
            get { return _stateMgr; }
            set { _stateMgr = value; }
        }

        private IRCGameStateStack _stateStk = null;

        /// <summary>
        /// The state stack for the game.
        /// </summary>
        protected IRCGameStateStack StateStk
        {
            get { return _stateStk; }
            set { _stateStk = value; }
        }

        private IRCRenderManager _renderMgr = null;

        /// <summary>
        /// The render manager for the game.
        /// </summary>
        protected IRCRenderManager RenderMgr
        {
            get { return _renderMgr; }
            set { _renderMgr = value; }
        }

        /// <summary>
        /// I am a contructor that will setup the graphics device manager.
        /// </summary>
        public RCXnaGame()
        {
            _pluginMgr = new RCPluginManager(this);
            _deviceMgr = new GraphicsDeviceManager(this);
            _contentMgr = new RCContentManager(this);
            _cameraMgr = new RCCameraManager(this);
            _stateMgr = new RCGameStateManager(this);
            _stateStk = (IRCGameStateStack) _stateMgr;
            _renderMgr = new RCRenderManager(this);

            //_deviceMgr.IsFullScreen = true;
            _deviceMgr.PreferredBackBufferWidth = 800;
            _deviceMgr.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// I initialize stuff.
        /// </summary>
        protected override void Initialize()
        {
            Components.Add(_contentMgr);
            Components.Add(_stateMgr);
            base.Initialize();
        }

        /// <summary>
        /// Called on an update pass.
        /// </summary>
        /// <param name="gameTime">The current gametime.</param>
        protected override void Update(GameTime gameTime)
        {
            RCPluginManager.GameTimeEventArgs args = new RCPluginManager.GameTimeEventArgs(gameTime);
            _pluginMgr.RaiseUpdateEvent(args);
            base.Update(gameTime);
        }
    }
}

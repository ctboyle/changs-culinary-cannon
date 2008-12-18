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
        /// <summary>
        /// The graphics device manager for the game.
        /// </summary>
        protected GraphicsDeviceManager _deviceMgr = null;

        /// <summary>
        /// The plugin manager for the game.
        /// </summary>
        protected RCPluginManager _pluginMgr = null;

        /// <summary>
        /// I am a contructor that will setup the graphics device manager.
        /// </summary>
        public RCXnaGame()
        {
            _pluginMgr = new RCPluginManager(this);

            _deviceMgr = new GraphicsDeviceManager(this);
            //_deviceMgr.IsFullScreen = true;
            _deviceMgr.PreferredBackBufferWidth = 1920;
            _deviceMgr.PreferredBackBufferHeight = 1200;
        }

        /// <summary>
        /// I initialize stuff.
        /// </summary>
        protected override void Initialize()
        {
            RCContentManager contentMgr = new RCContentManager(this);
            RCCameraManager cameraMgr = new RCCameraManager(this);
            RCGameStateManager stateMgr = new RCGameStateManager(this);
            RCRenderManager renderMgr = new RCRenderManager(this);

            Components.Add(contentMgr);
            Components.Add(stateMgr);

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

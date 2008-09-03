using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RC.Engine.StateManagement;
using RC.Engine.Rendering;
using RC.Engine.Cameras;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine
{
    public class RCBasicGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private IRCRenderManager _renderMgr;

        public RCBasicGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _renderMgr = new RCRenderManager();

            Services.AddService(typeof(IRCRenderManager), _renderMgr);
            Services.AddService(typeof(IRCGameStateManager), new RCGameStateManager(this));
            Services.AddService(typeof(IRCCameraManager), new RCCameraManager());
        }

        protected override void LoadContent()
        {
            _renderMgr.Load(Services);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _renderMgr.Unload();
            base.UnloadContent();
        }
    }
}
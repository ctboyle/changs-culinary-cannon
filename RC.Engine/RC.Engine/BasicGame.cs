using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RC.Engine.StateManagement;
using RC.Engine.Rendering;
using RC.Engine.Cameras;
using RC.Engine.Utility;

namespace RC.Engine
{
    public class RCBasicGame : Game
    {
        private RCGameStateManager stateMgr;
        private GraphicsDeviceManager graphics;
        private RCLoadableCollection mgrs = new RCLoadableCollection();

        public RCBasicGame()
        {
            graphics = new GraphicsDeviceManager(this);
            stateMgr = new RCGameStateManager(this);
            mgrs.Add(new RCRenderManager(this));
            mgrs.Add(new RCCameraManager(this));
            Services.AddService(typeof(ContentManager), this.Content);
        }

        protected IRCGameStateManager StateManager
        {
            get { return stateMgr; }
        }
        
        protected override void Initialize()
        {
            Components.Add(stateMgr);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach(IRCLoadable mgr in mgrs)
            {
                mgr.Load();
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (IRCLoadable mgr in mgrs)
            {
                mgr.Unload();
            }

            base.UnloadContent();
        }
    }
}
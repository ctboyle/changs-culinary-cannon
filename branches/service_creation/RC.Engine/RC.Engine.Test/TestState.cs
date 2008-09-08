using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using RC.Engine.GraphicsManagement;
using RC.Engine.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Input;
using RC.Engine.StateManagement;

namespace RC.Engine.Test
{
    class TestState : RC.Engine.StateManagement.RCGameState
    {
        public TestState(IServiceProvider services)
            : base(services)
        {
        }

        protected ContentManager ContentMgr
        {
            get { return Services.GetService(typeof(ContentManager)) as ContentManager; }
        }

        protected GraphicsDevice Graphics
        {
            get
            {
                IGraphicsDeviceService service = Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
                return service.GraphicsDevice;
            }
        }

        protected IRCGameStateStack GameStateStack
        {
            get { return Services.GetService(typeof(IRCGameStateStack)) as IRCGameStateStack; }
        }

        protected IRCCameraManager CameraMgr
        {
            get { return Services.GetService(typeof(IRCCameraManager)) as IRCCameraManager; }
        }

        protected IRCRenderManager RenderMgr
        {
            get { return Services.GetService(typeof(IRCRenderManager)) as IRCRenderManager; }
        }
    }
}
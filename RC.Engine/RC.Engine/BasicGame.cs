using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RC.Engine.StateManagement;
using RC.Engine.Rendering;
using RC.Engine.Cameras;

using Ninject.Core;
using Ninject.Core.Binding.Syntax;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.ContentManagement;

namespace RC.Engine
{
    [Singleton]
    public class RCBasicGame : Game
    {
        private GraphicsDeviceManager _deviceMgr = null;
        private IRCGameStateManager _stateMgr = null;
        private IRCRenderManager _renderMgr = null;
        private IRCContentRequester _content = null;

        public RCBasicGame()
        {
            _deviceMgr = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            Components.Add(ContentRqst);
            Components.Add(StateMgr);
            base.Initialize();
        }

        [Inject]
        public IRCContentRequester ContentRqst
        {
            get { return _content; }
            set { _content = value; }
        }

        [Inject]
        public IRCRenderManager RenderMgr
        {
            get { return _renderMgr; }
            set { _renderMgr = value; }
        }

        [Inject]
        public IRCGameStateManager StateMgr
        {
            get { return _stateMgr; }
            set { _stateMgr = value; }
        }
    }
}
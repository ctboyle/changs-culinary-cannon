using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.StateManagement;
using RC.Engine.ContentManagement;
using RC.Engine.Cameras;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Ninject.Core;
using RC.Engine.Base;
using Microsoft.Xna.Framework;

namespace RC.Engine.Ninject
{
    internal class NinjectGameContext : RCGameContext
    {
        private IRCContentRequester _content = null;
        private IRCGameStateStack _stateStack = null;
        private IRCGameStateManager _stateMgr = null;
        private IRCCameraManager _cameraMgr = null;
        private IRCRenderManager _renderMgr = null;
        private IGraphicsDeviceService _graphics = null;
        private Game _game = null;
        
        [Inject]
        public IRCContentRequester ContentRqst
        {
            set { _content = value; }
            get { return _content; }
        }

        [Inject]
        public IRCGameStateManager StateMgr
        {
            set { _stateMgr = value; }
            get { return _stateMgr; }
        }

        [Inject]
        public IRCGameStateStack StateStack
        {
            set { _stateStack = value; }
            get { return _stateStack; }
        }

        [Inject]
        public IRCCameraManager CameraMgr
        {
            set { _cameraMgr = value; }
            get { return _cameraMgr; }
        }

        [Inject]
        public IRCRenderManager RenderMgr
        {
            set { _renderMgr = value; }
            get { return _renderMgr; }
        }

        [Inject]
        public IGraphicsDeviceService Graphics
        {
            set { _graphics = value; }
            get { return _graphics; }
        }

        [Inject]
        public Game Game
        {
            get { return _game; }
            set { _game = value; }
        }
    }
}
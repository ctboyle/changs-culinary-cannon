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
    public class RCBasicGame
    {
        private IRCCameraManager _cameraMgr = null;
        private IRCGameStateManager _stateMgr = null;
        private IRCContentRequester _content = null;

        public virtual void Initialize()
        {
        }

        public virtual void BeginRun()
        {
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

        [Inject]
        public IRCCameraManager CameraMgr
        {
            get { return _cameraMgr; }
            set { _cameraMgr = value; }
        }
    }
}
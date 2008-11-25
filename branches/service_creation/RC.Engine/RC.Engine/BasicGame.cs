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
    /// <summary>
    /// I am the basic game class that all games must provide to
    /// the <see cref="RCGameStarter"/>.  I am here to do any
    /// initialization to be performed.  The initialization includes
    /// setting up the states with state manager, etc. 
    /// </summary>
    [Singleton]
    public class RCBasicGame
    {
        private IRCCameraManager _cameraMgr = null;
        private IRCGameStateManager _stateMgr = null;
        private IRCContentRequester _content = null;

        /// <summary>
        /// I am used to initialize anything that needs to be initialized.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// I am used to do any last minute initialization before the
        /// Update and Draw loops start.
        /// </summary>
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
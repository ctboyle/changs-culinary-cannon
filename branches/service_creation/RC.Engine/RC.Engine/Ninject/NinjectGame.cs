using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RC.Engine.ContentManagement;
using RC.Engine.StateManagement;
using RC.Engine.Base;
using Ninject.Core;

namespace RC.Engine.Ninject
{
    internal class NinjectGame : RCXnaGame
    {
        private IRCContentRequester _contentRqst = null;
        private IRCGameStateManager _stateMgr = null;
        private RCBasicGame _game = null;
        private INinjectUpdatePlugin _updatePlug = null;

        [Inject]
        public override IRCContentRequester ContentRqst
        {
            get { return _contentRqst; }
            set { _contentRqst = value; }
        }

        [Inject]
        public override IRCGameStateManager StateMgr
        {
            get { return _stateMgr; }
            set { _stateMgr = value; }
        }

        [Inject]
        public override RCBasicGame Game
        {
            get { return _game; }
            set { _game = value; }
        }

        [Inject]
        public INinjectUpdatePlugin UpdatePlug
        {
            get { return _updatePlug; }
            set { _updatePlug = value; }
        }

        protected override void Update(GameTime gameTime)
        {
            UpdatePlug.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
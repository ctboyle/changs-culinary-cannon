using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using RC.Engine.StateManagement;
using RC.Engine.GraphicsManagement;
using RC.Engine.Rendering;
using RC.Engine.Cameras;
using RC.Engine.ContentManagement;
using RC.Engine.Base;

namespace RC.Engine.StateManagement
{
    public class RCGameState : IDisposable
    {
        private bool _isVisible = true;
        private bool _isUpdated = true;
        private IServiceProvider _services = null;

        public RCGameState(IServiceProvider services)
        {
            _services = services;

            IRCGameStateManager stateMgr = (IRCGameStateManager)services.GetService(typeof(IRCGameStateManager));
            stateMgr.StateChanged += new StateChangeHandler(StateChanged);
        }

        ~RCGameState()
        {
            Dispose();
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        public IServiceProvider Services
        {
            get { return _services; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool IsUpdateable
        {
            get { return _isUpdated; }
            set { _isUpdated = value; }
        }

        public virtual void Initialize()
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        protected virtual void StateChanged(
            RCGameState newState,
            RCGameState oldState
            )
        {
            if (newState == this)
            {
                IsVisible = IsUpdateable = true;
            }
            else
            {
                IsVisible = IsUpdateable = false;
            }
        }
    }
}

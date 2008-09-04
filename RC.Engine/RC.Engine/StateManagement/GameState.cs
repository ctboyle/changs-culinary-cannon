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

namespace RC.Engine.StateManagement
{
    public abstract partial class RCGameState
    {
        private IRCGameStateManager _gameManager = null;
        private bool _isVisible = true;
        private bool _isUpdated = true;

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool IsUpdated
        {
            get { return _isUpdated; }
            set { _isUpdated = value; }
        }

        public virtual void Load(IServiceProvider services)
        {
            _gameManager = services.GetService(typeof(IRCGameStateManager)) as IRCGameStateManager;
        }

        public virtual void Unload()
        {

        }

        public abstract void Draw(GameTime gameTime, IServiceProvider services);

        public abstract void Update(GameTime gameTime, IServiceProvider services);

        protected IRCGameStateManager GameStateMgr
        {
            get { return _gameManager; }
        }

        internal protected virtual void StateChanged(
            RCGameState newState,
            RCGameState oldState
            )
        {
            if (newState == this)
            {
                IsVisible = IsUpdated = true;
            }
            else
            {
                IsVisible = IsUpdated = false;
            }
        }
    }
}
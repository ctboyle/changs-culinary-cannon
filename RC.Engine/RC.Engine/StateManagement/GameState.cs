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
    public class RCGameState
    {
        private IServiceProvider _services = null;
        private bool _isVisible = true;
        private bool _isUpdated = true;

        public RCGameState(IServiceProvider services)
        {
            _services = services;
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

        public virtual void Load()
        {
        }

        public virtual void Unload()
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        protected IServiceProvider Services
        {
            get { return _services; }
        }

        internal protected virtual void StateChanged(
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
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
        public abstract void Initialize(IServiceProvider services);

        public abstract void LoadContent(ContentManager content, IServiceProvider services);

        public abstract void UnloadContent();

        public abstract void StateChanged(RCGameState newState, RCGameState oldState);

        public abstract void Draw(GameTime gameTime, IServiceProvider services);

        public abstract void Update(GameTime gameTime);
    }
}
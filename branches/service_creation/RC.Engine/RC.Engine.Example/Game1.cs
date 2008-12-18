using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using RC.Engine.Base;
using RC.Engine.ContentManagement;
using RC.Engine.Cameras;
using RC.Engine.Rendering;
using RC.Engine.GraphicsManagement;
using RC.Engine.ContentManagement.ContentTypes;
using RC.Engine.SceneEffects;
using RC.Engine.StateManagement;

namespace RC.Engine.Example
{
    public class Game1 : RCXnaGame
    {
        protected override void Initialize()
        {
            StateMgr.StateChanged += OnStateChanged;

            StateMgr.AddState("GameState", new RCGameState(Services));

            base.Initialize();
        }

        protected override void BeginRun()
        {
            StateStk.PushState("GameState");

            base.BeginRun();
        }

        private void OnStateChanged(RCGameState newState, RCGameState previousState)
        {
            if (newState == null)
            {
                Exit();
            }
        }
    }
}

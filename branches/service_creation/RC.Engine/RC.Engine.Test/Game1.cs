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
using RC.Engine.StateManagement;
using RC.Engine.Base;
using RC.Physics;

namespace RC.Engine.Test
{
    ///// <summary>
    ///// This is the main type for your game
    ///// </summary>
    public class Game1 : RCXnaGame
    {
        private static String GameStart = "Start";

        protected override void Initialize()
        {
            StateMgr.StateChanged += new StateChangeHandler(OnStateChanged);

            StateMgr.AddState(GameStart, new GameState(Services));

            base.Initialize();
        }

        protected override void BeginRun()
        {
            StateStk.PushState(GameStart);

            base.BeginRun();
        }

        private void OnStateChanged(RCGameState newState, RCGameState oldState)
        {
            if (newState == null)
            {
                Exit();
            }
        }
    }
}
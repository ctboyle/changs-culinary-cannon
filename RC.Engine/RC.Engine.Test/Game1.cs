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

namespace RC.Engine.Test
{
    ///// <summary>
    ///// This is the main type for your game
    ///// </summary>
    class Game1 : RC.Engine.RCBasicGame
    {
        private static String GameStart = "Start";

        public override void Initialize()
        {
            StateMgr.AddState(GameStart, typeof(TestState));
            base.Initialize();
        }

        public override void BeginRun()
        {
            StateMgr.PushState(GameStart);
            base.BeginRun();
        }
    }
}
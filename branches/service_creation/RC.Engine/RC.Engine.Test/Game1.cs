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
    public class Game1 : RCBasicGame
    {
        private static String GameStart = "Start";

        public Game1(RCGameContext gameCtx)
            : base(gameCtx)
        {
        }

        public override void Initialize()
        {
            Ctx.StateMgr.AddState(GameStart, typeof(GameState));
            base.Initialize();
        }

        public override void BeginRun()
        {
            Ctx.StateStack.PushState(GameStart);
            base.BeginRun();
        }
    }
}
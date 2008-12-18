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

        private IRCGameStateManager _stateMgr = null;
        private IRCGameStateStack _stateStk = null;

        protected override void Initialize()
        {
            base.Initialize();

            _stateMgr = (IRCGameStateManager)Services.GetService(typeof(IRCGameStateManager));
            _stateStk = (IRCGameStateStack)Services.GetService(typeof(IRCGameStateStack));

            _stateMgr.AddState(GameStart, new GameState(this));
        }

        protected override void BeginRun()
        {
            _stateStk.PushState(GameStart);
            base.BeginRun();
        }
    }
}
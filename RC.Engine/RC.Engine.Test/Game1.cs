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
using Ninject.Core;

namespace RC.Engine.Test
{
    ///// <summary>
    ///// This is the main type for your game
    ///// </summary>
    [Singleton]
    class Game1 : RC.Engine.RCBasicGame
    {
        private RCGameState _startState = null;

        public override void Initialize()
        {
            StateMgr.AddState("Test", StartState);
            base.Initialize();
        }

        public override void BeginRun()
        {
            StateMgr.PushState("Test");
            base.BeginRun();
        }

        [Inject, Tag("Start")]
        public RCGameState StartState
        {
            get { return _startState; }
            set { _startState = value; }
        }
    }
}
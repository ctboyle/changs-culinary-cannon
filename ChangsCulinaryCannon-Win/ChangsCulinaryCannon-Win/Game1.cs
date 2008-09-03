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

namespace ChangsCulinaryCannon
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : RC.Engine.RCBasicGame
    {
        protected override void BeginRun()
        {
            IRCGameStateManager stateMgr = Services.GetService(typeof(IRCGameStateManager)) as IRCGameStateManager;
            TestState state = new TestState();
            stateMgr.PushState(state);
            base.BeginRun();
        }
    }
}

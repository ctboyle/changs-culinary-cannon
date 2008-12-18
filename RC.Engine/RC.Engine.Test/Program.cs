using System;
using RC.Engine.StateManagement;
using System.Collections.Generic;
using RC.Engine.Base;
using RC.Physics;
using RC.Engine.Plugin;

namespace RC.Engine.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Game1 game = new Game1();
            JigLibXModule mod = new JigLibXModule(game);
            game.Run();
        }
    }
}
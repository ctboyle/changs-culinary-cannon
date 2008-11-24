using System;
using RC.Engine.StateManagement;
using System.Collections.Generic;

namespace RC.Engine.Test
{
    static class Program
    {
        private static Type GameType = typeof(Game1);
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            RCGameStarter.Start(GameType);
        }
    }
}
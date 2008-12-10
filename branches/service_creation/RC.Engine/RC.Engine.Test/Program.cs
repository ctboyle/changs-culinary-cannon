using System;
using RC.Engine.StateManagement;
using System.Collections.Generic;
using RC.Engine.Base;
using RC.Physics;

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
            RCGameManagerFactory factory = new RCDefaultGameManagerFactory();
            RCGameManager mgr = factory.GetInstance();
            //mgr.LoadModule(new RCPhysicsModule());
            mgr.Start(GameType);
        }
    }
}
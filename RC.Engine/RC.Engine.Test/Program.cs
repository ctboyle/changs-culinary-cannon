using System;
using Ninject.Core;
using RC.Engine.StateManagement;

namespace RC.Engine.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            RCGameStarter g = new RCGameStarter(new GameModule());
            g.Start();
        }
    }
}
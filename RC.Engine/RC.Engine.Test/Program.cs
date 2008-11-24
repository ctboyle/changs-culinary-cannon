using System;
using Ninject.Core;
using RC.Engine.StateManagement;
using System.Collections.Generic;

namespace RC.Engine.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            RCGameStarter g = new RCGameStarter(
                typeof(Game1),
                new KeyValuePair<String, Type>[] 
                { 
                    new KeyValuePair<String, Type>("Start", typeof(TestState))
                }
            );

            g.Start();
        }
    }
}
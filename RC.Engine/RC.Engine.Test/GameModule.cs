using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Core;
using RC.Engine.StateManagement;
using Ninject.Conditions;

namespace RC.Engine.Test
{
    class GameModule : StandardModule
    {
        public override void Load()
        {
            Bind<RCBasicGame>().To<Game1>();
            Bind<RCGameState>().
                To<TestState>().
                Only(When.Context.Target.Tag.EqualTo("Start"));
        }
    }
}
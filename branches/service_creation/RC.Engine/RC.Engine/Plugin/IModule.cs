using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Plugin;
using RC.Engine.Base;

namespace RC.Engine.Plugin
{
    public interface IRCModule
    {
        void Plugin(RCGameManager gameMgr, RCPluginManager pluginMgr);
    }
}
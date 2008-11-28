using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Plugin;

namespace RC.Engine.Plugin
{
    public interface IRCModule
    {
        void Plugin(RCPluginManager pluginMgr);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine;
using JigLibX.Physics;
using RC.Engine.Base;
using RC.Engine.Plugin;

namespace RC.Physics
{
    public class RCPhysicsModule : IRCModule
    {
        public void Plugin(RCPluginManager pluginMgr)
        {
            pluginMgr.UpdateEvent += new EventHandler<RCPluginManager.GameTimeEventArgs>(UpdateEvent);
        }

        private void UpdateEvent(object sender, RCPluginManager.GameTimeEventArgs e)
        {
        }
    }
}
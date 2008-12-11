using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine;
using JigLibX.Physics;
using RC.Engine.Base;
using RC.Engine.Plugin;
using JigLibX.Collision;
using Microsoft.Xna.Framework;

namespace RC.Physics
{
    public class RCPhysicsModule : IRCModule
    {
        private PhysicsSystem physicSystem = null;

        public RCPhysicsModule()
        {
            physicSystem = new PhysicsSystem();
            physicSystem.Gravity = new Vector3(0, -0.9f, 0);
            physicSystem.CollisionSystem = new CollisionSystemGrid(32, 32, 32, 30, 30, 30);

            physicSystem.EnableFreezing = true;
            physicSystem.SolverType = PhysicsSystem.Solver.Accumulated;
            physicSystem.CollisionSystem.UseSweepTests = true;
        }

        public void Plugin(RCGameManager gameMgr, RCPluginManager pluginMgr)
        {
            pluginMgr.UpdateEvent += new EventHandler<RCPluginManager.GameTimeEventArgs>(UpdateEvent);
        }

        private void UpdateEvent(object sender, RCPluginManager.GameTimeEventArgs e)
        {
            GameTime gt = e.GameTime;
            physicSystem.Integrate((float)gt.ElapsedGameTime.TotalSeconds);
        }
    }
}
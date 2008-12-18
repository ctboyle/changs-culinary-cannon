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
    public class JigLibXModule
    {
        private PhysicsSystem physicSystem = null;

        public JigLibXModule()
        {
            physicSystem = new PhysicsSystem();
            physicSystem.Gravity = new Vector3(0, -15f, 0);
            physicSystem.CollisionSystem = new CollisionSystemGrid(32, 32, 32, 30, 30, 30);

            physicSystem.EnableFreezing = true;
            physicSystem.SolverType = PhysicsSystem.Solver.Normal;
            physicSystem.CollisionSystem.UseSweepTests = true;
        }

        public void Plugin(Game game)
        {
            RCPluginManager pluginMgr = (RCPluginManager)game.Services.GetService(typeof(RCPluginManager));

            pluginMgr.UpdateEvent += new EventHandler<RCPluginManager.GameTimeEventArgs>(UpdateEvent);
        }

        private void UpdateEvent(object sender, RCPluginManager.GameTimeEventArgs e)
        {
            GameTime gt = e.GameTime;
            physicSystem.Integrate((float)gt.ElapsedGameTime.TotalSeconds);
        }
    }
}
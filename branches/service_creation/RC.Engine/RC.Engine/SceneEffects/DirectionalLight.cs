using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RC.Engine.GraphicsManagement;

namespace RC.Engine.SceneEffects
{
    /// <summary>
    /// Acts as a scene graph container for a single RCLight. 
    /// 
    /// Allows the light's position information to be updated
    /// to match the world transform of the light node.
    /// 
    /// </summary>
    public class RCLightNode : RCSceneNode
    {
        protected RCLight _light;

        public void SetLight(RCLight light)
        {
            _light = light;

            // Take the light's transform for its initial local transform.
            if (_light != null)
            {
                LocalTrans = _light.Transform;
            }
        }

        protected override void UpdateWorldData(GameTime gameTime)
        {
            base.UpdateWorldData(gameTime);

            // Update the light's cached postion with that of the light node.
            if (_light != null)
            {
                _light.Transform = WorldTrans;
            }
        }
    }
}
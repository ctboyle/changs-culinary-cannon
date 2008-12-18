using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RC.Engine.SceneEffects
{
    /// <summary>
    /// A light object that can be used as a light source in the scene.
    /// </summary>
    public class RCLight
    {
        enum LightType
        {
            AMBIENT,
            DIRECTIONAL,
            POINT,
            SPOT
        };

        //LightType Type;

        /// <summary>
        /// Location in world space.
        /// </summary>
        public Matrix Transform = Matrix.Identity;

        /// <summary>
        /// Colors of the light source.
        /// </summary>
        public Vector3 Diffuse, Specular;
          
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RC.Engine.SceneEffects
{
 
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

        public Matrix Transform = Matrix.Identity;

        // Color Members
        public Vector3 Diffuse, Specular;
          
    }
}

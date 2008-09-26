using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RC.Engine.Rendering
{
    public class RCMaterialState : RCRenderState
    {
        public Color Emissive   = new Color(new Vector3(0.0f, 0.0f, 0.0f));
        public Color Ambient    = new Color(new Vector3(0.2f, 0.2f, 0.2f));
        public Color Diffuse    = new Color(new Vector3(0.8f, 0.8f, 0.8f));
        public Color Specular   = new Color(new Vector3(0.0f, 0.0f, 0.0f));
        public float Alpha      = 1.0f;
        public float Shininess  = 1.0f;

        static RCMaterialState()
        {
            Default[StateType.Material] = new RCMaterialState();
        }

        public override StateType GetStateType()
        {
            return StateType.Material;
        }

        public override void ConfigureDevice(GraphicsDevice device)
        {
            // Do nothing on the device
        }
    }
}

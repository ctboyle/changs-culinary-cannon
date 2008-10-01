using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Rendering
{
    public abstract class RCRenderState
    {
        static RCRenderState()
        {
            Default.Add(new RCAlphaState());
            Default.Add(new RCMaterialState());
            Default.Add(new RCDepthBufferState());
        }


        // supported global states
        public enum StateType
        {
            Alpha,
            Material,
            Depth
            //CULL,
            //FOG,
            //POLYGONOFFSET,
            //STENCIL,
            //WIREFRAME, 
        };

        public abstract StateType GetStateType();

        public abstract void ConfigureDevice(GraphicsDevice device);

        // default states
        public static RCRenderStateCollection Default = new RCRenderStateCollection(false);
    }
}

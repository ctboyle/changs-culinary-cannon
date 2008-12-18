using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Rendering
{
    /// <summary>
    /// Container class for defining render states.
    /// </summary>
    public abstract class RCRenderState
    {
        /// <summary>
        /// Creates the collection of default render states.
        /// </summary>
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

        /// <summary>
        /// Gets the render state's type.
        /// </summary>
        /// <returns></returns>
        public abstract StateType GetStateType();

        /// <summary>
        /// Defines how to configure the graphics device for each state. 
        /// </summary>
        /// <param name="device"></param>
        public abstract void ConfigureDevice(GraphicsDevice device);

        /// <summary>
        /// The collection of default renderstates.
        /// </summary>
        public static RCRenderStateCollection Default = new RCRenderStateCollection(false);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Rendering
{
    public class RCDepthBufferState : RCRenderState
    {
        public float DepthBias = 0.0f;
        public bool  DepthTestingEnabled = false;
        public CompareFunction DepthTest = CompareFunction.LessEqual;
        public bool DepthBufferWriteEnabled = true;

        public override RCRenderState.StateType GetStateType()
        {
            return StateType.Depth;
        }

        public override void ConfigureDevice(Microsoft.Xna.Framework.Graphics.GraphicsDevice device)
        {
            device.RenderState.DepthBufferEnable = DepthTestingEnabled;
            device.RenderState.DepthBufferFunction = DepthTest;
            device.RenderState.DepthBufferWriteEnable = DepthBufferWriteEnabled;
            device.RenderState.DepthBias = DepthBias;
        }
    }
}

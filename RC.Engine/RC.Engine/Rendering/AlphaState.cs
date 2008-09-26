using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Rendering
{
    public class RCAlphaState : RCRenderState
    {
        public bool BlendEnabled = false;      
        public Blend SrcBlend = Blend.SourceAlpha;  
        public Blend DstBlend = Blend.InverseSourceAlpha;
        public BlendFunction BlendFunction = BlendFunction.Add;
        
        public bool SeparateAlphaBlendEnabled = false;
        public Blend SeparateAlphaSrcBlend = Blend.SourceAlpha;
        public Blend SeparateAlphaDstBlend = Blend.InverseSourceAlpha;
        public BlendFunction SeparateAlphaBelndFucntion = BlendFunction.Add;

        public bool TestEnabled = false;
        public CompareFunction Test = CompareFunction.Always;
        
        public Color BlendFactor = new Color(0,0,0,0);

        public override void  ConfigureDevice(GraphicsDevice device)
        {
            device.RenderState.AlphaBlendEnable = BlendEnabled;
            device.RenderState.SourceBlend = SrcBlend;
            device.RenderState.DestinationBlend = DstBlend;
            device.RenderState.BlendFunction = BlendFunction;

            device.RenderState.SeparateAlphaBlendEnabled = SeparateAlphaBlendEnabled;
            device.RenderState.AlphaSourceBlend = SeparateAlphaSrcBlend;
            device.RenderState.AlphaDestinationBlend = SeparateAlphaDstBlend;
            device.RenderState.AlphaBlendOperation = SeparateAlphaBelndFucntion;

            device.RenderState.AlphaTestEnable = TestEnabled;
            device.RenderState.AlphaFunction = Test;

            device.RenderState.BlendFactor = BlendFactor;
        }

        public override StateType GetStateType()
        {
            return StateType.Alpha;
        }
    }
}

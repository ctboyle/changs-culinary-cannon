using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;
using RC.Engine.SceneEffects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;
using Ninject.Core;

namespace RC.Engine.GraphicsManagement
{
    public class RCGeometry : RCSpatial
    {
        public RCRenderStateCollection RenderStates = new RCRenderStateCollection(false);

        private RCLightEffect _lightEffect = null;      
        private RCIndexBuffer _iBuffer = null;     
        private RCVertexBuffer _vBuffer = null;
        private IRCContentRequester _contentRqst = null;
        private IGraphicsDeviceService _graphics = null;

        public RCIndexBuffer IBuffer
        {
            get { return _iBuffer; }
        }

        public RCVertexBuffer VBuffer
        {
            get { return _vBuffer; }
        }

        public RCGeometry(IGraphicsDeviceService graphics, IRCContentRequester contentRqst, RCIndexBuffer indexBuffer, RCVertexBuffer vertexBuffer)
        {
            _graphics = graphics;
            _contentRqst = contentRqst;
            _iBuffer = indexBuffer;
            _vBuffer = vertexBuffer;
        }
        
        public override void Draw(IRCRenderManager render)
        {
            _iBuffer.Enabled = true;
            _vBuffer.Enabled = true;
            
            render.Draw(this);

            _iBuffer.Enabled = false;
            _vBuffer.Enabled = false;
        }

        protected override void UpdateState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            foreach (RCRenderState.StateType type in Enum.GetValues(typeof(RCRenderState.StateType)))
            {
                RenderStates.Add(stateStack[type]);
            }

            // If lights, add lighting effect and update them.
            if (lightStack.Count > 0)
            {
                if (_lightEffect != null)
                {
                    _lightEffect.RemoveAllLights();
                }
                else
                {
                    // Create a new light effect and put it to be rendered first in the list.
                    _lightEffect = new RCLightEffect(_contentRqst);
                    Effects.Insert(0, _lightEffect);
                }
                // Make sure the

                foreach (RCLight light in lightStack)
                {
                    _lightEffect.AddLight(light);
                }
            }
            else
            {
                if (_lightEffect != null)
                {
                    // No lights, remove it from the list.
                    Effects.Remove(_lightEffect);
                    _lightEffect = null; // TODO: See about keeping the refrence to avoid GC
                }
            }
        }

        protected override void UpdateWorldBound()
        {
            
        }
    }
}